using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  class Monsters : List<Monster> {

    public Monsters(string query)
      : base() {
      try {
        string resultsPage = new System.Net.WebClient().DownloadString("http://www.zybez.net/exResults.aspx?type=2&search=name=" + query);
        JArray results = (JArray)JObject.Parse(resultsPage)["results"];

        foreach (JObject npc in results) {
          this.Add(new Monster {
            Id = (int)npc["id"],
            Name = (string)npc["name"],
            Examine = (string)npc["examine"],
            Hits = (int)npc["hp"],
            Level = (int)npc["combat"],
            Members = (bool)npc["members"],
            Habitat = (string)npc["locstring"]
          });
        }
      } catch {
      }
    }

  } //class Monsters
} //namespace Supay.Bot