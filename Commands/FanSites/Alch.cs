using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BigSister {
  static partial class Command {

    public static void Alch(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Alch [qty] <item>");
        return;
      }

      int totalHigh = 0, totalLow = 0;
      string totalItem = string.Empty;
      bc.Message = bc.Message.Substring(bc.Message.IndexOf(' ') + 1);
      foreach (string token in bc.Message.Split('+')) {
        string[] itemLine = token.Trim().Split(' ');

        double qty;
        string inputItem;
        if (itemLine.Length > 1 && MathParser.TryCalc(itemLine[0], out qty)) {
          // <qty> <item>
          qty = Math.Max(1, Math.Floor(qty));
          inputItem = itemLine.Join(1);
        } else {
          // <item>
          qty = 1;
          inputItem = itemLine.Join();
        }

        int highAlch, lowAlch, itemID;
        string itemName;
        if (_GetAlch(inputItem, out itemName, out highAlch, out lowAlch)) {
          totalHigh += (int)qty * highAlch;
          totalLow += (int)qty * lowAlch;
          totalItem += "\\c07" + qty + "\\c " + itemName + " + ";
        } else if (int.TryParse(inputItem, out itemID)) {
          Item item = new Item(itemID);
          if (item.Name == null) {
            bc.SendReply("\\c12www.tip.it\\c doesn't have any record for item \\c07#{0}\\c.".FormatWith(itemID));
            return;
          }
          totalHigh += (int)qty * item.HighAlch;
          totalLow += (int)qty * item.LowAlch;
          totalItem += "\\c07" + qty + "\\c " + item.Name + " + ";
        } else {
          bc.SendReply("\\c12www.tip.it\\c doesn't have any record for item \"{0}\". You can use !item to search for correct item names.".FormatWith(inputItem));
        }
      }
      bc.SendReply("{0} | HighAlch: \\c07{1:N0}\\c | LowAlch: \\c07{2:N0}\\c".FormatWith(
                                 totalItem.Substring(0, totalItem.Length - 3), totalHigh, totalLow));
    }

    private static bool _GetAlch(string itemName, out string name, out int highAlch, out int lowAlch) {
      name = null;
      highAlch = 0;
      lowAlch = 0;
      string items_page = new System.Net.WebClient().DownloadString("http://www.tip.it/runescape/index.php?rs2item=&orderby=0&keywords={0}&Players=all&category=0&subcategory=0&cmd=8&action=Manage_Items&search=1&submit=Simple+Search".FormatWith(itemName));
      Match M = Regex.Match(items_page, "<strong>(\\d+)<\\/strong> items matched your search.");
      if (M.Groups[1].Value.ToInt32() > 0) {
        // items search page
        MatchCollection items = Regex.Matches(items_page, "item_id=(\\d+)\">([^<]+)<", RegexOptions.Singleline);
        if (items.Count == 0) {
          return false;
        } else {
          foreach (Match itemMatch in items) {
            if (itemMatch.Groups[2].Value.ToUpperInvariant() == itemName.ToUpperInvariant()) {
              Item item = new Item(int.Parse(itemMatch.Groups[1].Value, CultureInfo.InvariantCulture));
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

  } //class Command
} //namespace BigSister