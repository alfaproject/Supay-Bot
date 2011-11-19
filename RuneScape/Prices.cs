using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Supay.Bot
{
  internal class Prices : List<Price>
  {
    public Prices()
    {
    }

    public Prices(string query)
    {
      this.Search(query);
    }

    public int TotalItems
    {
      get;
      private set;
    }

    public void Search(string query)
    {
      string pricesPage = string.Empty;

      try
      {
        pricesPage = new WebClient().DownloadString("http://itemdb-rs.runescape.com/results.ws?query=" + query + "&price=all&members=");
      }
      catch
      {
      }

      int totalItems;
      if (int.TryParse(Regex.Match(pricesPage, "(\\d+) items matched the search term:").Groups[1].Value, out totalItems))
      {
        this.TotalItems = totalItems;
      }

      try
      {
        const string pricesRegex = @"<a href="".+?/viewitem.ws\?obj=(\d+)"">([^<]+)</a></td>\s+<td>([^<]+)</td>\s+<td><span class=""\w+"">([^<]+)</span></td>\s+<td>\s+<img src=""http://www.runescape.com/img/main/serverlist/star_(\w+)";
        foreach (Match priceMatch in Regex.Matches(pricesPage, pricesRegex, RegexOptions.Singleline))
        {
          var price = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value.Trim(), priceMatch.Groups[3].Value.ToInt32(), priceMatch.Groups[4].Value.ToInt32(), priceMatch.Groups[5].Value == "members");
          price.SaveToDB(false);
          this.Add(price);
        }

        if (this.TotalItems < this.Count)
        {
          this.TotalItems = this.Count;
        }
      }
      catch
      {
      }
    }

    public void SearchExact(string item)
    {
      item = item.Replace("\"", string.Empty).Replace("*", string.Empty);

      string pricesPage = new WebClient().DownloadString("http://itemdb-rs.runescape.com/results.ws?query=\"" + item + "\"&price=all&members=");
      const string pricesRegex = @"<a href="".+?/viewitem.ws\?obj=(\d+)"">([^<]+)</a></td>\s+<td>([^<]+)</td>\s+<td><span class=""\w+"">([^<]+)</span></td>\s+<td>\s+<img src=""http://www.runescape.com/img/main/serverlist/star_(\w+)";
      foreach (Match priceMatch in Regex.Matches(pricesPage, pricesRegex, RegexOptions.Singleline))
      {
        var price = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value.Trim(), priceMatch.Groups[3].Value.ToInt32(), priceMatch.Groups[4].Value.ToInt32(), priceMatch.Groups[5].Value == "members");
        price.SaveToDB(false);
        this.Add(price);
      }
    }
  }
}
