using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  static class CmdMonster {

    public static void Search(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !MonsterSearch <search terms>");
        return;
      }

      // get search terms
      string search_terms = bc.MessageTokens.Join(1);

      Monsters results = new Monsters(search_terms);

      if (results.Count > 0) {
        string reply = "\\c12www.zybez.net\\c found \\c07{0}\\c results:".FormatWith(results.Count);
        for (int i = 0; i < Math.Min(15, results.Count); i++)
          reply += " \\c07" + results[i].Name + "\\c (" + results[i].Level + ");";
        bc.SendReply(reply);
      } else {
        bc.SendReply("\\c12www.zybez.net\\c doesn't have any record for \"{0}\".".FormatWith(search_terms));
      }
    }

    public static void Info(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !MonsterInfo <monster>");
        return;
      }

      // get search terms
      string search_terms = bc.MessageTokens.Join(1);

      // get level
      int level = 0;
      Match M = Regex.Match(search_terms, "\\((\\d+)\\)");
      if (M.Success) {
        level = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
        search_terms = Regex.Replace(search_terms, "\\((\\d+)\\)", string.Empty).Trim();
      }

      Monsters results = new Monsters(search_terms);

      if (results.Count > 0) {
        Monster monster = null;
        if (level > 0) {
          // search for exact match at name and level
          foreach (Monster m in results)
            if (m.Name.ToUpperInvariant() == search_terms.ToUpperInvariant() && m.Level == level) {
              monster = m;
              break;
            }
          // search for partial match at name and level
          if (monster == null)
            foreach (Monster m in results)
              if (m.Name.ContainsI(search_terms) && m.Level == level) {
                monster = m;
                break;
              }
        }
        // search for exact match at name
        if (monster == null)
          foreach (Monster m in results)
            if (m.Name.ToUpperInvariant() == search_terms.ToUpperInvariant()) {
              monster = m;
              break;
            }
        // search for partial match at name
        if (monster == null)
          foreach (Monster m in results)
            if (m.Name.ContainsI(search_terms)) {
              monster = m;
              break;
            }

        if (monster == null) {
          bc.SendReply("\\c12www.zybez.net\\c doesn't have any record for \"{0}\".".FormatWith(search_terms));
        } else {
          monster.Update();
          bc.SendReply("Name: \\c07{0}\\c | Level: \\c07{1}\\c | Life points: \\c07{2}\\c | Examine: \\c07{3}\\c | \\c12www.zybez.net/npc.aspx?id={4}\\c".FormatWith(
                                     monster.Name, monster.Level, monster.Hits, monster.Examine, monster.Id));
          bc.SendReply("Aggressive? \\c{0}\\c | Members? \\c{1}\\c | Habitat: \\c07{2}\\c".FormatWith(
                                     monster.Aggressive ? "3Yes" : "4No",
                                     monster.Members ? "3Yes" : "4No",
                                     monster.Habitat));
        }
      } else {
        bc.SendReply("\\c12www.zybez.net\\c doesn't have any record for \"{0}\".".FormatWith(search_terms));
      }
    }

  } //class CmdMonster
} //namespace Supay.Bot