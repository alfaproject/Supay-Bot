using System;

namespace Supay.Bot
{
  internal static partial class Command
  {
    public static void PriceInfo(CommandContext bc)
    {
      if (bc.MessageTokens.Length == 1)
      {
        bc.SendReply("Syntax: !PriceInfo <item>");
        return;
      }

      Price price;

      int id;
      if (int.TryParse(bc.MessageTokens[1].TrimStart('#'), out id))
      {
        // !PriceInfo <id>
        price = new Price(id);
      }
      else
      {
        string query = bc.MessageTokens.Join(1);
        var prices = new Prices();
        prices.SearchExact(query);

        switch (prices.Count)
        {
          case 0:
            bc.SendReply("\\c12www.runescape.com\\c doesn't have any record for \"{0}\".".FormatWith(query));
            return;
          case 1:
            price = prices[0];
            break;
          default:
            string reply = "Results: \\c07{0}\\c".FormatWith(prices.Count);
            for (int i = 0; i < Math.Min(15, prices.Count); i++)
            {
              reply += " | \\c07#{0}\\c {1}".FormatWith(prices[i].Id, prices[i].Name);
            }
            if (prices.Count > 15)
            {
              reply += " | ...";
            }
            bc.SendReply(reply);
            return;
        }
      }

      price.LoadFromGE();

      string changeToday;
      if (price.ChangeToday < 0) {
        changeToday = @"\c04{0:0.#}\c".FormatWith(price.ChangeToday);
      } else if (price.Change30days > 0) {
        changeToday = @"\c03+{0:0.#}\c".FormatWith(price.ChangeToday);
      } else {
        changeToday = @"\c07{0:0.#}\c".FormatWith(price.ChangeToday);
      }

      string change30days;
      if (price.Change30days < 0)
      {
        change30days = "\\c04{0:0.#}%\\c".FormatWith(price.Change30days);
      }
      else if (price.Change30days > 0)
      {
        change30days = "\\c03+{0:0.#}%\\c".FormatWith(price.Change30days);
      }
      else
      {
        change30days = "\\c07{0:0.#}%\\c".FormatWith(price.Change30days);
      }

      string change90days;
      if (price.Change90days < 0)
      {
        change90days = "\\c04{0:0.#}%\\c".FormatWith(price.Change90days);
      }
      else if (price.Change30days > 0)
      {
        change90days = "\\c03+{0:0.#}%\\c".FormatWith(price.Change90days);
      }
      else
      {
        change90days = "\\c07{0:0.#}%\\c".FormatWith(price.Change90days);
      }

      string change180days;
      if (price.Change180days < 0)
      {
        change180days = "\\c04{0:0.#}%\\c".FormatWith(price.Change180days);
      }
      else if (price.Change30days > 0)
      {
        change180days = "\\c03+{0:0.#}%\\c".FormatWith(price.Change180days);
      }
      else
      {
        change180days = "\\c07{0:0.#}%\\c".FormatWith(price.Change180days);
      }

      bc.SendReply(@"Name: \c07{0}\c | Price: \c07{1}\c | Today's change: {2} | Last 30 days: {3} | Last 90 days: {4} | Last 180 days: {5}".FormatWith(price.Name, price.MarketPrice.ToShortString(1), changeToday, change30days, change90days, change180days));
      bc.SendReply(@"Examine: \c07{0}\c | \c12http://services.runescape.com/m=itemdb_rs/viewitem.ws?obj={1}\c".FormatWith(price.Examine, price.Id));
    }
  }
}
