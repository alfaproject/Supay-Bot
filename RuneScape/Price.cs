using System;
using System.Data.SQLite;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  internal class Price {
    public Price(int id, string name, int currentPrice) {
      Id = id;
      Name = name;
      MarketPrice = currentPrice;
    }

    public Price(int id, string name, int currentPrice, int changeToday, bool member)
      : this(id, name, currentPrice) {
      ChangeToday = changeToday;
      IsMember = member;
    }

    public Price(int id) {
      Id = id;
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
      set;
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
        Database.Insert("prices", "id", Id.ToStringI(), "name", Name, "price", MarketPrice.ToStringI(), "lastUpdate", lastUpdate);
      } catch {
        Database.Update("prices", "id=" + Id.ToStringI(), "name", Name, "price", MarketPrice.ToStringI(), "lastUpdate", lastUpdate);
      }
    }

    public void LoadFromCache() {
      LoadFromDB();
      if ((DateTime.UtcNow - LastUpdate).Days > 1) {
        LoadFromGE();
      }
    }

    public void LoadFromDB() {
      SQLiteDataReader dr = Database.ExecuteReader("SELECT name, price, lastUpdate FROM prices WHERE id=" + Id + ";");
      if (dr.Read()) {
        Name = dr.GetString(0);
        MarketPrice = dr.GetInt32(1);
        LastUpdate = dr.GetString(2).ToDateTime();
      }
      dr.Close();
    }

    public void LoadFromGE() {
      string pricePage = new WebClient().DownloadString("http://itemdb-rs.runescape.com/viewitem.ws?obj=" + Id);

      string priceRegex = @"<div class=""subsectionHeader"">\s+";
      priceRegex += @"<h2>\s+";
      priceRegex += @"(.+?)\s+";
      priceRegex += @"</h2>\s+";
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
        Name = priceMatch.Groups[1].Value;
        Examine = priceMatch.Groups[2].Value;
        MinimumPrice = priceMatch.Groups[3].Value.ToInt32();
        MarketPrice = priceMatch.Groups[4].Value.ToInt32();
        MaximumPrice = priceMatch.Groups[5].Value.ToInt32();
        Change30days = double.Parse(priceMatch.Groups[6].Value, CultureInfo.InvariantCulture);
        Change90days = double.Parse(priceMatch.Groups[7].Value, CultureInfo.InvariantCulture);
        Change180days = double.Parse(priceMatch.Groups[8].Value, CultureInfo.InvariantCulture);
      }

      if (!string.IsNullOrEmpty(Name) && MarketPrice > 0) {
        SaveToDB(false);
      }
    }
  }
}
