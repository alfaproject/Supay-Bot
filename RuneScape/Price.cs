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
            set;
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

        public void SaveToDB(bool updateDate)
        {
            string lastUpdate;
            if (updateDate)
            {
                lastUpdate = DateTime.UtcNow.ToStringI("yyyyMMddHHmm");
            }
            else
            {
                lastUpdate = Database.Lookup("lastUpdate", "prices", "ORDER BY lastUpdate DESC", null, string.Empty);
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

        public void LoadFromCache()
        {
            this.LoadFromDB();
            if ((DateTime.UtcNow - this.LastUpdate).Days > 1)
            {
                this.LoadFromGE();
            }
        }

        public async Task LoadFromDB()
        {
            var dr = await Database.FetchFirst("SELECT name,price,lastUpdate FROM prices WHERE id=@id", new MySqlParameter("@id", Id));
            if (dr != null)
            {
                this.Name = (string) dr["name"];
                this.MarketPrice = (uint) dr["price"];
                this.LastUpdate = ((string) dr["lastUpdate"]).ToDateTime();
            }
        }

        public void LoadFromGE()
        {
            var pricePage = new WebClient().DownloadString("http://services.runescape.com/m=itemdb_rs/viewitem.ws?obj=" + this.Id);

            var match = Regex.Match(pricePage, @"<h5>([^<]+)</h5>\s+<p>([^<]+)</p>");
            if (match.Success)
            {
                this.Name = match.Groups[1].Value.Trim();
                this.Examine = match.Groups[2].Value.Trim();

                match = Regex.Match(pricePage, @"<img src=""http://www.runescape.com/img/itemdb/(\w+)-icon-big.png");
                if (match.Success)
                {
                    this.IsMember = match.Groups[1].Value == "members";
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">Today's Change:</th>\s+<td class=""\w+"">([^<]+)</td>");
                if (match.Success)
                {
                    this.ChangeToday = match.Groups[1].Value.ToInt32();
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">30 Day Change:</th>\s+<td class=""\w+"">([^%]+)%</td>");
                if (match.Success)
                {
                    this.Change30days = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">90 Day Change:</th>\s+<td class=""\w+"">([^%]+)%</td>");
                if (match.Success)
                {
                    this.Change90days = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">180 Day Change:</th>\s+<td class=""\w+"">([^%]+)%</td>");
                if (match.Success)
                {
                    this.Change180days = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                match = Regex.Match(pricePage, @"<th scope=""row"">Current guide price:</th>\s+<td>([^<]+)</td>");
                if (match.Success)
                {
                    this.MarketPrice = match.Groups[1].Value.ToInt32();

                    this.SaveToDB(false);
                }
            }
        }
    }
}
