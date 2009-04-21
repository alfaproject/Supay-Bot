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
        string itemName, itemList;
        
        if (int.TryParse(inputItem, out itemID)) {
          Item item = new Item(itemID);
          if (item.Name == null) {
            bc.SendReply("\\c12www.tip.it\\c doesn't have any record for item \\c07#{0}\\c.".FormatWith(itemID));
            return;
          }
          totalHigh += (int)qty * item.HighAlch;
          totalLow += (int)qty * item.LowAlch;
          totalItem += "\\c07" + qty + "\\c " + item.Name + " + ";
        } else if (_GetAlch(inputItem, out itemName, out highAlch, out lowAlch, out itemList)) {
            totalHigh += (int)qty * highAlch;
            totalLow += (int)qty * lowAlch;
            totalItem += "\\c07" + qty + "\\c " + itemName + " + ";
        } else {
          if (itemList != string.Empty) { bc.SendReply("\\c12www.tip.it\\c found more than one result for \"{0}\": {1}".FormatWith(inputItem, itemList)); return; }
          bc.SendReply("\\c12www.tip.it\\c doesn't have any record for item \"{0}\". You can use !item to search for correct item names.".FormatWith(inputItem));
          return;
        }
      }
      bc.SendReply("{0} | HighAlch: \\c07{1:N0}\\c | LowAlch: \\c07{2:N0}\\c".FormatWith(
                                 totalItem.Substring(0, totalItem.Length - 3), totalHigh, totalLow));
    }

    private static bool _GetAlch(string itemName, out string name, out int highAlch, out int lowAlch, out string itemList) {
      name = null;
      highAlch = 0;
      lowAlch = 0;
      itemList = string.Empty;
      try {
        string items_page = new System.Net.WebClient().DownloadString("http://www.tip.it/runescape/index.php?rs2item=&orderby=0&keywords={0}&Players=all&category=0&subcategory=0&cmd=8&action=Manage_Items&search=1&submit=Simple+Search".FormatWith(itemName));
        Match M = Regex.Match(items_page, "<strong>(\\d+)<\\/strong> items matched your search.");
        if (M.Groups[1].Value.ToInt32() > 0) {
          // items search page
          MatchCollection items = Regex.Matches(items_page, "item_id=(\\d+)\">([^<]+)<", RegexOptions.Singleline);
          if (items.Count == 0) {
            return false;
          } else if (items.Count == 1) {
            Item item = new Item(int.Parse(items[0].Groups[1].Value, CultureInfo.InvariantCulture));
            name = item.Name;
            highAlch = item.HighAlch;
            lowAlch = item.LowAlch;
            return true;
          } else {
            foreach (Match itemMatch in items) {
              if (itemList.Length < 300) {
                itemList += "\\c07#{0}\\c {1}; ".FormatWith(itemMatch.Groups[1].Value, itemMatch.Groups[2].Value);
              }
            }
          }
        }
        itemList = itemList.Substring(0, itemList.Length - 1);
      } catch {
      }
      return false;
    }

  } //class Command
} //namespace BigSister