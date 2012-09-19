using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        private static readonly Regex _nextRegex = new Regex(@"\s+@n(?:ext)?", RegexOptions.Compiled);
        private static readonly Regex _expRegex = new Regex(@"\s+@e?xp(?:erience)?", RegexOptions.Compiled);
        private static readonly Regex _rankRegex = new Regex(@"\s+@r(?:ank)?", RegexOptions.Compiled);
        private static readonly Regex _virtualRegex = new Regex(@"\s+@v(?:irtual)?(?:le?ve?l)?", RegexOptions.Compiled);
        private static readonly Regex _lessThanRegex = new Regex(@"\s+<\D*(\d+)([mk]{0,1})", RegexOptions.Compiled);
        private static readonly Regex _greaterThanRegex = new Regex(@"\s+>\D*(\d+)([mk]{0,1})", RegexOptions.Compiled);

        public static async Task Stats(CommandContext bc)
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

            // get <
            var lessThan = 0;
            var lessThanMatch = _lessThanRegex.Match(bc.Message);
            if (lessThanMatch.Success)
            {
                lessThan = int.Parse(lessThanMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                switch (lessThanMatch.Groups[2].Value)
                {
                    case "m":
                        lessThan *= 1000000;
                        break;
                    case "k":
                        lessThan *= 1000;
                        break;
                }

                if (lessThan < 127)
                {
                    lessThan = lessThan.ToExp();
                }

                bc.Message = bc.Message.Replace(lessThanMatch.Value, string.Empty);
            }

            // get >
            var greaterThan = 0;
            var greaterThanMatch = _greaterThanRegex.Match(bc.Message);
            if (greaterThanMatch.Success)
            {
                greaterThan = int.Parse(greaterThanMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                switch (greaterThanMatch.Groups[2].Value)
                {
                    case "m":
                        greaterThan *= 1000000;
                        break;
                    case "k":
                        greaterThan *= 1000;
                        break;
                }

                if (greaterThan < 126)
                {
                    greaterThan = (greaterThan + 1).ToExp();
                }

                bc.Message = bc.Message.Replace(greaterThanMatch.Value, string.Empty);
            }

            // get player
            var player = new Player(bc.GetPlayerName(bc.MessageTokens.Length == 1 ? bc.From.Nickname : bc.MessageTokens.Join(1)));
            if (!player.Ranked)
            {
                bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", player.Name);
                return;
            }

            // !all @next
            if (nextMatch.Success)
            {
                var i = 0;
                bc.SendReply(from skill in player.Skills.SortedByExpToNextVLevel
                             group skill by i++ / 13
                             into skills
                             select skills.Aggregate(
                                 new StringBuilder(@"\b{0}\b's exp. to next level:".FormatWith(player.Name)),
                                 (sb, skill) => sb.AppendFormat(@"\c{0} {1:N0}\c {2};", skill.VLevel >= skill.MaxLevel ? "4" : "3", skill.ExpToVLevel, skill.Name)
                                 ));
                return;
            }
            
            // select non meta player skills
            var playerSkills = (from skill in player.Skills.Values
                                where skill.Name != Skill.OVER && skill.Name != Skill.COMB
                                select skill).ToList();

            // calculate overall max level and exp
            var maxOverallLevel = 0;
            var maxOverallExp = 0L;
            if (virtualMatch.Success)
            {
                maxOverallLevel = 126 * playerSkills.Count;
                maxOverallExp = 200000000L * playerSkills.Count;
            }
            else
            {
                foreach (var skill in playerSkills)
                {
                    maxOverallLevel += skill.MaxLevel;
                    maxOverallExp += skill.MaxLevel.ToExp();
                }
            }

            // calculate overall and average levels
            var overallLevel = virtualMatch.Success ? playerSkills.Sum(skill => skill.VLevel) : player.Skills[Skill.OVER].Level;
            var avgSkillLevel = (double) overallLevel / playerSkills.Count;
            var averageExp = player.Skills[Skill.OVER].Exp / playerSkills.Count;

            // add up SS rank if applicable
            var ssRank = string.Empty;
            var ssPlayers = from p in new Players("SS")
                            let overallSkill = p.Skills[Skill.OVER]
                            orderby overallSkill.Level descending, overallSkill.Exp descending
                            select p;
            var indexOfPlayer = ssPlayers.FindIndex(p => p.Name == player.Name);
            if (indexOfPlayer != -1)
            {
                ssRank = @" (SS rank: \c07{0}\c)".FormatWith(indexOfPlayer + 1);
            }

            // output overall information
            bc.SendReply(@"\b{0}\b \c7{1:n}\c | level:\c7 {2:N0}\c (\c07{3:N1}\c avg) | exp:\c7 {1:e}\c (\c07{4:#.#%}\c of {5}) | rank:\c7 {1:R}\c{6}", player.Name, player.Skills[Skill.OVER], overallLevel, avgSkillLevel, (double) player.Skills[Skill.OVER].Exp / maxOverallExp, maxOverallLevel, ssRank);

            string format;
            if (expMatch.Success)
            {
                format = @" {2}\c{1:00}{0:re}\c {0:n}{2};";
            }
            else if (rankMatch.Success)
            {
                format = @" {2}\c{1:00}{0:r}\c {0:n}{2};";
            }
            else if (virtualMatch.Success)
            {
                format = @" {2}\c{1:00}{0:rv}\c {0:n}{2};";
            }
            else
            {
                format = @" {2}\c{1:00}{0:rl}\c {0:n}{2};";
            }

            var reply = string.Empty;
            var replyCombat = @"\uCombat skills\u:";
            var replyOther = @"\uOther skills\u:";
            for (int i = 1; i < player.Skills.Count - 1; i++)
            {
                Skill s = player.Skills[i];

                if (lessThan > 0 && s.Exp >= lessThan)
                {
                    continue;
                }
                if (greaterThan > 0 && s.Exp <= greaterThan)
                {
                    continue;
                }
                if (lessThan > 0 && greaterThan > 0 && s.Exp >= lessThan && s.Exp <= greaterThan)
                {
                    continue;
                }

                reply = format.FormatWith(s, s.Exp > averageExp * 1.2 ? 3 : (s.Exp < averageExp * .8 ? 4 : 7), s.Exp == player.Skills.Highest[0].Exp ? @"\u" : string.Empty);

                if (s.Name != Skill.ATTA && s.Name != Skill.STRE && s.Name != Skill.DEFE && s.Name != Skill.HITP && s.Name != Skill.PRAY && s.Name != Skill.SUMM && s.Name != Skill.RANG && s.Name != Skill.MAGI)
                {
                    replyOther += reply;
                }
                else
                {
                    replyCombat += reply;
                }
            }
            bc.SendReply(replyCombat + format.Substring(0, format.Length - 1) + @" (\c07{3}\c)", player.Skills[Skill.COMB], 7, string.Empty, player.CombatClass);
            bc.SendReply(replyOther);

            bool ranked = false;
            reply = @"\uActivities\u:";
            foreach (Activity m in player.Activities.Values)
            {
                if (m.Rank > 0)
                {
                    ranked = true;
                    reply += @" \c07{0}\c {1};".FormatWith(rankMatch.Success ? m.Rank : m.Score, m.Name);
                }
            }
            if (ranked)
            {
                bc.SendReply(reply);
            }
        }
    }
}
