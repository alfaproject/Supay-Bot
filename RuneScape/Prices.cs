using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal class Prices : List<Price>
    {
        private const string REGEX_PRICES = @"<a href="".+?/viewitem.ws\?obj=(\d+)"">([^<]+)</a>\s+</td>\s+<td>\s+<img src=""http://www.runescape.com/img/itemdb/(members|free)-icon.png""[^>]+>\s+</td>\s+<td class=""price"">([^<]+)</td>\s+<td class=""[^""]+"">([^<]+)</td>";

        public int TotalItems
        {
            get;
            private set;
        }

        public static async Task<Prices> FromRuneScapeSearch(string query)
        {
            var prices = new Prices();

            string pricesPage = string.Empty;

            try
            {
                pricesPage = await new WebClient().DownloadStringTaskAsync("http://services.runescape.com/m=itemdb_rs/results.ws?query=" + query);
            }
            catch
            {
            }

            int totalItems;
            if (int.TryParse(Regex.Match(pricesPage, @"(\d+)</em> results").Groups[1].Value, out totalItems))
            {
                prices.TotalItems = totalItems;
            }

            foreach (Match priceMatch in Regex.Matches(pricesPage, REGEX_PRICES, RegexOptions.Singleline))
            {
                var price = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value.Trim(), priceMatch.Groups[4].Value.ToInt32(), priceMatch.Groups[5].Value.ToInt32(), priceMatch.Groups[3].Value == "members");
                price.SaveToDB(false);
                prices.Add(price);
            }

            if (prices.TotalItems < prices.Count)
            {
                prices.TotalItems = prices.Count;
            }

            return prices;
        }

        public static async Task<Prices> FromRuneScapeSearchExact(string item)
        {
            var prices = new Prices();

            item = item.Replace("\"", string.Empty).Replace("*", string.Empty);

            string pricesPage = await new WebClient().DownloadStringTaskAsync("http://services.runescape.com/m=itemdb_rs/results.ws?query=\"" + item + "\"");
            foreach (Match priceMatch in Regex.Matches(pricesPage, REGEX_PRICES, RegexOptions.Singleline))
            {
                var price = new Price(int.Parse(priceMatch.Groups[1].Value, CultureInfo.InvariantCulture), priceMatch.Groups[2].Value.Trim(), priceMatch.Groups[4].Value.ToInt32(), priceMatch.Groups[5].Value.ToInt32(), priceMatch.Groups[3].Value == "members");
                price.SaveToDB(false);
                prices.Add(price);
            }

            return prices;
        }
    }
}
