using System;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  class Price {

    public Price(int id, string name, int currentPrice) {
      this.Id = id;
      this.Name = name;
      this.MarketPrice = currentPrice;
    }

    public Price(int id, string name, int currentPrice, int changeToday, bool member)
      : this(id, name, currentPrice) {
      this.ChangeToday = changeToday;
      this.IsMember = member;
    }

    public Price(int id) {
      this.Id = id;
    }

    public int Id {
      get;
      private set;
    }

    public string Name {
      get;
      private set;
    }

    public bool IsMember {
      get;
      private set;
    }

    public int MarketPrice {
      get;
      set;
    }

    public int MinimumPrice {
      get;
      private set;
    }

    public int MaximumPrice {
      get;
      private set;
    }

    public int ChangeToday {
      get;
      private set;
    }

    public string Examine {
      get;
      private set;
    }

    public double Change30days {
      get;
      private set;
    }

    public double Change90days {
      get;
      private set;
    }

    public double Change180days {
      get;
      private set;
    }

    public DateTime LastUpdate {
      get;
      private set;
    }

    public void SaveToDB(bool updateDate) {
      string lastUpdate;
      if (updateDate) {
        lastUpdate = DateTime.UtcNow.ToStringI("yyyyMMddHHmm");
      } else {
        lastUpdate = Database.Lookup("lastUpdate", "prices", "ORDER BY lastUpdate DESC", null, string.Empty);
      }

      try {
        Database.Insert("prices", "id", this.Id.ToStringI(),
                                  "name", this.Name,
                                  "price", this.MarketPrice.ToStringI(),
                                  "lastUpdate", lastUpdate);
      } catch {
        Database.Update("prices", "id=" + this.Id.ToStringI(),
                                  "name", this.Name,
                                  "price", this.MarketPrice.ToStringI(),
                                  "lastUpdate", lastUpdate);
      }
    }

    public void LoadFromCache() {
      this.LoadFromDB();
      if ((DateTime.UtcNow - this.LastUpdate).Days > 1)
        this.LoadFromGE();
    }

    public void LoadFromDB() {
      SQLiteDataReader dr = Database.ExecuteReader("SELECT name, price, lastUpdate FROM prices WHERE id=" + this.Id + ";");
      if (dr.Read()) {
        this.Name = dr.GetString(0);
        this.MarketPrice = dr.GetInt32(1);
        this.LastUpdate = dr.GetString(2).ToDateTime();
      }
      dr.Close();
    }

    public void LoadFromGE() {
      string  pricePage = new System.Net.WebClient().DownloadString("http://itemdb-rs.runescape.com/viewitem.ws?obj=" + this.Id);

      string priceRegex = @"<div class=""subsectionHeader"">\s+";
      priceRegex += @"(.+?)\s+";
      priceRegex += @"</div>\s+";
      priceRegex += @"<div id=""item_additional"" class=""inner_brown_box"">\s+";
      priceRegex += @"<img [^>]+>\s+";
      priceRegex += @"(.+?)\s+";
      priceRegex += @"<br>\s+";
      priceRegex += @"<br>\s+";
      priceRegex += @"<b>Current market price range:</b><br>\s+";
      priceRegex += @"<span>\s+";
      priceRegex += @"<b>Minimum price:</b>\s*([0-9,.mk]+)\s+";
      priceRegex += @"</span>\s+";
      priceRegex += @"<span class=""spaced_span"">\s+";
      priceRegex += @"<b>Market price:</b>\s*([0-9,.mk]+)\s+";
      priceRegex += @"</span>\s+";
      priceRegex += @"<span>\s+";
      priceRegex += @"<b>Maximum price:</b>\s*([0-9,.mk]+)\s+";
      priceRegex += @"</span>\s+";
      priceRegex += @"<br><br>\s+";
      priceRegex += @"<b>Change in price:</b><br>\s+";
      priceRegex += @"<span[^>]*>\s+";
      priceRegex += @"<b>30 Days:</b> <span class=""\w+"">([0-9.+-]+)%</span>\s+";
      priceRegex += @"</span>\s+";
      priceRegex += @"<span[^>]*>\s+";
      priceRegex += @"<b>90 Days:</b> <span class=""\w+"">([0-9.+-]+)%</span>\s+";
      priceRegex += @"</span>\s+";
      priceRegex += @"<span[^>]*>\s+";
      priceRegex += @"<b>180 Days:</b> <span class=""\w+"">([0-9.+-]+)%";

      Match priceMatch = Regex.Match(pricePage, priceRegex, RegexOptions.Singleline);
      if (priceMatch.Success) {
        this.Name = priceMatch.Groups[1].Value;
        this.Examine = priceMatch.Groups[2].Value;
        this.MinimumPrice = priceMatch.Groups[3].Value.ToInt32();
        this.MarketPrice = priceMatch.Groups[4].Value.ToInt32();
        this.MaximumPrice = priceMatch.Groups[5].Value.ToInt32();
        this.Change30days = double.Parse(priceMatch.Groups[6].Value, CultureInfo.InvariantCulture);
        this.Change90days = double.Parse(priceMatch.Groups[7].Value, CultureInfo.InvariantCulture);
        this.Change180days = double.Parse(priceMatch.Groups[8].Value, CultureInfo.InvariantCulture);
      }

      if (!string.IsNullOrEmpty(this.Name) && this.MarketPrice > 0)
        this.SaveToDB(false);
    }

  } //class Price
} //namespace Supay.Bot