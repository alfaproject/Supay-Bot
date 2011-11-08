using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  internal class Monsters : List<Monster> {
    public Monsters(string query) {
      try {
        string resultsPage = new WebClient().DownloadString("http://www.zybez.net/exResults.aspx?type=2&search=name=" + query);
        var results = (JArray) JObject.Parse(resultsPage)["results"];

        foreach (JObject npc in results) {
          Add(new Monster {
            Id = (int) npc["id"],
            Name = (string) npc["name"],
            Examine = (string) npc["examine"],
            Hits = (int) npc["hp"],
            Level = (int) npc["combat"],
            Members = (bool) npc["members"],
            Habitat = (string) npc["locstring"]
          });
        }
      } catch {
      }
    }
  }
}
