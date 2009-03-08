using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BigSister {
  public class Monsters : Dictionary<string, Monster> {

    public List<Monster> SearchOnline(string Criteria) {
      List<Monster> ResultList = new List<Monster>();

      string MonsterSearchPage = new System.Net.WebClient().DownloadString("http://www.tip.it/runescape/index.php?rs2monster=&orderby=0&levels=All&race=0&keywords=" + Criteria);

      MatchCollection Results = Regex.Matches(MonsterSearchPage, "<td>(\\d+)</td>\\s+<td><a href=\"\\?rs2monster_id=(\\d+)\">([^<]+)</a>");
      if (Results.Count > 0) {
        foreach (Match M in Results) {
          if (this.ContainsKey(M.Groups[3].Value.ToUpperInvariant())) {
            ResultList.Add(this[M.Groups[3].Value.ToUpperInvariant()]);
          } else {
            Monster NewMonster = new Monster(int.Parse(M.Groups[2].Value));
            NewMonster.Name = M.Groups[3].Value;
            NewMonster.Level = int.Parse(M.Groups[1].Value);
            this.Add(M.Groups[3].Value.ToUpperInvariant(), NewMonster);

            ResultList.Add(NewMonster);
          }
        }
      }

      return ResultList;
    }

  }
}