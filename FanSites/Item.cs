using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  class Item {

    public Item(int id) {
      this.Id = id;
      this.LoadFromWeb();
    }

    public Item(int id, string name) {
      this.Id = id;
      this.Name = name;
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
        string itemPage = new System.Net.WebClient().DownloadString("http://www.zybez.net/exResults.aspx?type=1&id=" + this.Id);
        JObject item = JObject.Parse(itemPage);

        this.Name = (string)item["name"];
        this.Members = (bool)item["members"];
        this.Quests = (string)item["item_quests"];
        this.Tradable = (string)item["item_tradable"] == "True";
        this.Stackable = (string)item["item_stackable"] == "True";
        this.Examine = (string)item["examine"];
        this.Weight = double.Parse((string)item["item_weight"], CultureInfo.InvariantCulture);
        this.HighAlch = int.Parse((string)item["item_price_alchemy_max"], CultureInfo.InvariantCulture);
        this.LowAlch = int.Parse((string)item["item_price_alchemy_min"], CultureInfo.InvariantCulture);
        this.MarketPrice = (int)item["price"];
        this.Location = (string)item["item_source_text_en"];

        if (this.Quests == "No") {
          this.Quests = null;
        }
      } catch {
      }
    }

  } //class Item
} //namespace Supay.Bot