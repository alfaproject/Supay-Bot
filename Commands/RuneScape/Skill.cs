using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BigSister {
  static partial class Command {

    public static void SkillInfo(CommandContext bc) {
      // get skill name
      string skillName = Skill.Parse(bc.MessageTokens[0]);

      // get goal
      string goal;
      Match M = Regex.Match(bc.Message, @"(?:#|goal=)(\d+|nl|nr|r\d+)");
      if (M.Success) {
        goal = M.Groups[1].Value;
        bc.Message = Regex.Replace(bc.Message, @"(?:#|goal=)" + goal, string.Empty);
        bc.Message = Regex.Replace(bc.Message, @"\s+", " ").TrimEnd();
        Database.SetStringParam("users", "goals", "fingerprint='" + bc.From.FingerPrint + "'", skillName, goal);
      } else {
        goal = Database.GetStringParam("users", "goals", "fingerprint='" + bc.From.FingerPrint + "'", skillName, "nl");
      }

      // get item
      string item;
      M = Regex.Match(bc.Message, "(?:@|§)(.+)");
      if (M.Success) {
        item = M.Groups[1].Value;
        bc.Message = Regex.Replace(bc.Message, "(?:@|§)" + item, string.Empty);
        bc.Message = Regex.Replace(bc.Message, @"\s+", " ").TrimEnd();
        Database.SetStringParam("users", "items", "fingerprint='" + bc.From.FingerPrint + "'", skillName, item);
      } else {
        item = Database.GetStringParam("users", "items", "fingerprint='" + bc.From.FingerPrint + "'", skillName, null);
      }

      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
      else
        rsn = bc.From.Rsn;

      Player p = new Player(rsn);
      if (p.Ranked) {
        Skill skill = p.Skills[skillName];

        // parse goal
        int target_level = 0;
        int target_exp = 0;
        if (int.TryParse(goal, out target_level)) {
          // get level/exp
          if (target_level == 127) {
            target_level = 126;
            target_exp = 200000000;
          } else if (target_level > 127) {
            target_exp = Math.Min(200000000, target_level);
            target_level = target_exp.ToLevel();
          } else {
            target_exp = target_level.ToExp();
          }
        } else if (goal.StartsWithI("r")) {
          // get rank
          int goalrank;
          if (int.TryParse(goal.Substring(1), out goalrank)) {
            if (goalrank > 0 && goalrank < skill.Rank) {
              foreach (Skill h in new Hiscores(skill.Name, null, goalrank))
                if (h.Rank == goalrank) {
                  target_exp = h.Exp;
                  target_level = target_exp.ToLevel();
                  break;
                }
            }
          }
        } else if (goal == "nr") {
          // get next rank
          if (skill.Rank > 1) {
            foreach (Skill h in new Hiscores(skill.Name, null, skill.Rank - 1))
              if (h.Rank == skill.Rank - 1) {
                target_exp = h.Exp;
                break;
              }
          } else {
            target_exp = Math.Min(200000000, skill.Exp + 1);
          }
          target_level = target_exp.ToLevel();
        } else {
          // next level
          if (skill.VLevel == 126) {
            target_level = 126;
            target_exp = 200000000;
          } else {
            target_level = skill.VLevel + 1;
            target_exp = target_level.ToExp();
          }
        }
        if (target_exp < skill.Exp) {
          target_level = skill.VLevel + 1;
          target_exp = target_level.ToExp();
        }

        // calculate % done
        int expToGo = 0;
        string percent_done;
        if (skill.Name == Skill.OVER) {
          int oa_exp = 0;
          foreach (Skill s in p.Skills.Values)
            if (s.Name != Skill.OVER && s.Name != Skill.COMB)
              oa_exp += Math.Min(13034431, s.Exp);
          target_level = (p.Skills.Count - 2) * 99;
          int max_exp = 13034431 * (p.Skills.Count - 2);
          percent_done = Math.Round(oa_exp / (double)max_exp * 100.0, 1).ToStringI();

          item = null;
        } else {
          expToGo = target_exp - skill.Exp;
          percent_done = Math.Round(100 - expToGo / (double)(target_exp - skill.VLevel.ToExp()) * 100, 1).ToStringI();
        }

        string reply = "\\b{0}\\b \\c07{1}\\c | level: \\c07{1:v}\\c | exp: \\c07{1:e}\\c (\\c07{2}%\\c of {3}) | rank: \\c07{1:R}\\c".FormatWith(
                                     rsn, skill, percent_done, target_level);

        // Add up SS rank if applicable
        Players ssplayers = new Players("SS");
        if (ssplayers.Contains(p.Name)) {
          ssplayers.SortBySkill(skill.Name, false);
          reply += " (SS rank: \\c07{0}\\c)".FormatWith(ssplayers.IndexOf(rsn) + 1);
        }

        // Add exp to go and items
        if (expToGo > 0) {
          reply += " | \\c07{0:N0}\\c exp. to go".FormatWith(expToGo);

          int speed = int.Parse(Database.GetStringParam("users", "speeds", "fingerprint='" + bc.From.FingerPrint + "'", skillName, "0"), CultureInfo.InvariantCulture);
          if (speed > 0) {
            reply += @" (\c07{0}\c)".FormatWith(TimeSpan.FromHours((double)expToGo / (double)speed).ToLongString());
          }

          if (item != null && item.Length > 0) {
            string item_name;
            int monster_hp;

            switch (skill.Name) {
              case Skill.ATTA:
              case Skill.DEFE:
              case Skill.STRE:
              case Skill.RANG:
                if (_GetMonster(item, out item_name, out monster_hp))
                  reply += " (\\c07{0}\\c {1})".FormatWith(Math.Ceiling((double)expToGo / (monster_hp * 4)), item_name);
                else
                  reply += " (unknown monster)";
                break;
              case Skill.HITP:
                if (_GetMonster(item, out item_name, out monster_hp))
                  reply += " (\\c07{0}\\c {1})".FormatWith(Math.Ceiling((double)expToGo / (monster_hp * (4 / 3))), item_name);
                else
                  reply += " (unknown monster)";
                break;
              case Skill.SLAY:
                if (_GetMonster(item, out item_name, out monster_hp))
                  reply += " (\\c07{0}\\c {1})".FormatWith(Math.Ceiling((double)expToGo / monster_hp), item_name);
                else
                  reply += " (unknown monster)";
                break;
              default:
                SkillItem itemFound = _GetItem(skill.Name, item);
                if (itemFound != null)
                  reply += " (\\c07{1}\\c \\c{0}{2}\\c)".FormatWith(itemFound.IrcColour, Math.Ceiling(expToGo / itemFound.Exp), itemFound.Name);
                else
                  reply += " (unknown item)";
                break;
            }
          }
        }

        bc.SendReply(reply);

        // Show player performance if applicable
        DateTime lastupdate;
        string dblastupdate = Database.LastUpdate(rsn);
        if (dblastupdate == null || dblastupdate.Length < 8) {
          lastupdate = DateTime.Now.AddHours(-DateTime.Now.Hour + 6).AddMinutes(-DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second);
          if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
            lastupdate = lastupdate.AddDays(-1);
        } else {
          lastupdate = dblastupdate.ToDateTime();
        }

        string perf;
        reply = string.Empty;

        Player p_old = new Player(rsn, lastupdate);
        if (!p_old.Ranked)
          p_old = new Player(rsn, (int)(DateTime.Now - lastupdate).TotalSeconds);
        if (p_old.Ranked) {
          perf = _GetPerformance("Today", p_old.Skills[skill.Name], skill);
          if (perf != null)
            reply += perf + " | ";
        }

        p_old = new Player(rsn, lastupdate.AddDays(-((int)lastupdate.DayOfWeek)));
        if (!p_old.Ranked)
          p_old = new Player(rsn, (int)(DateTime.Now - lastupdate.AddDays(-((int)lastupdate.DayOfWeek))).TotalSeconds);
        if (p_old.Ranked) {
          perf = _GetPerformance("Week", p_old.Skills[skill.Name], skill);
          if (perf != null)
            reply += perf + " | ";
        }

        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.Day));
        if (!p_old.Ranked)
          p_old = new Player(rsn, (int)(DateTime.Now - lastupdate.AddDays(1 - lastupdate.Day)).TotalSeconds);
        if (p_old.Ranked) {
          perf = _GetPerformance("Month", p_old.Skills[skill.Name], skill);
          if (perf != null)
            reply += perf + " | ";
        }

        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.DayOfYear));
        if (!p_old.Ranked)
          p_old = new Player(rsn, (int)(DateTime.Now - lastupdate.AddDays(1 - lastupdate.DayOfYear)).TotalSeconds);
        if (p_old.Ranked) {
          perf = _GetPerformance("Year", p_old.Skills[skill.Name], skill);
          if (perf != null)
            reply += perf + " | ";
        }

        // ***** start war *****
        SQLiteDataReader warPlayer = Database.ExecuteReader("SELECT startrank, startlevel, startexp FROM warplayers WHERE channel='" + bc.Channel + "' AND rsn='" + rsn + "';");
        if (warPlayer.Read() && Database.GetString("SELECT skill FROM wars WHERE channel='" + bc.Channel + "';", null) == skill.Name) {
          Skill oldSkill = new Skill(skill.Name, warPlayer.GetInt32(0), warPlayer.GetInt32(1), warPlayer.GetInt32(2));
          perf = _GetPerformance("War", oldSkill, skill);
          if (perf != null)
            reply += perf;
        }
        // ***** end war *****

        if (reply.Length > 0)
          bc.SendReply(reply.EndsWithI(" | ") ? reply.Substring(0, reply.Length - 3) : reply);

        return;
      }
      bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
    }

    private static SkillItem _GetItem(string skill, string input_item) {
      // Load items data file
      SkillItems items = new SkillItems(skill);

      // Search for an exact match
      SkillItem item = items.Find(f => f.Name.ToUpperInvariant() == input_item.ToUpperInvariant());
      // Search for a partial match if the exact fails
      if (item == null)
        item = items.Find(f => f.Name.ContainsI(input_item));

      return item;
    }

    private static bool _GetMonster(string input_monster, out string monster_name, out int monster_hp) {
      // get level
      int level = 0;
      Match M = Regex.Match(input_monster, "\\((\\d+)\\)");
      if (M.Success) {
        level = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
        input_monster = Regex.Replace(input_monster, "\\((\\d+)\\)", string.Empty).Trim();
      }

      Monsters monsters = new Monsters();
      List<Monster> results = monsters.SearchOnline(input_monster);

      if (results.Count > 0) {
        Monster monster = null;
        if (level > 0) {
          // search for exact match at name and level
          foreach (Monster m in results)
            if (m.Name.ToUpperInvariant() == input_monster.ToUpperInvariant() && m.Level == level) {
              monster = m;
              break;
            }
          // search for partial match at name and level
          if (monster == null)
            foreach (Monster m in results)
              if (m.Name.ContainsI(input_monster) && m.Level == level) {
                monster = m;
                break;
              }
        }
        // search for exact match at name
        if (monster == null)
          foreach (Monster m in results)
            if (m.Name.ToUpperInvariant() == input_monster.ToUpperInvariant()) {
              monster = m;
              break;
            }
        // search for partial match at name
        if (monster == null)
          foreach (Monster m in results)
            if (m.Name.ContainsI(input_monster)) {
              monster = m;
              break;
            }

        if (monster != null) {
          monster.Update();
          monster_name = monster.Name;
          monster_hp = monster.Hits;
          return true;
        }
      }

      monster_name = null;
      monster_hp = 0;
      return false;
    }

    private static string _GetPerformance(string interval, Skill skillold, Skill skillnew) {
      Skill skilldif = skillnew - skillold;
      if (skilldif.Exp > 0 || skilldif.Level > 0 || skilldif.Rank != 0) {
        string result = "\\u" + interval + ":\\u ";

        if (skilldif.Exp > 0)
          result += "\\c03" + skilldif.Exp.ToShortString(1) + "\\c xp, ";

        if (skilldif.Level > 0)
          result += "\\c03" + skilldif.Level + "\\c level" + (skilldif.Level > 1 ? "s" : string.Empty) + ", ";

        if (skilldif.Rank > 0)
          result += "\\c03+" + skilldif.Rank + "\\c rank" + (skilldif.Rank > 1 ? "s" : string.Empty);
        else if (skilldif.Rank < 0)
          result += "\\c04" + skilldif.Rank + "\\c rank" + (skilldif.Rank < 1 ? "s" : string.Empty);

        return (result.EndsWithI(", ") ? result.Substring(0, result.Length - 2) : result);
      }
      return null;
    }

  } //class Command
} //namespace BigSister