﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Graph(CommandContext bc)
        {
            string skill = Skill.OVER;
            string rsn = await bc.GetPlayerName(bc.From.Nickname);

            if (bc.MessageTokens.Length > 1)
            {
                if (Skill.TryParse(bc.MessageTokens[1], ref skill))
                {
                    if (bc.MessageTokens.Length > 2)
                    {
                        rsn = await bc.GetPlayerName(bc.MessageTokens.Join(2));
                    }
                }
                else
                {
                    rsn = await bc.GetPlayerName(bc.MessageTokens.Join(1));
                }
            }

            await bc.SendReply(@"\b{0}\b \c07{1}\c graph | level: \c12http://t.rscript.org/graph-{0}.{2}.lvl.png\c | exp: \c12http://t.rscript.org/graph-{0}.{2}.png\c | rank: \c12http://t.rscript.org/graph-{0}.{2}.rank.png\c", rsn, skill.ToLowerInvariant(), Skill.NameToId(skill));
        }

        public static async Task Track(CommandContext bc)
        {
            // get time
            int intervalTime = 604800;
            string intervalName = "1 week";
            Match interval = Regex.Match(bc.Message, @"@(\d+)?(second|minute|month|hour|week|year|sec|min|day|s|m|h|d|w|y)s?", RegexOptions.IgnoreCase);
            if (interval.Success)
            {
                if (interval.Groups[1].Value.Length > 0)
                {
                    intervalTime = int.Parse(interval.Groups[1].Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    intervalTime = 1;
                }
                if (intervalTime < 1)
                {
                    intervalTime = 1;
                }
                switch (interval.Groups[2].Value)
                {
                    case "second":
                    case "sec":
                    case "s":
                        intervalName = intervalTime + " second" + (intervalTime == 1 ? string.Empty : "s");
                        break;
                    case "minute":
                    case "min":
                    case "m":
                        intervalName = intervalTime + " minute" + (intervalTime == 1 ? string.Empty : "s");
                        intervalTime *= 60;
                        break;
                    case "hour":
                    case "h":
                        intervalName = intervalTime + " hour" + (intervalTime == 1 ? string.Empty : "s");
                        intervalTime *= 3600;
                        break;
                    case "day":
                    case "d":
                        intervalName = intervalTime + " day" + (intervalTime == 1 ? string.Empty : "s");
                        intervalTime *= 86400;
                        break;
                    case "week":
                    case "w":
                        intervalName = intervalTime + " week" + (intervalTime == 1 ? string.Empty : "s");
                        intervalTime *= 604800;
                        break;
                    case "month":
                        intervalName = intervalTime + " month" + (intervalTime == 1 ? string.Empty : "s");
                        intervalTime *= 2629746;
                        break;
                    case "year":
                    case "y":
                        intervalName = intervalTime + " year" + (intervalTime == 1 ? string.Empty : "s");
                        intervalTime *= 31556952;
                        break;
                }
                bc.Message = Regex.Replace(bc.Message, @"@(\d+)?(second|minute|month|hour|week|year|sec|min|day|s|m|h|d|w|y)s?", string.Empty, RegexOptions.IgnoreCase);
                bc.Message = bc.Message.Trim();
            }
            intervalName = "last " + intervalName;

            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = await bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = await bc.GetPlayerName(bc.From.Nickname);
            }

            // Get new player
            var PlayerNew = await Player.FromHiscores(rsn);
            if (!PlayerNew.Ranked)
            {
                await bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }

            // Get old player
            var PlayerOld = new Player(rsn, intervalTime);
            if (!PlayerOld.Ranked)
            {
                await bc.SendReply(@"\b{0}\b wasn't being tracked on {1}.", rsn, DateTime.UtcNow.AddSeconds(-intervalTime).ToStringI("yyyy-MMM-dd"));
                return;
            }

            // 1st line: overall / combat
            string ReplyMsg = @"\b{0}\b \u{1}\u skills:".FormatWith(rsn, intervalName);
            Skill OverallDif = PlayerNew.Skills[Skill.OVER] - PlayerOld.Skills[Skill.OVER];
            if (OverallDif.Exp <= 0)
            {
                await bc.SendReply(@"No performance for \b{0}\b within this period.", rsn);
            }
            else
            {
                Skill CombatDif = PlayerNew.Skills[Skill.COMB] - PlayerOld.Skills[Skill.COMB];

                string DifLevel = string.Empty;
                if (OverallDif.Level > 0)
                {
                    DifLevel = @" [\b+{0}\b]".FormatWith(OverallDif.Level);
                }
                ReplyMsg += @" \c07Overall\c lvl {0} \c3+{1}\c xp (Avg. hourly exp.: \c07{2}\c)".FormatWith(PlayerNew.Skills[Skill.OVER].Level + DifLevel, OverallDif.Exp.ToShortString(1), (OverallDif.Exp / (intervalTime / 3600.0)).ToShortString(0));
                DifLevel = string.Empty;
                if (CombatDif.Level > 0)
                {
                    DifLevel = @" [\b+{0}\b]".FormatWith(CombatDif.Level);
                }
                ReplyMsg += @"; \c7Combat\c lvl {0} \c03+{1}\c xp (\c07{2}%\c)".FormatWith(PlayerNew.Skills[Skill.COMB].Level + DifLevel, CombatDif.Exp.ToShortString(1), (CombatDif.Exp / (double) OverallDif.Exp * 100.0).ToShortString(1));
                await bc.SendReply(ReplyMsg);

                // 2nd line: skills list
                List<Skill> SkillsDif = (from SkillNow in PlayerNew.Skills.Values
                                         where SkillNow.Name != Skill.OVER && SkillNow.Name != Skill.COMB
                                         select SkillNow - PlayerOld.Skills[SkillNow.Name]).ToList();
                SkillsDif.Sort();

                ReplyMsg = @"\b{0}\b \u{1}\u skills:".FormatWith(rsn, intervalName);
                for (int i = 0; i < 10; i++)
                {
                    if (SkillsDif[i].Exp > 0)
                    {
                        DifLevel = string.Empty;
                        if (SkillsDif[i].Level > 0)
                        {
                            DifLevel = @" [\b+{0}\b]".FormatWith(SkillsDif[i].Level);
                        }
                        ReplyMsg += @" \c07{0}\c lvl {1} \c3+{2}\c xp;".FormatWith(SkillsDif[i].Name, PlayerNew.Skills[SkillsDif[i].Name].Level + DifLevel, SkillsDif[i].Exp.ToShortString(1));
                    }
                }
                await bc.SendReply(ReplyMsg);
            }
        }

        public static async Task Record(CommandContext bc)
        {
            string rsn = await bc.GetPlayerName(bc.From.Nickname);
            string skill = Skill.OVER;

            if (bc.MessageTokens.Length > 1)
            {
                if (Skill.TryParse(bc.MessageTokens[1], ref skill))
                {
                    if (bc.MessageTokens.Length > 2)
                    {
                        rsn = await bc.GetPlayerName(bc.MessageTokens.Join(2));
                    }
                }
                else
                {
                    rsn = await bc.GetPlayerName(bc.MessageTokens.Join(1));
                }
            }

            try
            {
                string recordPage = new WebClient().DownloadString("http://runetracker.org/track-" + rsn + "," + Skill.NameToId(skill) + ",0");

                string recordRegex = @"Exp Gain Records[^:]+:<\/b><br \/>\s+";
                recordRegex += @"Day: <i>(?:<acronym title=""([^""]+)"">|)([^<]+)(?:<\/acronym>|)<\/i><br \/>\s+";
                recordRegex += @"Week: <i>(?:<acronym title=""([^""]+)"">|)([^<]+)(?:<\/acronym>|)<\/i><br \/>\s+";
                recordRegex += @"Month: <i>(?:<acronym title=""([^""]+)"">|)([^<]+)(?:<\/acronym>|)<\/i><br \/>";

                Match M = Regex.Match(recordPage, recordRegex, RegexOptions.Singleline);
                if (M.Success)
                {
                    await bc.SendReply(@"{0}'s records in {1}: Day \c07{2}\c ({3}); Week \c07{4}\c ({5}); Month \c07{6}\c ({7}); \c12http://runetracker.org/track-{0},{8},0\c", rsn, skill, M.Groups[2], string.IsNullOrEmpty(M.Groups[1].Value) ? "N/A" : M.Groups[1].Value, M.Groups[4], string.IsNullOrEmpty(M.Groups[3].Value) ? "N/A" : M.Groups[3].Value, M.Groups[6], string.IsNullOrEmpty(M.Groups[5].Value) ? "N/A" : M.Groups[5].Value, Skill.NameToId(skill));
                }
                else
                {
                    await bc.SendReply(@"rscript has no records in {0} for {1}.", skill, rsn);
                }
                return;
            }
            catch
            {
            }
            await bc.SendReply("rscript data source appears to be unreachable at the moment.");
        }
    }
}
