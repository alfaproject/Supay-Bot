using System.Collections.Generic;
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
                                 new StringBuilder(512).AppendFormat(@"\b{0}\b exp. to next level:", player.Name),
                                 (sb, skill) => sb.AppendFormat(@"\c{0} {1:N0}\c {2};", skill.VLevel >= skill.MaxLevel ? 4 : 3, skill.ExpToVLevel, skill.Name)
                                 ));
                return;
            }
            
            // select non meta player skills
            var playerSkills = player.Skills.Values.Where(s => s.Name != Skill.OVER && s.Name != Skill.COMB).ToList();

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

            var reply = new StringBuilder(512)
                .AppendFormat(@"\b{0}\b \c7{1:n}\c | level:\c7 {2:N0}\c (\c07{3:N1}\c avg) | exp:\c7 {1:e}\c (\c07{4:0.#%}\c of {5}) | rank:\c7 {1:R}\c", player.Name, player.Skills[Skill.OVER], overallLevel, avgSkillLevel, (double) player.Skills[Skill.OVER].Exp / maxOverallExp, maxOverallLevel);

            // add up SS rank if applicable
            var ssPlayers = from p in new Players("SS")
                            let overallSkill = p.Skills[Skill.OVER]
                            orderby overallSkill.Level descending, overallSkill.Exp descending
                            select p;
            var indexOfPlayer = ssPlayers.FindIndex(p => p.Name == player.Name);
            if (indexOfPlayer != -1)
            {
                reply.AppendFormat(@" (SS rank: \c07{0}\c)", indexOfPlayer + 1);
            }

            // output overall information
            bc.SendReply(reply);

            // output skills
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
            format += @"}\c {0:n}{2};";

            var avgExpThreshold = averageExp / 5;
            var highestExp = playerSkills.Max(s => s.Exp);

            var filteredSkills = playerSkills.Where(s => (lessThan == 0 || s.Exp < lessThan) && (greaterThan == 0 || s.Exp > greaterThan)).ToList();

            bc.SendReply(filteredSkills.Where(s => s.IsCombat).Concat(player.Skills[Skill.COMB]).Aggregate(
                new StringBuilder(@"\uCombat skills\u:", 512),
                (sb, s) => sb.AppendFormat(format, s, s.Exp > averageExp + avgExpThreshold ? 3 : (s.Exp < averageExp - avgExpThreshold ? 4 : 7), s.Exp == highestExp ? @"\u" : string.Empty)
                ).AppendFormat(@" (\c7{0}\c)", player.CombatClass));

            bc.SendReply(filteredSkills.Where(s => !s.IsCombat).AggregateOrDefault(
                new StringBuilder(@"\uOther skills\u:", 512),
                (sb, s) => sb.AppendFormat(format, s, s.Exp > averageExp + avgExpThreshold ? 3 : (s.Exp < averageExp - avgExpThreshold ? 4 : 7), s.Exp == highestExp ? @"\u" : string.Empty)
                ));

            // output activities
            if (lessThan == 0 && greaterThan == 0)
            {
                bc.SendReply(player.Activities.Values.Where(a => a.Rank > 0).AggregateOrDefault(
                    new StringBuilder(@"\uActivities\u:", 512),
                    (sb, a) => sb.AppendFormat(@"\c7 {0:N0}\c {1};", rankMatch.Success ? a.Rank : a.Score, a.Name)
                    ));
            }
        }
    }
}
