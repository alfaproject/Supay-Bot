using System;

namespace Supay.Bot {
  internal static partial class Command {
    public static void Alch(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Alch [qty] <item>");
        return;
      }

      int totalHigh = 0,
        totalLow = 0;
      string itemList = string.Empty;

      foreach (string messageToken in bc.Message.Substring(bc.Message.IndexOf(' ') + 1).Split('+')) {
        string[] queryTokens = messageToken.Trim().Split(new[] { ' ' }, 2);

        double queryQty;
        string queryItem;
        if (queryTokens.Length == 2 && MathParser.TryCalc(queryTokens[0], out queryQty)) {
          // <qty> <item>
          queryQty = Math.Max(1.0, Math.Floor(queryQty));
          queryItem = queryTokens[1];
        } else {
          // <item>
          queryQty = 1.0;
          queryItem = queryTokens.Join();
        }

        Item item;

        int itemId;
        if (int.TryParse(queryItem.TrimStart('#'), out itemId)) {
          item = new Item(itemId);
        } else {
          var items = new Items(queryItem);

          switch (items.Count) {
            case 0:
              bc.SendReply(@"\c12www.zybez.net\c doesn't have any record for item ""{0}"".".FormatWith(queryItem));
              return;
            case 1:
              item = items[0];
              item.LoadFromWeb();
              break;
            default:
              string reply = @"\c12www.zybez.net\c has \c07{0}\c items matching ""{1}"":".FormatWith(items.Count, queryItem);
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
          bc.SendReply(@"\c12www.zybez.net\c doesn't have any record for item \c07#{0}\c.".FormatWith(itemId));
          return;
        }
        totalHigh += (int) queryQty * item.HighAlch;
        totalLow += (int) queryQty * item.LowAlch;
        itemList += @"\c07{0:N0}\c {1} + ".FormatWith(queryQty, item.Name);
      }

      bc.SendReply(@"{0} | HighAlch: \c07{1:N0}\c | LowAlch: \c07{2:N0}\c".FormatWith(itemList.Substring(0, itemList.Length - 3), totalHigh, totalLow));
    }
  }
}
