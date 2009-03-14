using System;
using System.Globalization;

namespace BigSister {
  class CmdGrandExchange {

    public static void Price(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Price [qty] <item>");
        return;
      }

      string search_term;
      double qty = 0;
      if (bc.MessageTokens.Length > 2 && Util.TryCalc(bc.MessageTokens[1], out qty)) {
        qty = Math.Round(qty, 1);
        search_term = bc.MessageTokens.Join(2);
      } else {
        qty = 1;
        search_term = bc.MessageTokens.Join(1);
      }

      Prices price_list = new Prices(search_term);
      if (price_list.Count == 0) {
        bc.SendReply(string.Format("\\c12www.runescape.com\\c doesn't have any record for \"{0}\".", search_term));
        return;
      }

      string reply = string.Format("\\c12www.runescape.com\\c found \\c07{0}\\c results", price_list.TotalItems);
      for (int i = 0; i < Math.Min(10, price_list.Count); i++) {
        reply += string.Format(" | {0}: \\c07{1}", price_list[i].Name, (qty * price_list[i].MarketPrice).ToShortString(1));
        reply += "\\c";
        if (price_list[i].ChangeToday > 0)
          reply += string.Format(" \\c3[+{0}]\\c", price_list[i].ChangeToday.ToShortString(1));
        else if (price_list[i].ChangeToday < 0)
          reply += string.Format(" \\c4[{0}]\\c", price_list[i].ChangeToday.ToShortString(1));
      }
      if (price_list.TotalItems > 10)
        reply += " | (...)";
      bc.SendReply(reply);
    }

    public static void PriceInfo(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !PriceInfo <item>");
        return;
      }

      Price price;

      int id;
      if (int.TryParse(bc.MessageTokens[1].TrimStart('#'), out id)) {
        // !PriceInfo <id>
        price = new Price(id);
      } else {
        string query = bc.MessageTokens.Join(1);
        Prices prices = new Prices();
        prices.SearchExact(query);

        switch (prices.Count) {
          case 0:
            bc.SendReply(string.Format("\\c12www.runescape.com\\c doesn't have any record for \"{0}\".", query));
            return;
          case 1:
            price = prices[0];
            break;
          default:
            string reply = string.Format(CultureInfo.InvariantCulture, "Results: \\c07{0}\\c", prices.Count);
            for (int i = 0; i < Math.Min(15, prices.Count); i++)
              reply += string.Format(CultureInfo.InvariantCulture, " | \\c07#{0}\\c {1}", prices[i].Id, prices[i].Name);
            if (prices.Count > 15)
              reply += " | ...";
            bc.SendReply(reply);
            return;
        }
      }

      price.LoadFromGE();

      string change7days;
      if (price.Change7days < 0)
        change7days = string.Format(CultureInfo.InvariantCulture, "\\c04{0:0.#}%\\c", price.Change7days);
      else if (price.Change7days > 0)
        change7days = string.Format(CultureInfo.InvariantCulture, "\\c03+{0:0.#}%\\c", price.Change7days);
      else
        change7days = string.Format(CultureInfo.InvariantCulture, "\\c07{0:0.#}%\\c", price.Change7days);

      string change30days;
      if (price.Change30days < 0)
        change30days = string.Format(CultureInfo.InvariantCulture, "\\c04{0:0.#}%\\c", price.Change30days);
      else if (price.Change30days > 0)
        change30days = string.Format(CultureInfo.InvariantCulture, "\\c03+{0:0.#}%\\c", price.Change30days);
      else
        change30days = string.Format(CultureInfo.InvariantCulture, "\\c07{0:0.#}%\\c", price.Change30days);

      bc.SendReply(string.Format(CultureInfo.InvariantCulture, @"Name: \c07{0}\c | Market price: \c07{1}\c (\c07{2}\c - \c07{3}\c) | Last 7 days: {4} | Last 30 days: {5} | Examine: \c07{6}\c", 
                                 price.Name, price.MarketPrice.ToShortString(1), price.MinimumPrice.ToShortString(1), price.MaximumPrice.ToShortString(1), change7days, change30days, price.Examine));
    }

    public static void LastUpdate(CommandContext bc) {
      DateTime lastUpdate = DataBase.GetString("SELECT lastUpdate FROM prices ORDER BY lastUpdate DESC LIMIT 1;", DateTime.UtcNow.ToString("yyyyMMddHHmm")).ToDateTime();
      bc.SendReply(string.Format("The GE was last updated \\c07{0}\\c ago. ({1:R})", (DateTime.UtcNow - lastUpdate).ToLongString(), lastUpdate));
    }

  } //class CmdGrandExchange
} //namespace BigSister