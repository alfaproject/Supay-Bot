using System;

namespace Supay.Bot {
  static partial class Command {

    public static void Item(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Item <item>");
        return;
      }

      Item item;

      int itemId;
      if (int.TryParse(bc.MessageTokens[1].TrimStart('#'), out itemId)) {
        // !Item <id>
        item = new Item(itemId);
      } else {
        string query = bc.MessageTokens.Join(1);
        Items items = new Items(query);

        switch (items.Count) {
          case 0:
            bc.SendReply(@"\c12www.tip.it\c doesn't have any record for item ""{0}"".".FormatWith(query));
            return;
          case 1:
            item = items[0];
            item.LoadFromWeb();
            break;
          default:
            string reply = @"\c12www.tip.it\c has \c07{0}\c items:".FormatWith(items.Count);
            for (int i = 0; i < Math.Min(14, items.Count); i++) {
              reply += @" \c07#{0}\c {1};".FormatWith(items[i].Id, items[i].Name);
            }
            if (items.Count > 14) {
              reply += " ...";
            }
            bc.SendReply(reply);
            return;
        }
      }

      if (item.Name == null) {
        bc.SendReply(@"\c12www.tip.it\c doesn't have any record for item \c07#{0}\c.".FormatWith(itemId));
      } else {
        bc.SendReply(@"\c07{0}\c | Alch: \c07{1}/{2}\c | MarketPrice: \c07{3}\c | Location: \c07{4}\c | \c12www.tip.it/runescape/index.php?rs2item_id={5}\c".FormatWith(
                     item.Name,
                     item.HighAlch.ToShortString(1), item.LowAlch.ToShortString(1),
                     item.MarketPrice.ToShortString(1),
                     item.Location, item.Id));
        bc.SendReply(@"Members? \c{0}\c | Quest: \c{1}\c | Tradeable? \c{2}\c | Stackable? \c{3}\c | Weight? \c07{4}Kg\c | Examine: \c07{5}\c".FormatWith(
                     item.Members ? "3Yes" : "4No",
                     item.Quest ? "3Yes" : "4No",
                     item.Tradable ? "3Yes" : "4No",
                     item.Stackable ? "3Yes" : "4No",
                     item.Weight, item.Examine));
      }
    }

  } //class Command
} //namespace Supay.Bot