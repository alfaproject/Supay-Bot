using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BigSister {
  
  class CmdZybez {

    public static void ItemInfo(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

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
        string query = Util.JoinTokens(bc.MessageTokens, 1);
        Items items = new Items(query);

        switch (items.Count) {
          case 0:
            bc.SendReply(string.Format("\\c12www.tip.it\\c doesn't have any record for item \"{0}\".", query));
            return;
          case 1:
            item = items[0];
            item.LoadFromWeb();
            break;
          default:
            string reply = string.Format(CultureInfo.InvariantCulture, @"\c12www.tip.it\c has \c07{0}\c items:", items.Count);
            for (int i = 0; i < Math.Min(14, items.Count); i++)
              reply += string.Format(CultureInfo.InvariantCulture, " \\c07#{0}\\c {1};", items[i].Id, items[i].Name);
            if (items.Count > 14)
              reply += " ...";
            bc.SendReply(reply);
            return;
        }
      }

      if (item.Name == null) {
        bc.SendReply(string.Format("\\c12www.tip.it\\c doesn't have any record for item \\c07#{0}\\c.", itemId));
      } else {
        bc.SendReply(string.Format("\\c07{0}\\c | Alch: \\c07{1}/{2}\\c | MarketPrice: \\c07{3}\\c | Location: \\c07{4}\\c | \\c12www.tip.it/runescape/index.php?rs2item_id={5}\\c",
                                   item.Name,
                                   Util.FormatShort(item.HighAlch, 1), Util.FormatShort(item.LowAlch, 1),
                                   Util.FormatShort(item.MarketPrice, 1),
                                   item.Location,
                                   item.Id));
        bc.SendReply(string.Format("Members? \\c{0}\\c | Quest: \\c{1}\\c | Tradeable? \\c{2}\\c | Stackable? \\c{3}\\c | Weight? \\c07{4}Kg\\c | Examine: \\c07{5}\\c",
                                   item.Members ? "3Yes" : "4No",
                                   item.Quest ? "3Yes" : "4No",
                                   item.Tradable ? "3Yes" : "4No",
                                   item.Stackable ? "3Yes" : "4No",
                                   item.Weight, item.Examine));
      }
    }

    public static void HighAlch(Object stateInfo) {
      BotCommand bc = (BotCommand)stateInfo;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Alch [qty] <item>");
        return;
      }

      if (bc.Message.Contains("+")) {
        // MULTIPLE ITEM ALCHEMY
        int totalHigh = 0, totalLow = 0;
        string totalItem = string.Empty;
        bc.Message = bc.Message.Substring(bc.Message.IndexOf(' ') + 1);
        foreach (string token in bc.Message.Split('+')) {
          string[] itemLine = token.Trim().Split(' ');

          double qty;
          string inputItem;
          if (itemLine.Length > 1 && Util.TryCalc(itemLine[0], out qty)) {
            // <qty> <item>
            qty = Math.Max(1, Math.Floor(qty));
            inputItem = Util.JoinTokens(itemLine, 1);
          } else {
            // <item>
            qty = 1;
            inputItem = Util.JoinTokens(itemLine, 0);
          }

          int highAlch, lowAlch;
          string itemName;
          if (_GetAlch(inputItem, out itemName, out highAlch, out lowAlch)) {
            totalHigh += (int)qty * highAlch;
            totalLow += (int)qty * lowAlch;
            totalItem += "\\c07" + qty + "\\c " + itemName + " + ";
          } else {
            bc.SendReply(string.Format("\\c12www.zybez.net\\c doesn't have any record for item \"{0}\". You can use !item to search for correct item names.", inputItem));
            return;
          }
        }
        bc.SendReply(string.Format(CultureInfo.InvariantCulture, "{0} | HighAlch: \\c07{1:N0}\\c | LowAlch: \\c07{2:N0}\\c",
                                   totalItem.Substring(0, totalItem.Length - 3), totalHigh, totalLow));
      } else {
        // SINGLE ITEM ALCHEMY
        double qty;
        string input_item;
        if (bc.MessageTokens.Length > 2 && Util.TryCalc(bc.MessageTokens[1], out qty)) {
          // !alch <qty> <item>
          qty = Math.Max(1, Math.Floor(qty));
          input_item = Util.JoinTokens(bc.MessageTokens, 2);
        } else {
          // !alch <item>
          qty = 1;
          input_item = Util.JoinTokens(bc.MessageTokens, 1);
        }

        int item_id;
        if (int.TryParse(input_item, out item_id)) {
          Item item = new Item(item_id);
          if (item.Name == null)
            bc.SendReply(string.Format("\\c12www.zybez.net\\c doesn't have any record for item \\c07#{0}\\c.", item_id));
          else
            bc.SendReply(string.Format("\\c07{0}\\c | HighAlch: \\c07{1:N0}\\c | LowAlch: \\c07{2:N0}\\c | \\c12www.zybez.net/items.php?id={3}\\c",
                                       item.Name, qty * item.HighAlch, qty * item.LowAlch, item.Id));
          return;
        }

        try {
          string items_page = new System.Net.WebClient().DownloadString("http://www.zybez.net/items.php?search_area=name&search_term=" + input_item);
          Match M = Regex.Match(items_page, "Browsing (\\d+) Items\\(s\\)");
          if (M.Success) {
            // items search page
            MatchCollection items = Regex.Matches(items_page, "id=(\\d+)[^.]+\\.htm\">([^<]+)</a></td>\\s+<td class=\"tablebottom\">", RegexOptions.Singleline);
            if (items.Count == 0) {
              bc.SendReply(string.Format("\\c12www.zybez.net\\c doesn't have any record for item \"{0}\".", input_item));
            } else {
              string reply = string.Format("\\c12www.zybez.net\\c found \\c07{0}\\c results", M.Groups[1].Value);
              for (int i = 0; i < Math.Min(14, items.Count); i++)
                reply += string.Format(" | \\c07#{0}\\c {1}", items[i].Groups[1].Value, items[i].Groups[2].Value);
              if (items.Count > 14)
                reply += " | (...)";
              bc.SendReply(reply);
            }
          }
        } catch {
          bc.SendReply("\\c12www.zybez.net\\c seems to be down.");
        }
      }
    }

    private static bool _GetAlch(string itemName, out string name, out int highAlch, out int lowAlch) {
      name = null;
      highAlch = 0;
      lowAlch = 0;

      string items_page = new System.Net.WebClient().DownloadString("http://www.zybez.net/items.php?search_area=name&search_term=" + itemName);
      Match M = Regex.Match(items_page, "Browsing (\\d+) Items\\(s\\)");
      if (M.Success) {
        // items search page
        MatchCollection items = Regex.Matches(items_page, "id=(\\d+)[^.]+\\.htm\">([^<]+)</a></td>\\s+<td class=\"tablebottom\">", RegexOptions.Singleline);
        if (items.Count == 0) {
          return false;
        } else {
          foreach (Match itemMatch in items) {
            if (itemMatch.Groups[2].Value.ToUpperInvariant() == itemName.ToUpperInvariant()) {
              Item item = new Item(int.Parse(itemMatch.Groups[1].Value));
              name = item.Name;
              highAlch = item.HighAlch;
              lowAlch = item.LowAlch;
              return true;
            }
          }
        }
      }

      return false;
    }

  }
}