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
            var player = await Player.FromHiscores(await bc.GetPlayerName(bc.MessageTokens.Length == 1 ? bc.From.Nickname : bc.MessageTokens.Join(1)));
            if (!player.Ranked)
            {
                await bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", player.Name);
                return;
            }

            var playerSkills = player.Skills.Values.Where(s => s.Name != Skill.OVER && s.Name != Skill.COMB).ToList();
            var averageExp = player.Skills[Skill.OVER].Exp / playerSkills.Count;
            var avgExpThreshold = averageExp / 5;
            var highestLevelExp = playerSkills.Max(s => s.Exp);

            var expectedMaxSlayerExp = (int) ((player.Skills[Skill.HITP].Exp - 1154) * 3 / 4.0);

            var combatClass = Utils.CombatClass(player.Skills, virtualMatch.Success);
            var combatLevel = Utils.CalculateCombat(player.Skills, virtualMatch.Success);

            var reply = new StringBuilder(512)
                .AppendFormat(@"\b{0}\b \c07combat\c | level: \c07{1}\c | exp: \c07{2:e}\c | combat%: \c07{3:0.##}%\c | slayer%: \c07{4:0.##}%\c | class: \c07{5}\c", player.Name, combatLevel, player.Skills[Skill.COMB], (double) player.Skills[Skill.COMB].Exp / (double) player.Skills[Skill.OVER].Exp * 100.0, (double) player.Skills[Skill.SLAY].Exp / (double) expectedMaxSlayerExp * 100.0, combatClass);

            // Add SS rank if applicable
            var ssPlayers = (await Players.FromClan("SS")).OrderBy(p => p.Skills[Skill.COMB]);
            var ssRank = ssPlayers.FindIndex(p => p.Name.EqualsI(player.Name));
            if (ssRank != -1)
            {
                reply.AppendFormat(@" (SS rank: \c07{0}\c)", ssRank + 1);
            }

            await bc.SendReply(reply);

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
                            next = virtualMatch.Success
                                ? Utils.NextCombatAttack(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.MAGI].VLevel)
                                : Utils.NextCombatAttack(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.MAGI].Level);
                            break;
                        case Skill.STRE:
                            next = virtualMatch.Success
                                ? Utils.NextCombatStrength(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.MAGI].VLevel)
                                : Utils.NextCombatStrength(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.MAGI].Level);
                            break;
                        case Skill.DEFE:
                            next = virtualMatch.Success
                                ? Utils.NextCombatDefence(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.MAGI].VLevel)
                                : Utils.NextCombatDefence(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.MAGI].Level);
                            break;
                        case Skill.MAGI:
                            next = virtualMatch.Success
                                ? Utils.NextCombatMagic(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.MAGI].VLevel)
                                : Utils.NextCombatMagic(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.MAGI].Level);
                            break;
                        case Skill.RANG:
                            next = virtualMatch.Success
                                ? Utils.NextCombatRanged(player.Skills[Skill.ATTA].VLevel, player.Skills[Skill.STRE].VLevel, player.Skills[Skill.DEFE].VLevel, player.Skills[Skill.RANG].VLevel, player.Skills[Skill.MAGI].VLevel)
                                : Utils.NextCombatRanged(player.Skills[Skill.ATTA].Level, player.Skills[Skill.STRE].Level, player.Skills[Skill.DEFE].Level, player.Skills[Skill.RANG].Level, player.Skills[Skill.MAGI].Level);
                            break;
                    }
                }

                reply.AppendFormat(format, s, s.Exp > averageExp + avgExpThreshold ? 3 : (s.Exp < averageExp - avgExpThreshold ? 4 : 7), s.Exp == highestLevelExp ? @"\u" : string.Empty, next > 0 ? "(+" + next + ")" : string.Empty);
            }
            await bc.SendReply(reply);

            // Show player performance if applicable
            var dblastupdate = await Database.LastUpdate(player.Name);
            if (dblastupdate != null && dblastupdate.Length == 8)
            {
                DateTime lastupdate = dblastupdate.ToDateTime();
                string perf;
                reply = new StringBuilder(512);

                var p_old = await Player.FromDatabase(player.Name, lastupdate);
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Today", p_old.Skills[Skill.COMB], player.Skills[Skill.COMB]);
                    if (perf != null)
                    {
                        reply.Append(perf + " | ");
                    }
                }
                p_old = await Player.FromDatabase(player.Name, lastupdate.AddDays(-((int) lastupdate.DayOfWeek)));
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Week", p_old.Skills[Skill.COMB], player.Skills[Skill.COMB]);
                    if (perf != null)
                    {
                        reply.Append(perf + " | ");
                    }
                }
                p_old = await Player.FromDatabase(player.Name, lastupdate.AddDays(1 - lastupdate.Day));
                if (p_old.Ranked)
                {
                    perf = _GetPerformance("Month", p_old.Skills[Skill.COMB], player.Skills[Skill.COMB]);
                    if (perf != null)
                    {
                        reply.Append(perf + " | ");
                    }
                }
                p_old = await Player.FromDatabase(player.Name, lastupdate.AddDays(1 - lastupdate.DayOfYear));
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
                    await bc.SendReply(r.EndsWithI(" | ") ? r.Substring(0, r.Length - 3) : r);
                }
            }
        }
    }
}
