using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  class Items : List<Item> {

    public Items(string query)
      : base() {
      try {
        string itemsPage = new System.Net.WebClient().DownloadString("http://www.tip.it/runescape/index.php?rs2item=&orderby=0&keywords=" + query + "&Players=all&category=0&subcategory=0&cmd=8&action=Manage_Items&search=1&submit=Simple+Search");

        foreach (Match itemMatch in Regex.Matches(itemsPage, @"rs2item_id=(\d+)"">([^<]+)")) {
          this.Add(new Item(int.Parse(itemMatch.Groups[1].Value, CultureInfo.InvariantCulture), itemMatch.Groups[2].Value));
        }
      } catch {
      }
    }

  } //class Items
} ////namespace Supay.Bot