﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BigSister {
  static class CmdZybez {

    public static void ItemInfo(CommandContext bc) {
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
            bc.SendReply("\\c12www.tip.it\\c doesn't have any record for item \"{0}\".".FormatWith(query));
            return;
          case 1:
            item = items[0];
            item.LoadFromWeb();
            break;
          default:
            string reply = @"\c12www.tip.it\c has \c07{0}\c items:".FormatWith(items.Count);
            for (int i = 0; i < Math.Min(14, items.Count); i++)
              reply += " \\c07#{0}\\c {1};".FormatWith(items[i].Id, items[i].Name);
            if (items.Count > 14)
              reply += " ...";
            bc.SendReply(reply);
            return;
        }
      }

      if (item.Name == null) {
        bc.SendReply("\\c12www.tip.it\\c doesn't have any record for item \\c07#{0}\\c.".FormatWith(itemId));
      } else {
        bc.SendReply("\\c07{0}\\c | Alch: \\c07{1}/{2}\\c | MarketPrice: \\c07{3}\\c | Location: \\c07{4}\\c | \\c12www.tip.it/runescape/index.php?rs2item_id={5}\\c".FormatWith(
                                   item.Name,
                                   item.HighAlch.ToShortString(1), item.LowAlch.ToShortString(1),
                                   item.MarketPrice.ToShortString(1),
                                   item.Location,
                                   item.Id));
        bc.SendReply("Members? \\c{0}\\c | Quest: \\c{1}\\c | Tradeable? \\c{2}\\c | Stackable? \\c{3}\\c | Weight? \\c07{4}Kg\\c | Examine: \\c07{5}\\c".FormatWith(
                                   item.Members ? "3Yes" : "4No",
                                   item.Quest ? "3Yes" : "4No",
                                   item.Tradable ? "3Yes" : "4No",
                                   item.Stackable ? "3Yes" : "4No",
                                   item.Weight, item.Examine));
      }
    }

    public static void HighAlch(CommandContext bc) {
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
          if (itemLine.Length > 1 && MathParser.TryCalc(itemLine[0], out qty)) {
            // <qty> <item>
            qty = Math.Max(1, Math.Floor(qty));
            inputItem = itemLine.Join(1);
          } else {
            // <item>
            qty = 1;
            inputItem = itemLine.Join();
          }

          int highAlch, lowAlch;
          string itemName;
          if (_GetAlch(inputItem, out itemName, out highAlch, out lowAlch)) {
            totalHigh += (int)qty * highAlch;
            totalLow += (int)qty * lowAlch;
            totalItem += "\\c07" + qty + "\\c " + itemName + " + ";
          } else {
            bc.SendReply("\\c12www.zybez.net\\c doesn't have any record for item \"{0}\". You can use !item to search for correct item names.".FormatWith(inputItem));
            return;
          }
        }
        bc.SendReply("{0} | HighAlch: \\c07{1:N0}\\c | LowAlch: \\c07{2:N0}\\c".FormatWith(
                                   totalItem.Substring(0, totalItem.Length - 3), totalHigh, totalLow));
      } else {
        // SINGLE ITEM ALCHEMY
        double qty;
        string input_item;
        if (bc.MessageTokens.Length > 2 && MathParser.TryCalc(bc.MessageTokens[1], out qty)) {
          // !alch <qty> <item>
          qty = Math.Max(1, Math.Floor(qty));
          input_item = bc.MessageTokens.Join(2);
        } else {
          // !alch <item>
          qty = 1;
          input_item = bc.MessageTokens.Join(1);
        }

        int item_id;
        if (int.TryParse(input_item, out item_id)) {
          Item item = new Item(item_id);
          if (item.Name == null)
            bc.SendReply("\\c12www.zybez.net\\c doesn't have any record for item \\c07#{0}\\c.".FormatWith(item_id));
          else
            bc.SendReply("\\c07{0}\\c | HighAlch: \\c07{1:N0}\\c | LowAlch: \\c07{2:N0}\\c | \\c12www.zybez.net/items.php?id={3}\\c".FormatWith(
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
              bc.SendReply("\\c12www.zybez.net\\c doesn't have any record for item \"{0}\".".FormatWith(input_item));
            } else {
              string reply = "\\c12www.zybez.net\\c found \\c07{0}\\c results".FormatWith(M.Groups[1].Value);
              for (int i = 0; i < Math.Min(14, items.Count); i++)
                reply += " | \\c07#{0}\\c {1}".FormatWith(items[i].Groups[1].Value, items[i].Groups[2].Value);
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

  } //class CmdZybez
} //namespace BigSister