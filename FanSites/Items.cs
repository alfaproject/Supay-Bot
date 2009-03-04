using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BigSister {
  class Items : List<Item> {

    public Items(string query) : base() {
      try {
        string itemsPage = new System.Net.WebClient().DownloadString("http://www.tip.it/runescape/index.php?rs2item=&orderby=0&keywords=" + query + "&Players=all&category=0&subcategory=0&cmd=8&action=Manage_Items&search=1&submit=Simple+Search");

        foreach (Match itemMatch in Regex.Matches(itemsPage, @"<tr>\s+<td><a href=""index\.php\?rs2item_id=(\d+)"">([^<]+)</a></td>", RegexOptions.Singleline))
          this.Add(new Item(int.Parse(itemMatch.Groups[1].Value), itemMatch.Groups[2].Value));
      } catch {
      }
    }

  } //class Items
} //namespace BigSister