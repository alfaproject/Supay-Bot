using System.Net;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  internal class Monster {
    public int Id {
      get;
      set;
    }

    public string Name {
      get;
      set;
    }

    public string Examine {
      get;
      set;
    }

    public int Hits {
      get;
      set;
    }

    public int Level {
      get;
      set;
    }

    public bool Members {
      get;
      set;
    }

    public string Habitat {
      get;
      set;
    }

    public bool Aggressive {
      get;
      set;
    }

    public string TopDrops {
      get;
      set;
    }

    public string Drops {
      get;
      set;
    }

    public void Update() {
      try {
        string npcPage = new WebClient().DownloadString("http://www.zybez.net/exResults.aspx?type=2&id=" + Id);
        JObject npc = JObject.Parse(npcPage);

        Name = (string) npc["name"];
        Examine = (string) npc["examine"];
        Hits = (int) npc["hp"];
        Level = (int) npc["combat"];
        Members = (bool) npc["members"];
        Habitat = (string) npc["locstring"];
        Aggressive = (string) npc["npc_is_aggresive"] == "True";
        TopDrops = (string) npc["npc_top_drops_en"];
        Drops = (string) npc["npc_drops_en"];
      } catch {
      }
    }
  }
}
