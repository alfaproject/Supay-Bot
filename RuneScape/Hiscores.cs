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
      string hiscore_page;

      if (activity == null)
      {
        this.Name = skill;

        // get this skill hiscore table
        hiscore_page = wc.DownloadString("http://hiscore.runescape.com/overall.ws?table=" + Skill.NameToId(skill) + "&rank=" + rank);
        foreach (Match M in Regex.Matches(hiscore_page, @"<span class=""rankColumn"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""nameColumn"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""\w+Column"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""xpColumn"">\s+<span>([^<]+)</span>", RegexOptions.Singleline))
        {
          this.Add(new Skill(this.Name, int.Parse(M.Groups[1].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture), int.Parse(M.Groups[3].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture), long.Parse(M.Groups[4].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture)));
          var newReg = new Regex(@"\W");
          this[this.Count - 1].RSN = newReg.Replace(M.Groups[2].Value, "_");
        }
      }
      else
      {
        this.Name = activity;

        // get this activity hiscore table
        hiscore_page = wc.DownloadString("http://hiscore.runescape.com/overall.ws?category_type=1&table=" + Activity.NameToId(activity) + "&rank=" + rank);

        foreach (Match M in Regex.Matches(hiscore_page, @"<span class=""rankColumn"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""nameColumn"">\s+<span>([^<]+)</span>\s+</span>\s+<span class=""\w+Column"">\s+<span>([^<]+)</span>", RegexOptions.Singleline))
        {
          this.Add(new Activity(this.Name, int.Parse(M.Groups[1].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture), int.Parse(M.Groups[3].Value.Replace(",", string.Empty), CultureInfo.InvariantCulture)));
          this[this.Count - 1].RSN = M.Groups[2].Value.Replace(' ', '_');
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
