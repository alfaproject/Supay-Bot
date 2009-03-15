using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BigSister {
  class Worlds : Dictionary<int, World> {

    public List<World> FindActivity(string activity) {
      List<World> ret = new List<World>();
      foreach (World world in this.Values)
        if (world.Activity.ToLowerInvariant().Contains(activity.ToLowerInvariant()))
          ret.Add(world);
      return ret;
    }

    public Worlds() {
      System.Net.WebClient WebClient = new System.Net.WebClient();
      ParseWorldPage(WebClient.DownloadString("http://www.runescape.com/slu.ws"));
    }

    private void ParseWorldPage(string worldPage) {
      MatchCollection worlds = Regex.Matches(worldPage, @"<td class=""(f|m).*?World (\d+).*?</td>\s+<td>([^<]+)</td>\s+<td class=""\w+"">([^<]+)</td>\s+<td[^>]*?>([^<]+)</td>\s+<td.*?title=""(Y|N)""></td>\s+<td.*?title=""(Y|N)""></td>\s+<td.*?title=""(Y|N)""></td>", RegexOptions.Singleline);
      foreach (Match W in worlds) {
        World NewWorld = new World();
        NewWorld.Member = (W.Groups[1].Value == "m");
        NewWorld.Number = int.Parse(W.Groups[2].Value);

        switch (W.Groups[3].Value.ToUpperInvariant()) {
          case "FULL":
            NewWorld.Players = 2000;
            NewWorld.Status = "Full";
            break;
          case "OFFLINE":
            NewWorld.Players = 0;
            NewWorld.Status = "Offline";
            break;
          default:
            NewWorld.Players = int.Parse(W.Groups[3].Value.Split(' ')[0]);
            NewWorld.Status = "Online";
            break;
        }

        NewWorld.Location = W.Groups[4].Value;
        NewWorld.Activity = W.Groups[5].Value;

        NewWorld.LootShare = (W.Groups[6].Value == "Y");
        NewWorld.QuickChat = (W.Groups[7].Value == "Y");
        NewWorld.PVP = (W.Groups[8].Value == "Y");

        this.Add(NewWorld.Number, NewWorld);
      }
    }

  }
}