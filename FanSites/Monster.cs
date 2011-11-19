using System.Net;
using Newtonsoft.Json.Linq;

namespace Supay.Bot
{
  internal class Monster
  {
    public int Id
    {
      get;
      set;
    }

    public string Name
    {
      get;
      set;
    }

    public string Examine
    {
      get;
      set;
    }

    public int Hits
    {
      get;
      set;
    }

    public int Level
    {
      get;
      set;
    }

    public bool Members
    {
      get;
      set;
    }

    public string Habitat
    {
      get;
      set;
    }

    public bool Aggressive
    {
      get;
      set;
    }

    public string TopDrops
    {
      get;
      set;
    }

    public string Drops
    {
      get;
      set;
    }

    public void Update()
    {
      try
      {
        string npcPage = new WebClient().DownloadString("http://www.zybez.net/exResults.aspx?type=2&id=" + this.Id);
        JObject npc = JObject.Parse(npcPage);

        this.Name = (string) npc["name"];
        this.Examine = (string) npc["examine"];
        this.Hits = (int) npc["hp"];
        this.Level = (int) npc["combat"];
        this.Members = (bool) npc["members"];
        this.Habitat = (string) npc["locstring"];
        this.Aggressive = (string) npc["npc_is_aggresive"] == "True";
        this.TopDrops = (string) npc["npc_top_drops_en"];
        this.Drops = (string) npc["npc_drops_en"];
      }
      catch
      {
      }
    }
  }
}
