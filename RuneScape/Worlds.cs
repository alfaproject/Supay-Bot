using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  class Worlds : Dictionary<int, World> {

    public List<World> FindActivity(string activity) {
      List<World> ret = new List<World>();
      foreach (World world in this.Values)
        if (world.Activity.ContainsI(activity))
          ret.Add(world);
      return ret;
    }

    public Worlds() {
      System.Net.WebClient WebClient = new System.Net.WebClient();
      ParseWorldPage(WebClient.DownloadString("http://www.runescape.com/slu.ws"));
    }

    private void ParseWorldPage(string worldPage) {
      MatchCollection worlds = Regex.Matches(worldPage, @"<td class=""(f|m)"">\s+(?:<a[^>]*>)?World (\d+)(?:</a>)?\s+</td>\s+<td>([^<]+)</td>\s+<td class=""[^\x22]+"">([^<]+)</td>\s+<td[^>]*><img.+?title=""(Y|N)", RegexOptions.Singleline);

      foreach (Match W in worlds) {
        World NewWorld = new World();
        NewWorld.Member = (W.Groups[1].Value == "m");
        NewWorld.Number = int.Parse(W.Groups[2].Value, CultureInfo.InvariantCulture);

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
            NewWorld.Players = int.Parse(W.Groups[3].Value.Split(' ')[0], CultureInfo.InvariantCulture);
            NewWorld.Status = "Online";
            break;
        }

        NewWorld.Activity = W.Groups[4].Value;

        switch (NewWorld.Activity) {
          case "PvP World":
            NewWorld.PVP = true;
            break;
          case "Bounty World":
          case "Bounty World (+1 item)":
            NewWorld.QuickChat = true;
            break;
          default:
            break;
        }

        NewWorld.LootShare = (W.Groups[5].Value == "Y");

        this.Add(NewWorld.Number, NewWorld);
      }
    }

  }
}