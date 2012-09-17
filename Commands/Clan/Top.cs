using System;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static void ClanTop(CommandContext bc)
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
            var clanPlayers = new Players(clanInitials);

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
                    clanPlayers.RemoveAll(p => !p.Ranked || p.Skills[skill].Exp == 0);
                    clanPlayers.SortBySkill(skill, exp);
                }
                else
                {
                    clanPlayers.RemoveAll(p => !p.Ranked || p.Activities[activity].Score == 0);
                    clanPlayers.SortByActivity(activity);
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
                        if (clanPlayers.Contains(rsn))
                        {
                            rank = clanPlayers.IndexOf(rsn) + 1;
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
                Player p = clanPlayers.Find(rsn);
                if (p != null)
                {
                    // individual skill ranks
                    string reply = "[{0}] \\b{1}\\b skill ranks:".FormatWith(clanInitials, rsn);
                    foreach (Skill s in p.Skills.Values)
                    {
                        if (s.Exp > 0)
                        {
                            clanPlayers.SortBySkill(s.Name, false);
                            reply += " \\c07#" + (clanPlayers.IndexOf(p) + 1) + "\\c " + s.ShortName + ";";
                        }
                    }
                    bc.SendReply(reply);

                    // individual activity ranks
                    bool ranked = false;
                    reply = "[{0}] \\b{1}\\b activity ranks:".FormatWith(clanInitials, rsn);
                    foreach (Activity mg in p.Activities.Values)
                    {
                        if (mg.Score > 0)
                        {
                            ranked = true;
                            clanPlayers.SortByActivity(mg.Name);
                            reply += " \\c07#" + (clanPlayers.IndexOf(p) + 1) + "\\c " + mg.Name + ";";
                        }
                    }
                    if (ranked)
                    {
                        bc.SendReply(reply);
                    }
                }
                else
                {
                    bc.SendReply("\\b{0}\\b isn't at {1}.".FormatWith(rsn, clanName));
                }
            }
            else
            {
                // Get input player rank
                int input_player_rank = 0;
                if (clanPlayers.Contains(bc.GetPlayerName(bc.From.Nickname)))
                {
                    input_player_rank = clanPlayers.IndexOf(bc.GetPlayerName(bc.From.Nickname)) + 1;
                }

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

                string skillFormat = "l";
                if (exp)
                {
                    skillFormat = "e";
                }

                if (activity == null)
                {
                    if (clanPlayers.Count > 0)
                    {
                        string reply = "[" + clanInitials + "] \\u" + skill.ToLowerInvariant() + "\\u ranking:";
                        if (input_player_rank > 0 && input_player_rank <= MinRank)
                        {
                            reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Skills[skill].ToStringI(skillFormat) + ");";
                        }

                        for (int i = MinRank; i < Math.Min(MinRank + 11, clanPlayers.Count); i++)
                        {
                            reply += " ";
                            if (i == rank - 1)
                            {
                                reply += "\\b";
                            }
                            reply += "\\c07#" + (i + 1) + "\\c ";
                            if (i == input_player_rank - 1)
                            {
                                reply += "\\u";
                            }
                            reply += clanPlayers[i].Name;
                            if (i == input_player_rank - 1)
                            {
                                reply += "\\u";
                            }

                            reply += " (" + clanPlayers[i].Skills[skill].ToStringI(skillFormat) + ")";

                            if (i == rank - 1)
                            {
                                reply += "\\b";
                            }
                            reply += ";";
                        }

                        if (input_player_rank > 0 && input_player_rank > MinRank + 11)
                        {
                            reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Skills[skill].ToStringI(skillFormat) + ");";
                        }

                        bc.SendReply(reply);
                    }
                    else
                    {
                        bc.SendReply(clanName + " don't have any member ranked at this skill.");
                    }
                }
                else
                {
                    if (clanPlayers.Count > 0)
                    {
                        string reply = "[" + clanInitials + "] \\u" + activity.ToLowerInvariant() + "\\u ranking:";
                        if (input_player_rank > 0 && input_player_rank <= MinRank)
                        {
                            reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Activities[activity].Score + ");";
                        }

                        for (int i = MinRank; i < Math.Min(MinRank + 11, clanPlayers.Count); i++)
                        {
                            reply += " ";
                            if (i == rank - 1)
                            {
                                reply += "\\b";
                            }
                            reply += "\\c07#" + (i + 1) + "\\c ";
                            if (i == input_player_rank - 1)
                            {
                                reply += "\\u";
                            }
                            reply += clanPlayers[i].Name;
                            if (i == input_player_rank - 1)
                            {
                                reply += "\\u";
                            }
                            reply += " (" + clanPlayers[i].Activities[activity].Score + ")";
                            if (i == rank - 1)
                            {
                                reply += "\\b";
                            }
                            reply += ";";
                        }

                        if (input_player_rank > 0 && input_player_rank > MinRank + 11)
                        {
                            reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Activities[activity].Score + ");";
                        }

                        bc.SendReply(reply);
                    }
                    else
                    {
                        bc.SendReply(clanName + " don't have any member ranked at this activity.");
                    }
                }
            }
        }
    }
}
