using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  internal class Items : List<Item> {
    public Items(string query) {
      try {
        string resultsPage = new WebClient().DownloadString("http://www.zybez.net/exResults.aspx?type=1&search=name=" + query);
        var results = (JArray) JObject.Parse(resultsPage)["results"];

        foreach (JObject item in results) {
          Add(new Item(int.Parse((string) item["id"], CultureInfo.InvariantCulture), (string) item["name"]));
        }
      } catch {
      }
    }
  }
}
