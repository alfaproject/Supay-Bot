using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task ClanPerformance(CommandContext bc)
        {
            string clanInitials;
            string clanName;
            if (bc.Message.ContainsI("SS"))
            {
                clanInitials = "SS";
                clanName = "Supreme Skillers";
            }
            else if (bc.Message.ContainsI("TS"))
            {
                clanInitials = "TS";
                clanName = "True Skillers";
            }
            else
            {
                clanInitials = "PT";
                clanName = "Portugal";
            }

            var rsn = await bc.GetPlayerName(bc.From.Nickname);
            string skill = null;
            int rank = 0;
            bool IsIndividual = false;

            // get last updated player date
            DateTime lastUpdate = (await Database.Lookup<string>("lastUpdate", "players", "ORDER BY lastUpdate DESC")).ToDateTime();

            DateTime firstDay,
                     lastDay;
            if (bc.MessageTokens[0].Contains("yesterday") || bc.MessageTokens[0].Contains("yday"))
            {
                lastDay = lastUpdate;
                firstDay = lastDay.AddDays(-1);
            }
            else if (bc.MessageTokens[0].Contains("lastweek") | bc.MessageTokens[0].Contains("lweek"))
            {
                lastDay = lastUpdate.AddDays(-((int) lastUpdate.DayOfWeek));
                firstDay = lastDay.AddDays(-7);
            }
            else if (bc.MessageTokens[0].Contains("lastmonth") | bc.MessageTokens[0].Contains("lmonth"))
            {
                lastDay = lastUpdate.AddDays(1 - lastUpdate.Day);
                firstDay = lastDay.AddMonths(-1);
            }
            else if (bc.MessageTokens[0].Contains("lastyear") | bc.MessageTokens[0].Contains("lyear"))
            {
                lastDay = lastUpdate.AddDays(1 - lastUpdate.DayOfYear);
                firstDay = lastDay.AddYears(-1);
            }
            else if (bc.MessageTokens[0].Contains("week"))
            {
                firstDay = lastUpdate.AddDays(-((int) lastUpdate.DayOfWeek));
                lastDay = lastUpdate;
            }
            else if (bc.MessageTokens[0].Contains("month"))
            {
                firstDay = lastUpdate.AddDays(1 - lastUpdate.Day);
                lastDay = lastUpdate;
            }
            else if (bc.MessageTokens[0].Contains("year"))
            {
                firstDay = lastUpdate.AddDays(1 - lastUpdate.DayOfYear);
                lastDay = lastUpdate;
            }
            else
            {
                Match M = Regex.Match(bc.MessageTokens[0], "last(\\d+)days");
                if (M.Success)
                {
                    lastDay = lastUpdate;
                    firstDay = lastDay.AddDays(-int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture));
                }
                else
                {
                    return;
                }
            }
            if (firstDay == lastDay)
            {
                return;
            }

            // Create a list of Clan players
            var clanPlayers = await Players.FromClanAsPeriod(clanInitials, firstDay, lastDay);

            // Parse command arguments
            if (bc.MessageTokens.Length == 1)
            {
                // !ClanTop
                IsIndividual = true;
            }
            else if (Skill.TryParse(bc.MessageTokens[1], ref skill))
            {
                // !ClanTop Skill
                rank = 1;

                // Clean and sort clan members by specified skill
                clanPlayers = clanPlayers.Where(p => p.Skills[skill].Exp > 0)
                                         .OrderByDescending(p => p.Skills[skill].Exp)
                                         .ToList();

                if (bc.MessageTokens.Length > 2)
                {
                    if (int.TryParse(bc.MessageTokens[2], out rank))
                    {
                        // !ClanTop Skill Rank
                    }
                    else if (bc.MessageTokens.Length == 3 && bc.MessageTokens[2].ToUpperInvariant() == "@LAST")
                    {
                        // !ClanTop Skill @last
                        rank = clanPlayers.Count;
                    }
                    else
                    {
                        // !ClanTop Skill
                        rsn = await bc.GetPlayerName(bc.MessageTokens.Join(1));
                        var indexOfPlayer = clanPlayers.FindIndex(p => p.Name.EqualsI(rsn));
                        if (indexOfPlayer != -1)
                        {
                            rank = indexOfPlayer + 1;
                        }
                    }
                }
            }
            else
            {
                // !ClanTop RSN
                rsn = await bc.GetPlayerName(bc.MessageTokens.Join(1));
                IsIndividual = true;
            }

            if (IsIndividual)
            {
                var player = clanPlayers.FirstOrDefault(p => p.Name.EqualsI(rsn));
                if (player != null)
                {
                    // individual skill ranks
                    var reply = @"[{0}] \b{1}\b skill ranks:";
                    foreach (var s in player.Skills.Values.Where(s => s.Exp > 0))
                    {
                        var indexOfPlayer = clanPlayers.OrderBy(p => p.Skills[s.Name]).IndexOf(player);
                        reply += @" \c07#{0}\c {1};".FormatWith(indexOfPlayer + 1, s.ShortName);
                    }
                    await bc.SendReply(reply, clanInitials, rsn);
                }
                else
                {
                    await bc.SendReply(@"\b{0}\b wasn't at {1}.", rsn, clanName);
                }
                return;
            }
            
            // Get input player rank
            var inputPlayerName = await bc.GetPlayerName(bc.From.Nickname);
            var inputPlayerRank = clanPlayers.FindIndex(p => p.Name.EqualsI(inputPlayerName)) + 1;

            // fix rank
            if (rank < 1)
            {
                rank = 1;
            }
            else if (rank > clanPlayers.Count)
            {
                rank = clanPlayers.Count;
            }

            int MinRank = rank - 6;
            if (MinRank < 0)
            {
                MinRank = 0;
            }
            else if (MinRank > clanPlayers.Count - 11)
            {
                MinRank = clanPlayers.Count - 11;
            }

            if (clanPlayers.Count > 0)
            {
                string reply = @"[{0}] \u{1}\u ranking:".FormatWith(clanInitials, skill.ToLowerInvariant());
                if (inputPlayerRank > 0 && inputPlayerRank <= MinRank)
                {
                    reply += @" \c7#{0}\c \u{1}\u ({2:e});".FormatWith(inputPlayerRank, clanPlayers[inputPlayerRank - 1].Name, clanPlayers[inputPlayerRank - 1].Skills[skill]);
                }

                for (int i = MinRank; i < Math.Min(MinRank + 11, clanPlayers.Count); i++)
                {
                    reply += " ";
                    if (i == rank - 1)
                    {
                        reply += @"\b";
                    }
                    reply += @"\c07#{0}\c ".FormatWith(i + 1);
                    if (i == inputPlayerRank - 1)
                    {
                        reply += @"\u";
                    }
                    reply += clanPlayers[i].Name;
                    if (i == inputPlayerRank - 1)
                    {
                        reply += @"\u";
                    }
                    reply += " (" + clanPlayers[i].Skills[skill].ToStringI("e") + ")";
                    if (i == rank - 1)
                    {
                        reply += @"\b";
                    }
                    reply += ";";
                }

                if (inputPlayerRank > 0 && inputPlayerRank > MinRank + 11)
                {
                    reply += @" \c7#{0}\c \u{1}\u ({2:e});".FormatWith(inputPlayerRank, clanPlayers[inputPlayerRank - 1].Name, clanPlayers[inputPlayerRank - 1].Skills[skill]);
                }

                await bc.SendReply(reply);
            }
            else
            {
                await bc.SendReply(clanName + " didn't have any member ranked at this skill.");
            }
        }
    }
}
