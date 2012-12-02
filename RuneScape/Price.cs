using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Supay.Bot
{
    internal class Price
    {
        public Price(int id, string name, int currentPrice)
        {
            this.Id = id;
            this.Name = name;
            this.MarketPrice = currentPrice;
        }

        public Price(int id, string name, int currentPrice, int changeToday, bool member)
            : this(id, name, currentPrice)
        {
            this.ChangeToday = changeToday;
            this.IsMember = member;
        }

        public Price(int id)
        {
            this.Id = id;
        }

        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public bool IsMember
        {
            get;
            private set;
        }

        public long MarketPrice
        {
            get;
            private set;
        }

        public long ChangeToday
        {
            get;
            set;
        }

        public string Examine
        {
            get;
            private set;
        }

        public double Change30days
        {
            get;
            private set;
        }

        public double Change90days
        {
            get;
            private set;
        }

        public double Change180days
        {
            get;
            private set;
        }

        public DateTime LastUpdate
        {
            get;
            private set;
        }

        public async Task SaveToDB(bool updateDate)
        {
            string lastUpdate;
            if (updateDate)
            {
                lastUpdate = DateTime.UtcNow.ToStringI("yyyyMMddHHmm");
            }
            else
            {
                lastUpdate = await Database.Lookup("lastUpdate", "prices", "ORDER BY lastUpdate DESC", null, string.Empty);
            }

            try
            {
                Database.Insert("prices", "id", this.Id.ToStringI(), "name", this.Name, "price", this.MarketPrice.ToStringI(), "lastUpdate", lastUpdate);
            }
            catch
            {
                Database.Update("prices", "id=" + this.Id.ToStringI(), "name", this.Name, "price", this.MarketPrice.ToStringI(), "lastUpdate", lastUpdate);
            }
        }

        public static async Task<Price> FromCache(int id)
        {
            var price = await FromDatabase(id);
            if ((DateTime.UtcNow - price.LastUpdate).Hours < 12)
            {
                return price;
            }

            return await FromRuneScape(id);
        }

        public static async Task<Price> FromDatabase(int id)
        {
            var price = new Price(id);

            var dr = await Database.FetchFirst("SELECT name,price,lastUpdate FROM prices WHERE id=@id", new MySqlParameter("@id", id));
            if (dr != null)
            {
                price.Name = (string) dr["name"];
                price.MarketPrice = (uint) dr["price"];
                price.LastUpdate = ((string) dr["lastUpdate"]).ToDateTime();
            }

            return price;
        }

        public static async Task<Price> FromRuneScape(int id)
        {
            var price = new Price(id);

            var pricePage = await new WebClient().DownloadStringTaskAsync("http://services.runescape.com/m=itemdb_rs/viewitem.ws?obj=" + id);

            var match = Regex.Match(pricePage, @"<h5>([^<]+)</h5>\s+<p>([^<]+)</p>");
            if (match.Success)
            {
                price.Name = match.Groups[1].Value.Trim();
                price.Examine = match.Groups[2].Value.Trim();

                match = Regex.Match(pricePage, @"<img src=""http://www.runescape.com/img/itemdb/(\w+)-icon-big.png");
                if (match.Success)
                {
                    price.IsMember = match.Groups[1].Value == "members";
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">Today's Change:</th>\s+<td class=""\w+"">([^<]+)</td>");
                if (match.Success)
                {
                    price.ChangeToday = match.Groups[1].Value.ToInt32();
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">30 Day Change:</th>\s+<td class=""\w+"">([^%]+)%</td>");
                if (match.Success)
                {
                    price.Change30days = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">90 Day Change:</th>\s+<td class=""\w+"">([^%]+)%</td>");
                if (match.Success)
                {
                    price.Change90days = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">180 Day Change:</th>\s+<td class=""\w+"">([^%]+)%</td>");
                if (match.Success)
                {
                    price.Change180days = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">Current guide price:</th>\s+<td>([^<]+)</td>");
                if (match.Success)
                {
                    price.MarketPrice = match.Groups[1].Value.ToInt32();

                    await price.SaveToDB(false);
                }
            }

            return price;
        }
    }
}
