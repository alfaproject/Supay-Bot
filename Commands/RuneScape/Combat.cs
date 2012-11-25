using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Combat(CommandContext bc)
        {
            // get @next
            var nextMatch = _nextRegex.Match(bc.Message);
            if (nextMatch.Success)
            {
                bc.Message = bc.Message.Replace(nextMatch.Value, string.Empty);
            }

            // get @experience
            var expMatch = _expRegex.Match(bc.Message);
            if (expMatch.Success)
            {
                bc.Message = bc.Message.Replace(expMatch.Value, string.Empty);
            }

            // get @rank
            var rankMatch = _rankRegex.Match(bc.Message);
            if (rankMatch.Success)
            {
                bc.Message = bc.Message.Replace(rankMatch.Value, string.Empty);
            }

            // get @virtuallevel
            var virtualMatch = _virtualRegex.Match(bc.Message);
            if (virtualMatch.Success)
            {
                bc.Message = bc.Message.Replace(virtualMatch.Value, string.Empty);
            }

            // get player
            var player = new Player(bc.GetPlayerName(bc.MessageTokens.Length == 1 ? bc.From.Nickname : bc.MessageTokens.Join(1)));
            if (!player.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", player.Name);
                return;
            }

            var playerSkills = player.Skills.Values.Where(s => s.Name != Skill.OVER && s.Name != Skill.COMB).ToList();
            var averageExp = player.Skills[Skill.OVER].Exp / playerSkills.Count;
            var avgExpThreshold = averageExp / 5;
            var highestLevelExp = playerSkills.Max(s => s.Exp);

            var expectedMaxSlayerExp = (int) ((player.Skills[Skill.HITP].Exp - 1154) * 3 / 4.0);

            int combatLevel,
                combatF2pLevel;
            string combatClass;
            if (virtualMatch.Success)
            {
                combatClass = Utils.CombatClass(player.Skills, true);
                combatLevel = Utils.CalculateCombat(player.Skills, true, false);
                combatF2pLevel = Utils.CalculateCombat(player.Skills, true, true);
            }
            else
            {
                combatClass = Utils.CombatClass(player.Skills, false);
                combatLevel = Utils.CalculateCombat(player.Skills, false, false);
                combatF2pLevel = Utils.CalculateCombat(player.Skills, false, true);
            }

            var reply = new StringBuilder(512)
                .AppendFormat(@"\b{0}\b \c07combat\c | level: \c07{1}\c (f2p: \c07{2}\c) | exp: \c07{3:e}\c | combat%: \c07{4:0.##}%\c | slayer%: \c07{5:0.##}%\c | class: \c07{6}\c", player.Name, combatLevel, combatF2pLevel, player.Skills[Skill.COMB], (double) player.Skills[Skill.COMB].Exp / (double) player.Skills[Skill.OVER].Exp * 100.0, (double) player.Skills[Skill.SLAY].Exp / (double) expectedMaxSlayerExp * 100.0, combatClass);

            // Add up SS rank if applicable
            var ssPlayers = new Players("SS").OrderBy(p => p.Skills[Skill.COMB]);
            var indexOfPlayer = ssPlayers.FindIndex(p => p.Name == player.Name);
            if (indexOfPlayer != -1)
            {
                reply.AppendFormat(@" (SS rank: \c07{0}\c)", indexOfPlayer + 1);
            }

            bc.SendReply(reply);

            var format = @" {2}\c{1:00}{0:r";
            if (expMatch.Success)
            {
                format += 'e';
            }
            else if (virtualMatch.Success)
            {
                format += 'v';
            }
            else if (!rankMatch.Success)
            {
                format += 'l';
            }
            format += @"}{3}\c {0:n}{2};";

            reply = new StringBuilder(rankMatch.Success ? @"\uSkills\u:" : @"\uSkills (to level)\u:", 512);

            foreach (var s in player.Skills.Values.Where(s => s.IsCombat))
            {
                var next = 0;
                if (!rankMatch.Success)
                {
                    switch (s.Name)
                    {
                        case Skill.ATTA:
                        case Skill.STRE:
                            next = virtualMatch.Success
                                ? Utils.NextCombatAttStr(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.HITP].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.PRAY].VLevel, player.Skills[Skill.MAGI].VLevel, player.Skills[Skill.SUMM].VLevel)
                                : Utils.NextCombatAttStr(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.HITP].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.PRAY].Level, player.Skills[Skill.MAGI].Level, player.Skills[Skill.SUMM].Level);
                            break;
                        case Skill.DEFE:
                        case Skill.HITP:
                            next = virtualMatch.Success
                                ? Utils.NextCombatDefHp(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.HITP].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.PRAY].VLevel, player.Skills[Skill.MAGI].VLevel, player.Skills[Skill.SUMM].VLevel)
                                : Utils.NextCombatDefHp(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.HITP].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.PRAY].Level, player.Skills[Skill.MAGI].Level, player.Skills[Skill.SUMM].Level);
                            break;
                        case Skill.PRAY:
                            next = virtualMatch.Success
                                ? Utils.NextCombatPray(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.HITP].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.PRAY].VLevel, player.Skills[Skill.MAGI].VLevel, player.Skills[Skill.SUMM].VLevel)
                                : Utils.NextCombatPray(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.HITP].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.PRAY].Level, player.Skills[Skill.MAGI].Level, player.Skills[Skill.SUMM].Level);
                            break;
                        case Skill.SUMM:
                            next = virtualMatch.Success
                                ? Utils.NextCombatSum(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.HITP].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.PRAY].VLevel, player.Skills[Skill.MAGI].VLevel, player.Skills[Skill.SUMM].VLevel)
                                : Utils.NextCombatSum(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.HITP].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.PRAY].Level, player.Skills[Skill.MAGI].Level, player.Skills[Skill.SUMM].Level);
                            break;
                        case Skill.MAGI:
                            next = virtualMatch.Success
                                ? Utils.NextCombatMag(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.HITP].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.PRAY].VLevel, player.Skills[Skill.MAGI].VLevel, player.Skills[Skill.SUMM].VLevel)
                                : Utils.NextCombatMag(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.HITP].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.PRAY].Level, player.Skills[Skill.MAGI].Level, player.Skills[Skill.SUMM].Level);
                            break;
                        case Skill.RANG:
                            next = virtualMatch.Success
                                ? Utils.NextCombatRan(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.HITP].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.PRAY].VLevel, player.Skills[Skill.MAGI].VLevel, player.Skills[Skill.SUMM].VLevel)
                                : Utils.NextCombatRan(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.HITP].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.PRAY].Level, player.Skills[Skill.MAGI].Level, player.Skills[Skill.SUMM].Level);
                            break;
                    }
                }

                reply.AppendFormat(format, s, s.Exp > averageExp + avgExpThreshold ? 3 : (s.Exp < averageExp - avgExpThreshold ? 4 : 7), s.Exp == highestLevelExp ? @"\u" : string.Empty, next > 0 ? "(+" + next + ")" : string.Empty);
            }
            bc.SendReply(reply);

            // Show player performance if applicable
            string dblastupdate = Database.LastUpdate(player.Name);
            if (dblastupdate != null && dblastupdate.Length == 8)
            {
                DateTime lastupdate = dblastupdate.ToDateTime();
                string perf;
                reply = new StringBuilder(512);

                var p_old = new Player(player.Name, lastupdate);
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Today", p_old.Skills[Skill.COMB], player.Skills[Skill.COMB]);
                    if (perf != null)
                    {
                        reply.Append(perf + " | ");
                    }
                }
                p_old = new Player(player.Name, lastupdate.AddDays(-((int) lastupdate.DayOfWeek)));
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Week", p_old.Skills[Skill.COMB], player.Skills[Skill.COMB]);
                    if (perf != null)
                    {
                        reply.Append(perf + " | ");
                    }
                }
                p_old = new Player(player.Name, lastupdate.AddDays(1 - lastupdate.Day));
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Month", p_old.Skills[Skill.COMB], player.Skills[Skill.COMB]);
                    if (perf != null)
                    {
                        reply.Append(perf + " | ");
                    }
                }
                p_old = new Player(player.Name, lastupdate.AddDays(1 - lastupdate.DayOfYear));
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Year", p_old.Skills[Skill.COMB], player.Skills[Skill.COMB]);
                    if (perf != null)
                    {
                        reply.Append(perf + " | ");
                    }
                }
                if (reply.Length > 0)
                {
                    var r = reply.ToString();
                    bc.SendReply(r.EndsWithI(" | ") ? r.Substring(0, r.Length - 3) : r);
                }
            }
        }
    }
}
