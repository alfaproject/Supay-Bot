using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Alog(CommandContext bc)
        {
            int timeSpan = 0;
            string timeSpanName = string.Empty;
            Match timeInterval = Regex.Match(bc.Message, @"@(.+?)( |$)");
            if (timeInterval.Success)
            {
                var timePeriod = new TimeInterval();
                if (timePeriod.Parse(timeInterval.Groups[1].Value))
                {
                    timeSpan = (int) timePeriod.Time.TotalSeconds;
                    timeSpanName = timePeriod.Name;
                }
                else if (timeInterval.Groups[1].Value == "all")
                {
                    timeSpan = int.MaxValue;
                }
                bc.Message = Regex.Replace(bc.Message, @"@(.+?)( |$)", string.Empty, RegexOptions.IgnoreCase);
                bc.Message = bc.Message.Trim();
            }

            string rsn = bc.GetPlayerName(bc.From.Nickname);
            if (bc.MessageTokens.GetLength(0) > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }

            List<RssItem> list = null;
            try
            {
                var reader = new RssManager("http://services.runescape.com/m=adventurers-log/rssfeed?searchName=" + rsn);
                reader.GetFeed();
                list = reader.RssItems;
            }
            catch
            {
            }

            if (list == null)
            {
                await bc.SendReply(@"No achievements found for \c7{0}\c{1}. The profile may be private, the player may be f2p, or the rsn incorrect.", rsn, string.IsNullOrEmpty(timeSpanName) ? string.Empty : @" in " + timeSpanName);
                return;
            }

            var p = await Player.FromHiscores(rsn);
            list.Sort((i1, i2) => i2.Date.CompareTo(i1.Date));
            if (timeSpan > 0)
            {
                list.RemoveAll(i => (DateTime.UtcNow - i.Date).TotalSeconds > timeSpan);
            }
            else if (list.Count > 15)
            {
                list.RemoveAll(i => i.Date < list[14].Date);
                timeSpanName = "recent";
            }
            if (list.Count == 0 || !p.Ranked)
            {
                await bc.SendReply(@"No achievements found for \c7{0}\c{1}. The profile may be private, the player may be f2p, or the rsn incorrect.", rsn, string.IsNullOrEmpty(timeSpanName) ? string.Empty : @" in " + timeSpanName);
                return;
            }

            const string questRegex = @"Quest complete: (.+)";
            const string killRegex = @"killed the player (.+?)\.|I killed\s*(?:an?|the)?\s*(.+?)\.?$";
            const string levelRegex = @"Level?led up (\w+)\.?|Levelled all skills over (\d+)";
            const string itemRegex = @"Item found: (?:an?|some) (.+)";
            const string expRegex = @"(\d+)XP in (\w+)";
            const string duRegex = @"Dungeon level (\d+) reached.";

            var alogItems = new Dictionary<string, Dictionary<string, AlogItem>> {
                { "I reached", new Dictionary<string, AlogItem>() },
                { "I gained", new Dictionary<string, AlogItem>() },
                { "I killed", new Dictionary<string, AlogItem>() },
                { "I found", new Dictionary<string, AlogItem>() },
                { "I completed", new Dictionary<string, AlogItem>() },
                { "Others", new Dictionary<string, AlogItem>() }
            };
            foreach (RssItem item in list)
            {
                Match M = Regex.Match(item.Title, questRegex);
                if (M.Success)
                {
                    var quest = new AlogItem(item, M, "I completed");
                    alogItems["I completed"].Add(alogItems["I completed"].Count.ToStringI(), quest);
                    continue;
                }
                M = Regex.Match(item.Title, killRegex);
                if (M.Success)
                {
                    var kill = new AlogItem(item, M, "I killed");
                    string npc = Regex.Replace(kill.Info[0].Replace("monsters", "monster"), @"\W", " ");
                    if (alogItems["I killed"].ContainsKey(npc))
                    {
                        alogItems["I killed"][npc].Amount += kill.Amount;
                    }
                    else
                    {
                        kill.Info[0] = npc;
                        alogItems["I killed"].Add(npc, kill);
                    }
                    continue;
                }
                M = Regex.Match(item.Title, levelRegex);
                if (M.Success)
                {
                    var level = new AlogItem(item, M, "I gained");
                    try
                    {
                        level.Info[0] = Skill.Parse(level.Info[0]);
                        if (alogItems["I gained"].ContainsKey(level.Info[0]))
                        {
                            alogItems["I gained"][level.Info[0]].Amount++;
                        }
                        else
                        {
                            alogItems["I gained"].Add(level.Info[0], level);
                            alogItems["I gained"][level.Info[0]].Info[1] = p.Skills[level.Info[0]].Level.ToStringI();
                        }
                    }
                    catch
                    {
                        if (alogItems["I gained"].ContainsKey("all"))
                        {
                            if (level.Info[1].ToInt32() > alogItems["I gained"]["all"].Info[1].ToInt32())
                            {
                                alogItems["I gained"]["all"].Info[0] = "all";
                                alogItems["I gained"]["all"].Info[1] = level.Info[1];
                            }
                        }
                        else
                        {
                            alogItems["I gained"].Add("all", level);
                            alogItems["I gained"]["all"].Info[0] = "all";
                        }
                    }
                    continue;
                }
                M = Regex.Match(item.Title, itemRegex);
                if (M.Success)
                {
                    var drop = new AlogItem(item, M, "I found");
                    if (alogItems["I found"].ContainsKey(drop.Info[0]))
                    {
                        alogItems["I found"][drop.Info[0]].Amount++;
                    }
                    else
                    {
                        alogItems["I found"].Add(drop.Info[0], drop);
                    }
                    continue;
                }
                M = Regex.Match(item.Title, duRegex);
                if (M.Success)
                {
                    var duFloor = new AlogItem(item, M, "Others");
                    if (!alogItems["Others"].ContainsKey("duFloor"))
                    {
                        alogItems["Others"].Add("duFloor", duFloor);
                        alogItems["Others"]["duFloor"].Info[1] = "1";
                    }
                    alogItems["Others"]["duFloor"].Info[0] = "duFloor";
                    if (M.Groups[1].Value.ToInt32() > alogItems["Others"]["duFloor"].Info[1].ToInt32())
                    {
                        alogItems["Others"]["duFloor"].Info[1] = M.Groups[1].Value;
                    }
                    continue;
                }
                M = Regex.Match(item.Title, expRegex);
                if (M.Success)
                {
                    var exp = new AlogItem(item, M, "I reached");
                    if (alogItems["I reached"].ContainsKey(exp.Info[1]))
                    {
                        if (alogItems["I reached"][exp.Info[1]].Info[0].ToInt32() < exp.Info[0].ToInt32())
                        {
                            alogItems["I reached"].Remove(exp.Info[1]);
                            alogItems["I reached"].Add(exp.Info[1], exp);
                        }
                    }
                    else
                    {
                        alogItems["I reached"].Add(exp.Info[1], exp);
                    }
                    continue;
                }
                var other = new AlogItem(item, null, "Others");
                alogItems["Others"].Add(alogItems["Others"].Count.ToStringI(), other);
            }
            string reply = rsn + "'s achievements" + (string.IsNullOrEmpty(timeSpanName) ? string.Empty : " (" + timeSpanName + ")") + ": ";
            foreach (var category in alogItems)
            {
                if (category.Value.Count == 0)
                {
                    continue;
                }
                reply += category.Key + ": ";
                foreach (AlogItem item in category.Value.Values)
                {
                    string amount = string.Empty;
                    if (item.Amount > 1)
                    {
                        amount = @"\c07{0}\cx ".FormatWith(item.Amount);
                    }
                    if (category.Key == "I reached")
                    {
                        var skill = new Skill(item.Info[1], 1, 1);
                        reply += @"\c07{0}\c {1} exp; ".FormatWith(item.Info[0].ToInt32().ToShortString(1), skill.ShortName);
                    }
                    else if (category.Key == "I gained")
                    {
                        if (item.Info[0] == "all")
                        {
                            reply += @"All skills now at least \c07{0}\c; ".FormatWith(item.Info[1]);
                        }
                        else
                        {
                            var skill = new Skill(item.Info[0], 1, 1);
                            reply += @"\c07{0}\c {1} levels(->{2}); ".FormatWith(item.Amount, skill.ShortName, item.Info[1]);
                        }
                    }
                    else if (item.Info[0] == "duFloor")
                    {
                        reply += @"Unlocked dungeon floor \c07{0}\c; ".FormatWith(item.Info[1]);
                    }
                    else
                    {
                        reply += amount + @"\c07{0}\c; ".FormatWith(item.Info[0]);
                    }
                }
            }
            await bc.SendReply(reply.Trim());
        }
    }
}
