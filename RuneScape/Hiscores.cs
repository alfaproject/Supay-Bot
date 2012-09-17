using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Supay.Bot
{
    internal class Hiscores : List<Hiscore>
    {
        public Hiscores(string skill, string activity, int rank)
            : base(22)
        {
            var wc = new WebClient();

            if (activity == null)
            {
                this.Name = skill;

                // get this skill hiscore table
                var hiscorePage = wc.DownloadString("http://hiscore.runescape.com/overall.ws?table=" + Skill.NameToId(skill) + "&rank=" + rank);
                const string rankRegex = @"<span class=""columnRank"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""columnName"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""columnLevel"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""columnXp"">\s+<span>([^<]+)</span>";
                foreach (Match m in Regex.Matches(hiscorePage, rankRegex, RegexOptions.Singleline))
                {
                    this.Add(new Skill(this.Name, int.Parse(m.Groups[1].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture), int.Parse(m.Groups[3].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture), long.Parse(m.Groups[4].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture)));
                    var newReg = new Regex(@"\W");
                    this[this.Count - 1].RSN = newReg.Replace(m.Groups[2].Value, "_");
                }
            }
            else
            {
                this.Name = activity;

                // get this activity hiscore table
                var hiscorePage = wc.DownloadString("http://hiscore.runescape.com/overall.ws?category_type=1&table=" + Activity.NameToId(activity) + "&rank=" + rank);
                const string rankRegex = @"<span class=""columnRank"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""columnName"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""columnScore"">\s+<span>([^<]+)</span>";
                foreach (Match m in Regex.Matches(hiscorePage, rankRegex, RegexOptions.Singleline))
                {
                    this.Add(new Activity(this.Name, int.Parse(m.Groups[1].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture), int.Parse(m.Groups[3].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture)));
                    this[this.Count - 1].RSN = m.Groups[2].Value.Replace(' ', '_');
                }
            }
        }

        public string Name
        {
            get;
            private set;
        }
    }
}
