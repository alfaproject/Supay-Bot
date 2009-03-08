using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BigSister {

  class Hiscores : List<Hiscore> {
    public Hiscores(string skill, string minigame, int rank)
      : base(21) {
      System.Net.WebClient wc = new System.Net.WebClient();
      string hiscore_page;

      if (minigame == null) {
        Name = skill;

        // get this skill hiscore table
        hiscore_page = wc.DownloadString("http://hiscore.runescape.com/overall.ws?table=" + Skill.NameToId(skill) + "&rank=" + rank);

        foreach (Match M in Regex.Matches(hiscore_page, "<td class=\"[^\"]+\">([^<]+)</td>\\s+<td class=\"alL\"><[^>]+>([^<]+)</a></td>\\s+<td class=\"[^\"]+\">([\\d,]+)</td>\\s+<td class=\"[^\"]+\">([\\d,]+)</td>", RegexOptions.Singleline)) {
          this.Add(new Skill(Name,
                             int.Parse(M.Groups[1].Value.Replace(",", string.Empty)),
                             int.Parse(M.Groups[3].Value.Replace(",", string.Empty)),
                             int.Parse(M.Groups[4].Value.Replace(",", string.Empty))));
          this[this.Count - 1].RSN = M.Groups[2].Value.Replace(' ', '_');
        }

      } else {
        Name = minigame;

        // get this minigame hiscore table
        hiscore_page = wc.DownloadString("http://hiscore.runescape.com/overall.ws?category_type=1&table=" + Minigame.NameToId(minigame) + "&rank=" + rank);

        foreach (Match M in Regex.Matches(hiscore_page, "<td class=\"[^\"]+\">([^<]+)</td>\\s+<td class=\"alL\"><[^>]+>([^<]+)</a></td>\\s+<td class=\"[^\"]+\">([\\d,]+)</td>", RegexOptions.Singleline)) {
          this.Add(new Minigame(Name,
                                int.Parse(M.Groups[1].Value.Replace(",", string.Empty)),
                                int.Parse(M.Groups[3].Value.Replace(",", string.Empty))));
          this[this.Count - 1].RSN = M.Groups[2].Value.Replace(' ', '_');
        }

      }
    }

    public string Name {
      get;
      private set;
    }

  } // class Hiscores
} // namespace BigSister