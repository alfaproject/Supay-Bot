using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task ClanTop(CommandContext bc)
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

            string rsn = bc.GetPlayerName(bc.From.Nickname);
            string skill = null,
                   activity = null;
            int rank = 0;
            bool IsIndividual = false;

            // get @exp
            bool exp = false;
            if (bc.Message.Contains(" @exp") || bc.Message.Contains(" @xp"))
            {
                exp = true;
                bc.Message = bc.Message.Replace(" @exp", string.Empty);
                bc.Message = bc.Message.Replace(" @xp", string.Empty);
            }

            // Create a list of Clan players
            List<Player> clanPlayers = new Players(clanInitials);

            // Parse command arguments
            if (bc.MessageTokens.Length == 1)
            {
                // !ClanTop
                IsIndividual = true;
            }
            else if (Bot.Activity.TryParse(bc.MessageTokens[1], ref activity) || Skill.TryParse(bc.MessageTokens[1], ref skill))
            {
                // !ClanTop Skill/Activity
                rank = 1;

                // Clean and sort clan members by specified skill
                if (activity == null)
                {
                    var players = clanPlayers.Where(p => p.Skills[skill].Exp > 0);
                    players = exp
                        ? players.OrderByDescending(p => p.Skills[skill].Exp).ThenBy(p => p.Skills[skill].Rank)
                        : players.OrderBy(p => p.Skills[skill]);
                    clanPlayers = players.ToList();
                }
                else
                {
                    clanPlayers = clanPlayers.Where(p => p.Activities[activity].Score > 0).OrderBy(p => p.Activities[activity]).ToList();
                }

                if (bc.MessageTokens.Length > 2)
                {
                    if (int.TryParse(bc.MessageTokens[2], out rank))
                    {
                        // !ClanTop Skill/Activity Rank
                    }
                    else if (bc.MessageTokens.Length == 3 && bc.MessageTokens[2].ToUpperInvariant() == "@LAST")
                    {
                        // !ClanTop Skill/Activity @last
                        rank = clanPlayers.Count;
                    }
                    else
                    {
                        // !ClanTop Skill/Activity RSN
                        rsn = bc.GetPlayerName(bc.MessageTokens.Join(2));
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
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
                IsIndividual = true;
            }

            if (IsIndividual)
            {
                var player = clanPlayers.FirstOrDefault(p => p.Name.EqualsI(rsn));
                if (player != null)
                {
                    // individual skill ranks
                    string reply = @"[{0}] \b{1}\b skill ranks:".FormatWith(clanInitials, rsn);
                    foreach (var s in player.Skills.Values.Where(s => s.Exp > 0))
                    {
                        var indexOfPlayer = clanPlayers.OrderBy(p => p.Skills[s.Name]).IndexOf(player);
                        reply += @" \c7#{0}\c {1};".FormatWith(indexOfPlayer + 1, s.ShortName);
                    }
                    await bc.SendReply(reply);

                    // individual activity ranks
                    bool ranked = false;
                    reply = @"[{0}] \b{1}\b activity ranks:".FormatWith(clanInitials, rsn);
                    foreach (var a in player.Activities.Values.Where(a => a.Score > 0))
                    {
                        ranked = true;

                        var indexOfPlayer = clanPlayers.OrderBy(p => p.Activities[a.Name]).IndexOf(player);
                        reply += @" \c7#{0}\c {1};".FormatWith(indexOfPlayer + 1, a.Name);
                    }
                    if (ranked)
                    {
                        await bc.SendReply(reply);
                    }
                }
                else
                {
                    await bc.SendReply(@"\b{0}\b isn't at {1}.", rsn, clanName);
                }
                return;
            }
            
            // Get input player rank
            var inputPlayerRank = clanPlayers.FindIndex(p => p.Name.EqualsI(bc.GetPlayerName(bc.From.Nickname))) + 1;

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

            var skillFormat = exp ? "e" : "l";

            if (activity == null)
            {
                if (clanPlayers.Count > 0)
                {
                    var reply = @"[{0}] \u{1}\u ranking:".FormatWith(clanInitials, skill.ToLowerInvariant());
                    if (inputPlayerRank > 0 && inputPlayerRank <= MinRank)
                    {
                        reply += @" \c7#{0}\c \u{1}\u ({2});".FormatWith(inputPlayerRank, clanPlayers[inputPlayerRank - 1].Name, clanPlayers[inputPlayerRank - 1].Skills[skill].ToStringI(skillFormat));
                    }

                    for (int i = MinRank; i < Math.Min(MinRank + 11, clanPlayers.Count); i++)
                    {
                        reply += " ";
                        if (i == rank - 1)
                        {
                            reply += @"\b";
                        }
                        reply += @"\c7#{0}\c ".FormatWith(i + 1);
                        if (i == inputPlayerRank - 1)
                        {
                            reply += @"\u";
                        }
                        reply += clanPlayers[i].Name;
                        if (i == inputPlayerRank - 1)
                        {
                            reply += @"\u";
                        }

                        reply += " (" + clanPlayers[i].Skills[skill].ToStringI(skillFormat) + ")";

                        if (i == rank - 1)
                        {
                            reply += @"\b";
                        }
                        reply += ";";
                    }

                    if (inputPlayerRank > 0 && inputPlayerRank > MinRank + 11)
                    {
                        reply += @" \c7#{0}\c \u{1}\u ({2:e});".FormatWith(inputPlayerRank, clanPlayers[inputPlayerRank - 1].Name, clanPlayers[inputPlayerRank - 1].Skills[skill].ToStringI(skillFormat));
                    }

                    await bc.SendReply(reply);
                }
                else
                {
                    await bc.SendReply(clanName + " don't have any member ranked at this skill.");
                }
            }
            else
            {
                if (clanPlayers.Count > 0)
                {
                    var reply = @"[{0}] \u{1}\u ranking:".FormatWith(clanInitials, activity.ToLowerInvariant());
                    if (inputPlayerRank > 0 && inputPlayerRank <= MinRank)
                    {
                        reply += @" \c7#{0}\c \u{1}\u ({2});".FormatWith(inputPlayerRank, clanPlayers[inputPlayerRank - 1].Name, clanPlayers[inputPlayerRank - 1].Activities[activity].Score);
                    }

                    for (int i = MinRank; i < Math.Min(MinRank + 11, clanPlayers.Count); i++)
                    {
                        reply += " ";
                        if (i == rank - 1)
                        {
                            reply += @"\b";
                        }
                        reply += @"\c7#{0}\c ".FormatWith(i + 1);
                        if (i == inputPlayerRank - 1)
                        {
                            reply += @"\u";
                        }
                        reply += clanPlayers[i].Name;
                        if (i == inputPlayerRank - 1)
                        {
                            reply += @"\u";
                        }
                        reply += " (" + clanPlayers[i].Activities[activity].Score + ")";
                        if (i == rank - 1)
                        {
                            reply += @"\b";
                        }
                        reply += ";";
                    }

                    if (inputPlayerRank > 0 && inputPlayerRank > MinRank + 11)
                    {
                        reply += @" \c07#" + inputPlayerRank + @"\c \u" + clanPlayers[inputPlayerRank - 1].Name + @"\u (" + clanPlayers[inputPlayerRank - 1].Activities[activity].Score + ");";
                    }

                    await bc.SendReply(reply);
                }
                else
                {
                    await bc.SendReply(clanName + " don't have any member ranked at this activity.");
                }
            }
        }
    }
}
