using System.Globalization;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  internal class Item {
    public Item(int id) {
      Id = id;
      LoadFromWeb();
    }

    public Item(int id, string name) {
      Id = id;
      Name = name;
    }

    public int Id {
      get;
      private set;
    }

    public string Name {
      get;
      set;
    }

    public bool Members {
      get;
      set;
    }

    public string Quests {
      get;
      set;
    }

    public bool Tradable {
      get;
      set;
    }

    public bool Stackable {
      get;
      set;
    }

    public string Examine {
      get;
      set;
    }

    public double Weight {
      get;
      set;
    }

    public int HighAlch {
      get;
      set;
    }

    public int LowAlch {
      get;
      set;
    }

    public int MarketPrice {
      get;
      set;
    }

    public string Location {
      get;
      set;
    }

    public void LoadFromWeb() {
      try {
        string itemPage = new WebClient().DownloadString("http://www.zybez.net/exResults.aspx?type=1&id=" + Id);
        JObject item = JObject.Parse(itemPage);

        Name = (string) item["name"];
        Members = (bool) item["members"];
        Quests = (string) item["item_quests"];
        Tradable = (string) item["item_tradable"] == "True";
        Stackable = (string) item["item_stackable"] == "True";
        Examine = (string) item["examine"];
        Weight = double.Parse((string) item["item_weight"], CultureInfo.InvariantCulture);
        HighAlch = int.Parse((string) item["item_price_alchemy_max"], CultureInfo.InvariantCulture);
        LowAlch = int.Parse((string) item["item_price_alchemy_min"], CultureInfo.InvariantCulture);
        MarketPrice = (int) item["price"];
        Location = (string) item["item_source_text_en"];

        if (Quests == "No") {
          Quests = null;
        }
      } catch {
      }
    }
  }
}
