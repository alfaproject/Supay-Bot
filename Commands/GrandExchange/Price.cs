using System;

namespace Supay.Bot {
  static partial class Command {

    public static void Price(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Price [qty] <item>");
        return;
      }

      string search_term;
      double qty = 0;
      if (bc.MessageTokens.Length > 2 && MathParser.TryCalc(bc.MessageTokens[1], out qty)) {
        qty = Math.Round(qty, 1);
        search_term = bc.MessageTokens.Join(2);
      } else {
        qty = 1;
        search_term = bc.MessageTokens.Join(1);
      }

      Prices price_list = new Prices(search_term);
      if (price_list.Count == 0) {
        bc.SendReply("\\c12www.runescape.com\\c doesn't have any record for \"{0}\".".FormatWith(search_term));
        return;
      }

      string reply = "\\c12www.runescape.com\\c found \\c07{0}\\c results".FormatWith(price_list.TotalItems);
      for (int i = 0; i < Math.Min(10, price_list.Count); i++) {
        reply += " | {0}: \\c07{1}".FormatWith(price_list[i].Name, (qty * price_list[i].MarketPrice).ToShortString(1));
        reply += "\\c";
        if (price_list[i].ChangeToday > 0)
          reply += " \\c3[+{0}]\\c".FormatWith(price_list[i].ChangeToday.ToShortString(1));
        else if (price_list[i].ChangeToday < 0)
          reply += " \\c4[{0}]\\c".FormatWith(price_list[i].ChangeToday.ToShortString(1));
      }
      if (price_list.TotalItems > 10)
        reply += " | (...)";
      bc.SendReply(reply);
    }

  } //class Command
} //namespace Supay.Bot