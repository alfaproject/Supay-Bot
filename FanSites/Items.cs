using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  class Items : List<Item> {

    public Items(string query)
      : base() {
      try {
        string resultsPage = new System.Net.WebClient().DownloadString("http://www.zybez.net/exResults.aspx?type=1&search=name=" + query);
        JArray results = (JArray)JObject.Parse(resultsPage)["results"];

        foreach (JObject item in results) {
          this.Add(new Item(int.Parse((string)item["id"], CultureInfo.InvariantCulture), (string)item["name"]));
        }
      } catch {
      }
    }

  } //class Items
} //namespace Supay.Bot