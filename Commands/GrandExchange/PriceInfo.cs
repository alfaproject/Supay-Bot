using System;

namespace BigSister {
  static partial class Command {

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
            bc.SendReply("\\c12www.runescape.com\\c doesn't have any record for \"{0}\".".FormatWith(query));
            return;
          case 1:
            price = prices[0];
            break;
          default:
            string reply = "Results: \\c07{0}\\c".FormatWith(prices.Count);
            for (int i = 0; i < Math.Min(15, prices.Count); i++)
              reply += " | \\c07#{0}\\c {1}".FormatWith(prices[i].Id, prices[i].Name);
            if (prices.Count > 15)
              reply += " | ...";
            bc.SendReply(reply);
            return;
        }
      }

      price.LoadFromGE();

      string change7days;
      if (price.Change7days < 0)
        change7days = "\\c04{0:0.#}%\\c".FormatWith(price.Change7days);
      else if (price.Change7days > 0)
        change7days = "\\c03+{0:0.#}%\\c".FormatWith(price.Change7days);
      else
        change7days = "\\c07{0:0.#}%\\c".FormatWith(price.Change7days);

      string change30days;
      if (price.Change30days < 0)
        change30days = "\\c04{0:0.#}%\\c".FormatWith(price.Change30days);
      else if (price.Change30days > 0)
        change30days = "\\c03+{0:0.#}%\\c".FormatWith(price.Change30days);
      else
        change30days = "\\c07{0:0.#}%\\c".FormatWith(price.Change30days);

      bc.SendReply(@"Name: \c07{0}\c | Market price: \c07{1}\c (\c07{2}\c - \c07{3}\c) | Last 7 days: {4} | Last 30 days: {5} | Examine: \c07{6}\c".FormatWith(
                                 price.Name, price.MarketPrice.ToShortString(1), price.MinimumPrice.ToShortString(1), price.MaximumPrice.ToShortString(1), change7days, change30days, price.Examine));
    }

  } //class Command
} //namespace BigSister