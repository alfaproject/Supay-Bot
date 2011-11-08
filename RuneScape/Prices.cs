using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  class Prices : List<Price> {

    public Prices() {
    }

    public Prices(string query) {
      Search(query);
    }

    public void Search(string query) {
      string pricesPage = string.Empty;

      try {
        pricesPage = new System.Net.WebClient().DownloadString("http://itemdb-rs.runescape.com/results.ws?query=" + query + "&price=all&members=");
      } catch {
      }

      int totalItems;
      if (int.TryParse(Regex.Match(pricesPage, "(\\d+) items matched the search term:").Groups[1].Value, out totalItems))
        this.TotalItems = totalItems;

      try {
        string pricesRegex = @"<a href="".+?/viewitem.ws\?obj=(\d+)"">([^<]+)</a></td>\s+<td>([^<]+)</td>\s+<td><span class=""\w+"">([^<]+)</span></td>\s+<td>\s+<img src=""http://www.runescape.com/img/main/serverlist/star_(\w+)";
        foreach (Match priceMatch in Regex.Matches(pricesPage, pricesRegex, RegexOptions.Singleline)) {
          Price price = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value.Trim(), priceMatch.Groups[3].Value.ToInt32(), priceMatch.Groups[4].Value.ToInt32(), priceMatch.Groups[5].Value == "members");
          price.SaveToDB(false);
          this.Add(price);
        }

        if (this.TotalItems < this.Count)
          this.TotalItems = this.Count;
      } catch {
      }
    }

    public void SearchExact(string item) {
      item = item.Replace("\"", string.Empty).Replace("*", string.Empty);

      string pricesPage = new System.Net.WebClient().DownloadString("http://itemdb-rs.runescape.com/results.ws?query=\"" + item + "\"&price=all&members=");
      string pricesRegex = @"<a href="".+?/viewitem.ws\?obj=(\d+)"">([^<]+)</a></td>\s+<td>([^<]+)</td>\s+<td><span class=""\w+"">([^<]+)</span></td>\s+<td>\s+<img src=""http://www.runescape.com/img/main/serverlist/star_(\w+)";
      foreach (Match priceMatch in Regex.Matches(pricesPage, pricesRegex, RegexOptions.Singleline)) {
        Price price = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value.Trim(), priceMatch.Groups[3].Value.ToInt32(), priceMatch.Groups[4].Value.ToInt32(), priceMatch.Groups[5].Value == "members");
        price.SaveToDB(false);
        this.Add(price);
      }
    }

    public int TotalItems {
      get;
      private set;
    }

  } //class Prices
} //namespace Supay.Bot