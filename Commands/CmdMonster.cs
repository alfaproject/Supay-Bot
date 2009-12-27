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

      Monsters monsters = new Monsters();
      List<Monster> results = monsters.SearchOnline(search_terms);

      if (results.Count > 0) {
        string reply = "\\c12www.tip.it\\c found \\c07{0}\\c results:".FormatWith(results.Count);
        for (int i = 0; i < Math.Min(15, results.Count); i++)
          reply += " \\c07" + results[i].Name + "\\c (" + results[i].Level + ");";
        bc.SendReply(reply);
      } else {
        bc.SendReply("\\c12www.tip.it\\c doesn't have any record for \"{0}\".".FormatWith(search_terms));
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

      Monsters monsters = new Monsters();
      List<Monster> results = monsters.SearchOnline(search_terms);

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
          bc.SendReply("\\c12www.tip.it\\c doesn't have any record for \"{0}\".".FormatWith(search_terms));
        } else {
          monster.Update();
          bc.SendReply("Name: \\c07{0}\\c | Level: \\c07{1}\\c | Hitpoints: \\c07{2}\\c | Race: \\c07{3}\\c | \\c12www.tip.it/runescape/index.php?rs2monster_id={4}\\c".FormatWith(
                                     monster.Name, monster.Level, monster.Hits, monster.Race, monster.ID));
          bc.SendReply("Aggressive? \\c{0}\\c | Retreats? \\c{1}\\c | Quest? \\c{2}\\c | Members? \\c{3}\\c | Poisonous? \\c{4}\\c | Habitat: \\c07{5}\\c".FormatWith(
                                     monster.Aggressive ? "3Yes" : "4No",
                                     monster.Retreats ? "3Yes" : "4No",
                                     monster.Quest ? "3Yes" : "4No",
                                     monster.Members ? "3Yes" : "4No",
                                     monster.Poisonous ? "3Yes" : "4No",
                                     monster.Habitat.Count > 0 ? monster.Habitat[0] : "Unknown"));
        }
      } else {
        bc.SendReply("\\c12www.tip.it\\c doesn't have any record for \"{0}\".".FormatWith(search_terms));
      }
    }

  } //class CmdMonster
} //namespace Supay.Bot