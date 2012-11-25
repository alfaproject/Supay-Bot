using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static class CmdOthers
    {
        public static async Task Percent(CommandContext bc)
        {
            // get rsn
            string rsn = bc.GetPlayerName(bc.MessageTokens.Length > 1 ? bc.MessageTokens.Join(1) : bc.From.Nickname);

            var p = new Player(rsn);
            if (!p.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }

            long totalExp = p.Skills[Skill.OVER].Exp;
            long combatExp = p.Skills[Skill.COMB].Exp;
            long f2pExp = p.Skills.F2pExp;

            // slayer
            long hits_exp_gained = p.Skills[Skill.HITP].Exp - 1154;
            var expected_max_slayer_exp = (int) (hits_exp_gained * 3.0 / 4.0);

            // pc
            long expected_combat_xp = p.Skills[Skill.HITP].Exp + p.Skills[Skill.HITP].Exp * 12 / 4;
            long current_combat_xp = p.Skills[Skill.HITP].Exp + p.Skills[Skill.ATTA].Exp + p.Skills[Skill.STRE].Exp + p.Skills[Skill.DEFE].Exp + p.Skills[Skill.RANG].Exp;

            bc.SendReply(@"\b{0}\b statistic percentages | Total exp: \c07{1:N0}\c | Combat exp: \c07{2:N0}\c (\c07{3:0.##}%\c) | F2P exp: \c07{4:N0}\c (\c07{5:0.##}%\c) | Slayer%: \c07{6:0.##}% - {7:0.##}%\c | PestControl%: \c07{8:0.##}%\c", rsn, totalExp, combatExp, (double) combatExp / totalExp * 100, f2pExp, (double) f2pExp / totalExp * 100, (double) p.Skills[Skill.SLAY].Exp / expected_max_slayer_exp * 100, (double) p.Skills[Skill.SLAY].Exp / (expected_max_slayer_exp - (hits_exp_gained / 133)) * 100, (double) (current_combat_xp - expected_combat_xp) / current_combat_xp * 100);
        }

        public static async Task CombatPercent(CommandContext bc)
        {
            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
            }

            var p = new Player(rsn);
            if (!p.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }

            bc.SendReply(@"\b{0}\b is \c07{1:0.##}%\c combat based, with \c07{2:e}\c combat based exp. and \c07{3:e}\c total exp.", rsn, (double) p.Skills[Skill.COMB].Exp / (double) p.Skills[Skill.OVER].Exp * 100.0, p.Skills[Skill.COMB], p.Skills[Skill.OVER]);
        }

        public static async Task F2pPercent(CommandContext bc)
        {
            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
            }

            var p = new Player(rsn);
            if (!p.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }

            bc.SendReply(@"\b{0}\b is \c07{1:0.##}%\c f2p based, with \c07{2:N0}\c f2p based exp. and \c07{3:e}\c total exp.", rsn, (double) p.Skills.F2pExp / (double) p.Skills[Skill.OVER].Exp * 100, p.Skills.F2pExp, p.Skills[Skill.OVER]);
        }

        public static async Task SlayerPercent(CommandContext bc)
        {
            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
            }

            var p = new Player(rsn);
            if (!p.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }

            long hits_exp_gained = p.Skills[Skill.HITP].Exp - 1154;
            double expected_max_slayer_exp = hits_exp_gained * 3.0 / 4.0;

            bc.SendReply(@"\b{0}\b \c07{1:0.##}% - {2:0.##}%\c of combat exp. is slayer based, with \c07{3:N0}\c combat slayer exp. and \c07{4:N0}\c combat total exp. (This percentage isn't accurate, mostly because of monster hp regeneration ratio and cannon slayering.)", rsn, (double) p.Skills[Skill.SLAY].Exp / expected_max_slayer_exp * 100.0, (double) p.Skills[Skill.SLAY].Exp / (expected_max_slayer_exp - ((double) hits_exp_gained / 133.0)) * 100.0, (double) p.Skills[Skill.SLAY].Exp * 16.0 / 3.0, hits_exp_gained + hits_exp_gained * 3);
        }

        public static async Task PcPercent(CommandContext bc)
        {
            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
            }

            var p = new Player(rsn);
            if (!p.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }

            long expected_combat_xp = p.Skills[Skill.HITP].Exp + p.Skills[Skill.HITP].Exp * 12 / 4;
            long current_combat_xp = p.Skills[Skill.HITP].Exp + p.Skills[Skill.ATTA].Exp + p.Skills[Skill.STRE].Exp + p.Skills[Skill.DEFE].Exp + p.Skills[Skill.RANG].Exp;

            bc.SendReply(@"\b{0}\b \c07{1:0.##}%\c of combat exp. was pest controled and/or cannoned, with \c07{2:N0}\c normal combat exp. and \c07{3:N0}\c total combat exp. (This percentage might not be accurate; magic isn't included in calculations.)", rsn, (double) (current_combat_xp - expected_combat_xp) / (double) current_combat_xp * 100.0, expected_combat_xp, current_combat_xp);
        }

        public static async Task Players(CommandContext bc)
        {
            using (var wc = new WebClient())
            {
                var rsPage = wc.DownloadString("http://www.runescape.com/index.ws");

                var playersMatch = Regex.Match(rsPage, @"([\d,]+)</span> people");
                bc.SendReply(
                    playersMatch.Success
                        ? @"There are currently \c07{0}\c players online.".FormatWith(playersMatch.Groups[1].Value)
                        : @"Error: No players were found."
                    );
            }
        }

        public static async Task Grats(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !grats <skill> <level>");
                return;
            }

            // get rsn
            string rsn = bc.GetPlayerName(bc.From.Nickname);

            string skill = null;
            int level;
            if (int.TryParse(bc.MessageTokens[1], out level))
            {
                if (bc.MessageTokens.Length > 2 && Skill.TryParse(bc.MessageTokens[2], ref skill))
                {
                    // !grats <level> <skill>
                }
                else
                {
                    // !grats <level>
                    bc.SendReply("Syntax: !grats <skill> <level>");
                    return;
                }
            }
            else if (Skill.TryParse(bc.MessageTokens[1], ref skill))
            {
                if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[2], out level))
                {
                    // !grats <skill> <level>
                }
                else
                {
                    // !grats <skill>
                    var p = new Player(rsn);
                    if (p.Ranked)
                    {
                        level = p.Skills[skill].VLevel;
                    }
                    else
                    {
                        level = 0;
                    }
                }
            }
            else
            {
                bc.SendReply("Syntax: !grats <skill> <level>");
                return;
            }

            if (skill == Skill.COMB)
            {
                level = Math.Min(138, level);
            }
            else if (skill != Skill.OVER)
            {
                level = Math.Min(126, level);
            }

            bc.ReplyNotice = false;
            if (level > 0)
            {
                bc.SendReply(@":D\-< ¤.¡*°*¡.¤ Woo! Congrats on your \c07{0} level {1} {2}\c!! ¤.¡*°*¡.¤ :D/-<", skill.ToLowerInvariant(), level, rsn);
            }
            else
            {
                bc.SendReply(@":D\-< ¤.¡*°*¡.¤ Woo! Congrats on your \c07{0} level {1}\c!! ¤.¡*°*¡.¤ :D/-<", skill.ToLowerInvariant(), rsn);
            }
        }

        public static async Task HighLow(CommandContext bc)
        {
            // @rank
            bool rank = false;
            if (bc.Message.Contains(" @rank") || bc.Message.Contains(" @r"))
            {
                rank = true;
                bc.Message = bc.Message.Replace(" @rank", string.Empty);
                bc.Message = bc.Message.Replace(" @r", string.Empty);
            }

            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
            }

            var p = new Player(rsn);
            if (!p.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", p.Name);
                return;
            }

            List<Skill> highest,
                        lowest;
            int i;
            string reply;

            if (rank)
            {
                highest = p.Skills.HighestRanked;
                int highestRank = highest[0].Rank;
                reply = @"\b{0}\b \uhighest\u skills:".FormatWith(rsn);
                for (i = 0; i < highest.Count; i++)
                {
                    if (highest[i].Rank != highestRank)
                    {
                        break;
                    }
                    reply += @" \c07{0}\c,".FormatWith(highest[i].Name);
                }
                reply = reply.Substring(0, reply.Length - 1) + @" level \c07{0:rv}\c ranked \c07#{0:r}\c".FormatWith(highest[0]);
                reply += @" \ufollowed by\u \c07{0:N}\c level \c07{0:rv}\c ranked \c07#{0:r}\c.".FormatWith(highest[i]);
                bc.SendReply(reply);

                lowest = p.Skills.LowestRanked;
                int lowestRank = lowest[0].Rank;
                reply = @"\b{0}\b \ulowest\u skills:".FormatWith(rsn);
                for (i = 0; i < lowest.Count; i++)
                {
                    if (lowest[i].Rank != lowestRank)
                    {
                        break;
                    }
                    reply += @" \c07{0}\c,".FormatWith(lowest[i].Name);
                }
                reply = reply.Substring(0, reply.Length - 1) + @" level \c07{0:rv}\c ranked \c07#{0:r}\c".FormatWith(lowest[0]);
                reply += @" \ufollowed by\u \c07{0:N}\c level \c07{0:rv}\c ranked \c07#{0:r}\c.".FormatWith(lowest[i]);
                bc.SendReply(reply);
            }
            else
            {
                highest = p.Skills.Highest;
                long highestExp = highest[0].Exp;
                reply = @"\b{0}\b \uhighest\u skills:".FormatWith(rsn);
                for (i = 0; i < highest.Count; i++)
                {
                    if (highest[i].Exp != highestExp)
                    {
                        break;
                    }
                    reply += @" \c07{0}\c,".FormatWith(highest[i].Name);
                }
                reply = reply.Substring(0, reply.Length - 1) + @" level \c07{0:rv}\c with \c07{0:e}\c exp.".FormatWith(highest[0]);
                reply += @" \ufollowed by\u \c07{0:N}\c level \c07{0:rv}\c with \c07{0:e}\c exp.".FormatWith(highest[i]);
                bc.SendReply(reply);

                lowest = p.Skills.Lowest;
                long lowestExp = lowest[0].Exp;
                reply = @"\b{0}\b \ulowest\u skills:".FormatWith(rsn);
                for (i = 0; i < lowest.Count; i++)
                {
                    if (lowest[i].Exp != lowestExp)
                    {
                        break;
                    }
                    reply += @" \c07{0}\c,".FormatWith(lowest[i].Name);
                }
                reply = reply.Substring(0, reply.Length - 1) + @" level \c07{0:rv}\c with \c07{0:e}\c exp.".FormatWith(lowest[0]);
                reply += @" \ufollowed by\u \c07{0:N}\c level \c07{0:rv}\c with \c07{0:e}\c exp.".FormatWith(lowest[i]);
                bc.SendReply(reply);
            }
        }

        public static async Task CalcCombat(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !CalcCombat Att Str Def Hit Pray Ran Mag");
                return;
            }

            int Att,
                Str = 0,
                Def = 0,
                Hit = 0,
                Pray = 0,
                Ran = 0,
                Mag = 0;

            int.TryParse(bc.MessageTokens[1], out Att);
            if (bc.MessageTokens.Length > 2)
            {
                int.TryParse(bc.MessageTokens[2], out Str);
            }
            if (bc.MessageTokens.Length > 3)
            {
                int.TryParse(bc.MessageTokens[3], out Def);
            }
            if (bc.MessageTokens.Length > 4)
            {
                int.TryParse(bc.MessageTokens[4], out Hit);
            }
            if (bc.MessageTokens.Length > 5)
            {
                int.TryParse(bc.MessageTokens[5], out Pray);
            }
            if (bc.MessageTokens.Length > 6)
            {
                int.TryParse(bc.MessageTokens[6], out Ran);
            }
            if (bc.MessageTokens.Length > 7)
            {
                int.TryParse(bc.MessageTokens[7], out Mag);
            }

            var p = new Player(bc.GetPlayerName(bc.From.Nickname));
            if (p.Ranked)
            {
                if (Att <= 0)
                {
                    Att = p.Skills[Skill.ATTA].VLevel;
                }
                if (Str <= 0)
                {
                    Str = p.Skills[Skill.STRE].VLevel;
                }
                if (Def <= 0)
                {
                    Def = p.Skills[Skill.DEFE].VLevel;
                }
                if (Hit <= 0)
                {
                    Hit = p.Skills[Skill.HITP].VLevel;
                }
                if (Pray <= 0)
                {
                    Pray = p.Skills[Skill.PRAY].VLevel;
                }
                if (Ran <= 0)
                {
                    Ran = p.Skills[Skill.RANG].VLevel;
                }
                if (Mag <= 0)
                {
                    Mag = p.Skills[Skill.MAGI].VLevel;
                }
            }
            else
            {
                if (Att <= 0)
                {
                    Att = 1;
                }
                if (Str <= 0)
                {
                    Str = 1;
                }
                if (Def <= 0)
                {
                    Def = 1;
                }
                if (Hit <= 0)
                {
                    Hit = 10;
                }
                if (Pray <= 0)
                {
                    Pray = 1;
                }
                if (Ran <= 0)
                {
                    Ran = 1;
                }
                if (Mag <= 0)
                {
                    Mag = 1;
                }
            }

            string cmbclass = Utils.CombatClass(Att, Str, Ran, Mag);
            int cmblevel = Utils.CalculateCombat(Att, Str, Def, Hit, Ran, Pray, Mag);
            bc.SendReply(@"Combat: \c07{0}\c | Class: \c07{1}\c | Stats: \c07{2} {3} {4}\c {5} \c07{6}\c {7} {8}", cmblevel, cmbclass, Att, Str, Def, Hit, Pray, Ran, Mag);

            int nextAS = Utils.NextCombatAttStr(Att, Str, Def, Hit, Ran, Pray, Mag);
            int nextDH = Utils.NextCombatDefHp(Att, Str, Def, Hit, Ran, Pray, Mag);
            int nextP = Utils.NextCombatPray(Att, Str, Def, Hit, Ran, Pray, Mag);
            int nextR = Utils.NextCombatRan(Att, Str, Def, Hit, Ran, Pray, Mag);
            int nextM = Utils.NextCombatMag(Att, Str, Def, Hit, Ran, Pray, Mag);
            bc.SendReply(@"Stats to level | Att/Str: \c{0}\c | Def/Hp: \c{1}\c | Pray: \c{2}\c | Range: \c{3}\c | Mage: \c{4}\c", (Att + nextAS > 99 && Str + nextAS > 99 ? "04" : "03") + nextAS, (Def + nextDH > 99 && Hit + nextDH > 99 ? "04" : "03") + nextDH, (Pray + nextP > 99 ? "04" : "03") + nextP, (Ran + nextR > 99 ? "04" : "03") + nextR, (Mag + nextM > 99 ? "04" : "03") + nextM);
        }
    }
}
