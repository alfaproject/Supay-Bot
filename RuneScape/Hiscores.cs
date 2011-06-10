using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {

  class Hiscores : List<Hiscore> {
    public Hiscores(string skill, string activity, int rank)
      : base(22) {
      System.Net.WebClient wc = new System.Net.WebClient();
      string hiscore_page;

      if (activity == null) {
        Name = skill;

        // get this skill hiscore table
        hiscore_page = wc.DownloadString("http://hiscore.runescape.com/overall.ws?table=" + Skill.NameToId(skill) + "&rank=" + rank);
        foreach (Match M in Regex.Matches(hiscore_page, @"<span class=""rankColumn"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""nameColumn"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""\w+Column"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""xpColumn"">\s+<span>([^<]+)</span>", RegexOptions.Singleline)) {
          this.Add(new Skill(Name,
                             int.Parse(M.Groups[1].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture),
                             int.Parse(M.Groups[3].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture),
                             long.Parse(M.Groups[4].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture)));
          Regex newReg = new Regex(@"\W");
          this[this.Count - 1].RSN = newReg.Replace(M.Groups[2].Value, "_");
        }

      } else {
        Name = activity;

        // get this activity hiscore table
        hiscore_page = wc.DownloadString("http://hiscore.runescape.com/overall.ws?category_type=1&table=" + Activity.NameToId(activity) + "&rank=" + rank);

        foreach (Match M in Regex.Matches(hiscore_page, @"<span class=""rankColumn"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""nameColumn"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""\w+Column"">\s+<span>([^<]+)</span>", RegexOptions.Singleline)) {
          this.Add(new Activity(Name,
                                int.Parse(M.Groups[1].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture),
                                int.Parse(M.Groups[3].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture)));
          this[this.Count - 1].RSN = M.Groups[2].Value.Replace(' ', '_');
        }

      }
    }

    public string Name {
      get;
      private set;
    }

  } // class Hiscores
} // //namespace Supay.Bot