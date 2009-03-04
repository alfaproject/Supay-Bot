using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BigSister {
  class Item {

    public Item(int id) {
      this.Id = id;
      this.LoadFromWeb();
    }

    public Item(int id, string name) {
      this.Id = id;
      this.Name = name;
    }

    public int Id {
      get;
      private set;
    }

    public string Name {
      get;
      set;
    }

    public bool Members {
      get;
      set;
    }

    public bool Quest {
      get;
      set;
    }

    public bool Tradable {
      get;
      set;
    }

    public bool Stackable {
      get;
      set;
    }

    public string Examine {
      get;
      set;
    }

    public double Weight {
      get;
      set;
    }

    public int HighAlch {
      get;
      set;
    }

    public int LowAlch {
      get;
      set;
    }

    public int MarketPrice {
      get;
      set;
    }

    public string Location {
      get;
      set;
    }

    public void LoadFromWeb() {
      try {
        string itemPage = new System.Net.WebClient().DownloadString("http://www.tip.it/runescape/index.php?rs2item_id=" + this.Id);

        Match M = Regex.Match(itemPage, @"<td class=""header"" colspan=""3"">([^<]+?)</td>", RegexOptions.Singleline);
        if (M.Success)
          this.Name = M.Groups[1].Value.Trim();

        M = Regex.Match(itemPage, @"<b>Members\?</b>\s*?(No|Yes)", RegexOptions.Singleline);
        if (M.Success)
          this.Members = (M.Groups[1].Value == "Yes");

        M = Regex.Match(itemPage, @"<b>Quest\?</b>\s*?(No|Yes)", RegexOptions.Singleline);
        if (M.Success)
          this.Quest = (M.Groups[1].Value == "Yes");

        M = Regex.Match(itemPage, @"<b>Tradeable\?</b>\s*?(No|Yes)", RegexOptions.Singleline);
        if (M.Success)
          this.Tradable = (M.Groups[1].Value == "Yes");

        M = Regex.Match(itemPage, @"<b>Stackable\?</b>\s*?(No|Yes)", RegexOptions.Singleline);
        if (M.Success)
          this.Stackable = (M.Groups[1].Value == "Yes");

        M = Regex.Match(itemPage, @"<b>Examine:</b>([^<]+?)</td>", RegexOptions.Singleline);
        if (M.Success)
          this.Examine = M.Groups[1].Value.Trim();

        M = Regex.Match(itemPage, @"<b>Weight:</b>(.*?)kg", RegexOptions.Singleline);
        if (M.Success)
          this.Weight = double.Parse(M.Groups[1].Value.Trim(), CultureInfo.InvariantCulture);

        M = Regex.Match(itemPage, @"High Alchemy</td>\s+<td[^>]+>\s+(\d+)", RegexOptions.Singleline);
        if (M.Success)
          this.HighAlch = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);

        M = Regex.Match(itemPage, @"Low Alchemy</td>\s+<td[^>]+>\s+(\d+)", RegexOptions.Singleline);
        if (M.Success)
          this.LowAlch = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);

        M = Regex.Match(itemPage, @"Market Price</td>\s+<td>Maximum Price</td>\s+</tr>\s+<tr>\s+<td>[^<]+</td>\s+<td>\s+([\d,]+)", RegexOptions.Singleline);
        if (M.Success)
          this.MarketPrice = int.Parse(M.Groups[1].Value.Replace(",", ""), CultureInfo.InvariantCulture);

        M = Regex.Match(itemPage, @"Location</td>\s+<td>([^<]+)</td>", RegexOptions.Singleline);
        if (M.Success)
          this.Location = M.Groups[1].Value.Trim();

      } catch {
      }
    }

  } //class Item
} //namespace BigSister