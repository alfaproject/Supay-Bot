using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BigSister {
  static class CmdRuneScape {

    public static void Stats(CommandContext bc) {
      // get @next
      bool ExpNext = false;
      if (bc.Message.Contains(" @next") || bc.Message.Contains(" @n")) {
        ExpNext = true;
        bc.Message = bc.Message.Replace(" @next", string.Empty);
        bc.Message = bc.Message.Replace(" @n", string.Empty);
      }

      // get @rank
      bool Exp = false;
      if (bc.Message.Contains(" @exp") || bc.Message.Contains(" @xp")) {
        Exp = true;
        bc.Message = bc.Message.Replace(" @exp", string.Empty);
        bc.Message = bc.Message.Replace(" @xp", string.Empty);
      }

      // get @exp
      bool Rank = false;
      if (bc.Message.Contains(" @rank") || bc.Message.Contains(" @r")) {
        Rank = true;
        bc.Message = bc.Message.Replace(" @rank", string.Empty);
        bc.Message = bc.Message.Replace(" @r", string.Empty);
      }

      // get @vlevel
      bool VLevel = false;
      if (bc.Message.Contains(" @vlevel") || bc.Message.Contains(" @vlvl") || bc.Message.Contains(" @v")) {
        VLevel = true;
        bc.Message = bc.Message.Replace(" @vlevel", string.Empty);
        bc.Message = bc.Message.Replace(" @vlvl", string.Empty);
        bc.Message = bc.Message.Replace(" @v", string.Empty);
      }

      // get <
      int InputLessThan = 0;
      Match M = Regex.Match(bc.Message, " <(\\d+m|\\d+k|\\d+)");
      if (M.Success) {
        if (M.Groups[1].Value.EndsWithI("m"))
          InputLessThan = 1000000 * int.Parse(M.Groups[1].Value.Substring(0, M.Groups[1].Value.Length - 1), CultureInfo.InvariantCulture);
        else if (M.Groups[1].Value.EndsWithI("k"))
          InputLessThan = 1000 * int.Parse(M.Groups[1].Value.Substring(0, M.Groups[1].Value.Length - 1), CultureInfo.InvariantCulture);
        else
          InputLessThan = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
        if (InputLessThan < 127)
          InputLessThan = InputLessThan.ToExp();
        bc.Message = Regex.Replace(bc.Message, " <(\\d+m|\\d+k|\\d+)", string.Empty);
      }

      // get >
      int InputGreaterThan = 0;
      M = Regex.Match(bc.Message, " >(\\d+m|\\d+k|\\d+)");
      if (M.Success) {
        if (M.Groups[1].Value.EndsWithI("m"))
          InputGreaterThan = 1000000 * int.Parse(M.Groups[1].Value.Substring(0, M.Groups[1].Value.Length - 1), CultureInfo.InvariantCulture);
        else if (M.Groups[1].Value.EndsWithI("k"))
          InputGreaterThan = 1000 * int.Parse(M.Groups[1].Value.Substring(0, M.Groups[1].Value.Length - 1), CultureInfo.InvariantCulture);
        else
          InputGreaterThan = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
        if (InputGreaterThan < 127)
          InputGreaterThan = InputGreaterThan.ToExp();
        bc.Message = Regex.Replace(bc.Message, " >(\\d+m|\\d+k|\\d+)", string.Empty);
      }

      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
      else
        rsn = bc.From.RSN;

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      if (ExpNext) {
        string reply = string.Empty;
        List<Skill> skills = p.Skills.SortedByExpToNextVLevel;
        for (int i = 0; i < skills.Count; i++) {
          if (i == 0 || i == 15)
            reply = "Exp. to next level of \\b{0}\\b:".FormatWith(rsn);
          reply += " \\c{0}{1:N0}\\c {2};".FormatWith((skills[i].VLevel > 98 ? "04" : "03"), skills[i].ExpToVLevel, skills[i].Name);
          if (i == 14 || i == skills.Count - 1)
            bc.SendReply(reply);
        }
      } else {
        // calculate "real" overall xp
        int oa_exp = 0;
        for (int i = 1; i < p.Skills.Count - 1; i++)
          oa_exp += Math.Min(13034431, p.Skills[i].Exp);

        // calculate total level and average level
        int totalLevel = p.Skills[0].Level;
        if (VLevel) {
          totalLevel = 0;
          for (int i = 1; i < p.Skills.Count - 1; i++)
            totalLevel += p.Skills[i].VLevel;
        }

        int AvgSkill = totalLevel / (p.Skills.Count - 2);

        string reply = "\\b{0}\\b \\c07overall\\c | level: \\c07{1:N0}\\c (\\c07{2}\\c avg.) | exp: \\c07{3:e}\\c (\\c07{4}%\\c of {5}) | rank: \\c07{3:R}\\c".FormatWith(
                                     rsn,
                                     totalLevel,
                                     Math.Round((double)totalLevel / (p.Skills.Count - 2), 1),
                                     p.Skills[0],
                                     Math.Round((double)oa_exp / (13034431 * (p.Skills.Count - 2)) * 100.0, 1),
                                     (p.Skills.Count - 2) * 99);

        // add up SS rank if applicable
        Players ssplayers = new Players("SS");
        if (ssplayers.Contains(p.Name)) {
          ssplayers.SortBySkill("Overall", false);
          reply += " (SS rank: \\c07{0}\\c)".FormatWith(ssplayers.IndexOf(rsn) + 1);
        }

        bc.SendReply(reply);

        string format;
        if (Exp)
          format = " {2}\\c{1:00}{0:re}\\c {0:n}{2};";
        else if (Rank)
          format = " {2}\\c{1:00}{0:r}\\c {0:n}{2};";
        else if (VLevel)
          format = " {2}\\c{1:00}{0:rv}\\c {0:n}{2};";
        else
          format = " {2}\\c{1:00}{0:rl}\\c {0:n}{2};";

        string replyCombat = "\\uCombat skills\\u:";
        string replyOther = "\\uOther skills\\u:";
        for (int i = 1; i < p.Skills.Count - 1; i++) {
          Skill s = p.Skills[i];

          if (InputLessThan > 0 && s.Exp >= InputLessThan)
            continue;
          if (InputGreaterThan > 0 && s.Exp <= InputGreaterThan)
            continue;
          if (InputLessThan > 0 && InputGreaterThan > 0 && s.Exp >= InputLessThan && s.Exp <= InputGreaterThan)
            continue;

          reply = format.FormatWith(
            s,
            (VLevel ? s.VLevel : s.Level) > AvgSkill + 7 ? 3 : ((VLevel ? s.VLevel : s.Level) < AvgSkill - 7 ? 4 : 7),
            s.Exp == p.Skills.Highest[0].Exp ? "\\u" : string.Empty);

          if (s.Name != Skill.ATTA && s.Name != Skill.STRE && s.Name != Skill.DEFE && s.Name != Skill.HITP && s.Name != Skill.PRAY && s.Name != Skill.SUMM && s.Name != Skill.RANG && s.Name != Skill.MAGI)
            replyOther += reply;
          else
            replyCombat += reply;
        }
        bc.SendReply((replyCombat + format.Substring(0, format.Length - 1) + " (\\c07{3}\\c)").FormatWith(p.Skills[Skill.COMB], 7, string.Empty, p.CombatClass));
        bc.SendReply(replyOther);

        bool ranked = false;
        reply = "\\uMinigames\\u:";
        foreach (Minigame m in p.Minigames.Values) {
          if (m.Rank > 0) {
            ranked = true;
            reply += " \\c07" + (Rank ? m.Rank : m.Score) + "\\c " + m.Name + ";";
          }
        }
        if (ranked)
          bc.SendReply(reply);

      }
    }

    public static void Minigame(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
      else
        rsn = bc.From.RSN;

      Player p = new Player(rsn);
      if (p.Ranked) {
        Minigame minigame = p.Minigames[BigSister.Minigame.Parse(bc.MessageTokens[0])];
        if (minigame.Rank > 0) {
          string reply = "\\b{0}\\b \\c07{1:n}\\c | score: \\c07{1:s}\\c | rank: \\c07{1:R}\\c".FormatWith(rsn, minigame);

          // Add up SS rank if applicable
          Players ssplayers = new Players("SS");
          if (ssplayers.Contains(p.Name)) {
            ssplayers.SortByMinigame(minigame.Name);
            reply += " (SS rank: \\c07{0}\\c)".FormatWith(ssplayers.IndexOf(rsn) + 1);
          }

          bc.SendReply(reply);

          // Show player performance if applicable
          string dblastupdate = Database.LastUpdate(rsn);
          if (dblastupdate != null && dblastupdate.Length == 8) {
            DateTime lastupdate = dblastupdate.ToDateTime();
            string perf;
            reply = string.Empty;

            Player p_old = new Player(rsn, lastupdate);
            if (p_old.Ranked) {
              perf = GetPerformance("Today", p_old.Minigames[minigame.Name], minigame);
              if (perf != null)
                reply += perf + " | ";
            }
            p_old = new Player(rsn, lastupdate.AddDays(-((int)lastupdate.DayOfWeek)));
            if (p_old.Ranked) {
              perf = GetPerformance("Week", p_old.Minigames[minigame.Name], minigame);
              if (perf != null)
                reply += perf + " | ";
            }
            p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.Day));
            if (p_old.Ranked) {
              perf = GetPerformance("Month", p_old.Minigames[minigame.Name], minigame);
              if (perf != null)
                reply += perf + " | ";
            }
            p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.DayOfYear));
            if (p_old.Ranked) {
              perf = GetPerformance("Year", p_old.Minigames[minigame.Name], minigame);
              if (perf != null)
                reply += perf;
            }
            if (reply.Length > 0)
              bc.SendReply(reply.EndsWithI(" | ") ? reply.Substring(0, reply.Length - 3) : reply);
          }

          return;
        }
      }
      bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
    }

    public static void SkillInfo(CommandContext bc) {
      // get goal
      string goal = "nl";
      Match M = Regex.Match(bc.Message, " (#|goal=)([\\dnlr]+)( |$)");
      if (M.Success) {
        goal = M.Groups[2].Value;
        bc.Message = Regex.Replace(bc.Message, " (#|goal=)[\\dnlr]+$?", string.Empty);
      }

      // get item
      string item = null;
      M = Regex.Match(bc.Message, " (@|§)(.+)");
      if (M.Success) {
        item = M.Groups[2].Value;
        bc.Message = Regex.Replace(bc.Message, " (@|§).+", string.Empty);
      }

      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
      else
        rsn = bc.From.RSN;

      Player p = new Player(rsn);
      if (p.Ranked) {
        Skill skill = p.Skills[BigSister.Skill.Parse(bc.MessageTokens[0])];

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
        int exp_to_go = 0;
        string percent_done;
        if (skill.Name == "Overall") {
          int oa_exp = 0;
          foreach (Skill s in p.Skills.Values)
            if (s.Name != "Overall" && s.Name != "Combat")
              oa_exp += Math.Min(13034431, s.Exp);
          target_level = (p.Skills.Count - 2) * 99;
          int max_exp = 13034431 * (p.Skills.Count - 2);
          percent_done = Math.Round(oa_exp / (double)max_exp * 100.0, 1).ToStringI();

          item = null;
        } else {
          exp_to_go = target_exp - skill.Exp;
          percent_done = Math.Round(100 - exp_to_go / (double)(target_exp - skill.VLevel.ToExp()) * 100, 1).ToStringI();
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
        if (exp_to_go > 0) {
          reply += " | \\c07{0:N0}\\c exp. to go".FormatWith(exp_to_go);

          if (item != null && item.Length > 0) {
            string item_name;
            int monster_hp;

            switch (skill.Name) {
              case "Attack":
              case "Defence":
              case "Strength":
              case "Ranged":
                if (_GetMonster(item, out item_name, out monster_hp))
                  reply += " (\\c07{0}\\c {1})".FormatWith(Math.Ceiling((double)exp_to_go / (monster_hp * 4)), item_name);
                else
                  reply += " (unknown monster)";
                break;
              case "Hitpoints":
                if (_GetMonster(item, out item_name, out monster_hp))
                  reply += " (\\c07{0}\\c {1})".FormatWith(Math.Ceiling((double)exp_to_go / (monster_hp * (4 / 3))), item_name);
                else
                  reply += " (unknown monster)";
                break;
              case "Slayer":
                if (_GetMonster(item, out item_name, out monster_hp))
                  reply += " (\\c07{0}\\c {1})".FormatWith(Math.Ceiling((double)exp_to_go / monster_hp), item_name);
                else
                  reply += " (unknown monster)";
                break;
              default:
                ASkillItem itemFound = _GetItem(skill.Name, item);
                if (itemFound != null)
                  reply += " (\\c07{1}\\c \\c{0}{2}\\c)".FormatWith(itemFound.IrcColour, Math.Ceiling(exp_to_go / itemFound.Exp), itemFound.Name);
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
          perf = GetPerformance("Today", p_old.Skills[skill.Name], skill);
          if (perf != null)
            reply += perf + " | ";
        }

        p_old = new Player(rsn, lastupdate.AddDays(-((int)lastupdate.DayOfWeek)));
        if (!p_old.Ranked)
          p_old = new Player(rsn, (int)(DateTime.Now - lastupdate.AddDays(-((int)lastupdate.DayOfWeek))).TotalSeconds);
        if (p_old.Ranked) {
          perf = GetPerformance("Week", p_old.Skills[skill.Name], skill);
          if (perf != null)
            reply += perf + " | ";
        }

        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.Day));
        if (!p_old.Ranked)
          p_old = new Player(rsn, (int)(DateTime.Now - lastupdate.AddDays(1 - lastupdate.Day)).TotalSeconds);
        if (p_old.Ranked) {
          perf = GetPerformance("Month", p_old.Skills[skill.Name], skill);
          if (perf != null)
            reply += perf + " | ";
        }

        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.DayOfYear));
        if (!p_old.Ranked)
          p_old = new Player(rsn, (int)(DateTime.Now - lastupdate.AddDays(1 - lastupdate.DayOfYear)).TotalSeconds);
        if (p_old.Ranked) {
          perf = GetPerformance("Year", p_old.Skills[skill.Name], skill);
          if (perf != null)
            reply += perf + " | ";
        }

        // ***** start war *****
        XmlProfile _config = new XmlProfile("Data\\War.xml");
        _config.RootName = bc.Channel.Substring(1);

        if (_config.GetValue("Setup", "Skill", "Overall") == skill.Name && _config.HasEntry(rsn, "StartExp")) {
          Skill oldskill = new Skill(_config.GetValue("Setup", "Skill", "Overall"), _config.GetValue(rsn, "StartRank", -1), _config.GetValue(rsn, "StartLevel", 1), _config.GetValue(rsn, "StartExp", 0));
          perf = GetPerformance("War", oldskill, skill);
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

    private static ASkillItem _GetItem(string skill, string input_item) {
      // Load items data file
      SkillItems items = new SkillItems(skill);

      // Search for an exact match
      ASkillItem item = items.Find(f => f.Name.ToUpperInvariant() == input_item.ToUpperInvariant());
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

    public static void Combat(CommandContext bc) {
      // get @next
      bool ExpNext = false;
      if (bc.Message.Contains(" @next") || bc.Message.Contains(" @n")) {
        ExpNext = true;
        bc.Message = bc.Message.Replace(" @next", string.Empty);
        bc.Message = bc.Message.Replace(" @n", string.Empty);
      }

      // get @exp
      bool Exp = false;
      if (bc.Message.Contains(" @exp") || bc.Message.Contains(" @xp")) {
        Exp = true;
        bc.Message = bc.Message.Replace(" @exp", string.Empty);
        bc.Message = bc.Message.Replace(" @xp", string.Empty);
      }

      // get @rank
      bool Rank = false;
      if (bc.Message.Contains(" @rank") || bc.Message.Contains(" @r")) {
        Rank = true;
        bc.Message = bc.Message.Replace(" @rank", string.Empty);
        bc.Message = bc.Message.Replace(" @r", string.Empty);
      }

      // get @vlevel
      bool VLevel = false;
      if (bc.Message.Contains(" @vlevel") || bc.Message.Contains(" @vlvl") || bc.Message.Contains(" @v")) {
        VLevel = true;
        bc.Message = bc.Message.Replace(" @vlevel", string.Empty);
        bc.Message = bc.Message.Replace(" @vlvl", string.Empty);
        bc.Message = bc.Message.Replace(" @v", string.Empty);
      }

      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
      else
        rsn = bc.From.RSN;

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      int AvgSkill = p.Skills[0].Level / (p.Skills.Count - 2);

      int expected_max_slayer_exp = (int)((p.Skills["Hitpoints"].Exp - 1154) * 3 / 4.0);

      int combatLevel, combatF2pLevel;
      string combatClass;
      if (VLevel) {
        combatClass = RSUtil.CombatClass(p.Skills, true);
        combatLevel = RSUtil.CalculateCombat(p.Skills, true, false);
        combatF2pLevel = RSUtil.CalculateCombat(p.Skills, true, true);
      } else {
        combatClass = RSUtil.CombatClass(p.Skills, false);
        combatLevel = RSUtil.CalculateCombat(p.Skills, false, false);
        combatF2pLevel = RSUtil.CalculateCombat(p.Skills, false, true);
      }

      string reply = "\\b{0}\\b \\c07combat\\c | level: \\c07{1}\\c (f2p: \\c07{2}\\c) | exp: \\c07{3:e}\\c | combat%: \\c07{4:0.##}%\\c | slayer%: \\c07{5:0.##}%\\c | class: \\c07{6}\\c".FormatWith(
                                   rsn, combatLevel, combatF2pLevel, p.Skills[Skill.COMB],
                                   (double)p.Skills[Skill.COMB].Exp / (double)p.Skills[Skill.OVER].Exp * 100.0,
                                   (double)p.Skills[Skill.SLAY].Exp / (double)expected_max_slayer_exp * 100.0,
                                   combatClass);

      // Add up SS rank if applicable
      Players ssplayers = new Players("SS");
      if (ssplayers.Contains(rsn)) {
        ssplayers.SortBySkill("Combat", false);
        reply += " | SS rank: \\c07{0}\\c".FormatWith(ssplayers.IndexOf(rsn) + 1);
      }

      bc.SendReply(reply);

      string format;
      if (Exp)
        format = "\\c{1:00}{0:re}";
      else if (Rank)
        format = "\\c{1:00}{0:r}";
      else if (VLevel)
        format = "\\c{1:00}{0:rv}";
      else
        format = "\\c{1:00}{0:rl}";

      int next;

      if (Rank)
        reply = "\\uSkills\\u:";
      else
        reply = "\\uSkills (to level)\\u:";
      for (int i = 1; i < p.Skills.Count - 1; i++) {
        Skill s = p.Skills[i];

        if (s.Name != Skill.ATTA && s.Name != Skill.STRE && s.Name != Skill.DEFE && s.Name != Skill.HITP && s.Name != Skill.PRAY && s.Name != Skill.SUMM && s.Name != Skill.RANG && s.Name != Skill.MAGI)
          continue;

        reply += " ";
        if (s.Exp == p.Skills.Highest[0].Exp)
          reply += "\\u";

        reply += format.FormatWith(
          s, (VLevel ? s.VLevel : s.Level) > AvgSkill + 7 ? 3 : ((VLevel ? s.VLevel : s.Level) < AvgSkill - 7 ? 4 : 7));

        if (!Rank) {
          switch (s.Name) {
            case Skill.ATTA:
            case Skill.STRE:
              if (VLevel)
                next = RSUtil.NextCombatAttStr(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = RSUtil.NextCombatAttStr(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.DEFE:
            case Skill.HITP:
              if (VLevel)
                next = RSUtil.NextCombatDefHp(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = RSUtil.NextCombatDefHp(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.PRAY:
              if (VLevel)
                next = RSUtil.NextCombatPray(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = RSUtil.NextCombatPray(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.SUMM:
              if (VLevel)
                next = RSUtil.NextCombatSum(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = RSUtil.NextCombatSum(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.MAGI:
              if (VLevel)
                next = RSUtil.NextCombatMag(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = RSUtil.NextCombatMag(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.RANG:
              if (VLevel)
                next = RSUtil.NextCombatRan(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = RSUtil.NextCombatRan(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            default:
              next = 0;
              break;
          }
          if (next > 0)
            reply += "(+" + next + ")";
        }

        reply += "\\c ";

        reply += s.Name;
        if (s.Exp == p.Skills.Highest[0].Exp)
          reply += "\\u";

        reply += ";";
      }
      bc.SendReply(reply);

      // Show player performance if applicable
      string dblastupdate = Database.LastUpdate(rsn);
      if (dblastupdate != null && dblastupdate.Length == 8) {
        DateTime lastupdate = dblastupdate.ToDateTime();
        string perf;
        reply = string.Empty;

        Player p_old = new Player(rsn, lastupdate);
        if (p_old.Ranked) {
          perf = GetPerformance("Today", p_old.Skills["Combat"], p.Skills["Combat"]);
          if (perf != null)
            reply += perf + " | ";
        }
        p_old = new Player(rsn, lastupdate.AddDays(-((int)lastupdate.DayOfWeek)));
        if (p_old.Ranked) {
          perf = GetPerformance("Week", p_old.Skills["Combat"], p.Skills["Combat"]);
          if (perf != null)
            reply += perf + " | ";
        }
        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.Day));
        if (p_old.Ranked) {
          perf = GetPerformance("Month", p_old.Skills["Combat"], p.Skills["Combat"]);
          if (perf != null)
            reply += perf + " | ";
        }
        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.DayOfYear));
        if (p_old.Ranked) {
          perf = GetPerformance("Year", p_old.Skills["Combat"], p.Skills["Combat"]);
          if (perf != null)
            reply += perf;
        }
        if (reply.Length > 0)
          bc.SendReply(reply.EndsWithI(" | ") ? reply.Substring(0, reply.Length - 3) : reply);
      }
    }

    private static string GetPerformance(string interval, Skill skillold, Skill skillnew) {
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

    private static string GetPerformance(string interval, Minigame mg_old, Minigame mg_new) {
      Minigame mg_dif = mg_new - mg_old;
      if (mg_dif.Score > 0 || mg_dif.Rank != 0) {
        string result = "\\u" + interval + ":\\u ";

        if (mg_dif.Score > 0)
          result += "\\c03" + mg_dif.Score + "\\c score, ";

        if (mg_dif.Rank > 0)
          result += "\\c03+" + mg_dif.Rank + "\\c rank" + (mg_dif.Rank > 1 ? "s" : string.Empty) + ";";
        else if (mg_dif.Rank < 0)
          result += "\\c04" + mg_dif.Rank + "\\c rank" + (mg_dif.Rank < 1 ? "s" : string.Empty) + ";";

        return (result.EndsWithI(", ") ? result.Substring(0, result.Length - 2) + ";" : result);
      }
      return null;
    }

  } //class CmdRuneScape
} //namespace BigSister