using System;

namespace BigSister {
  static partial class Command {

    public static void Combat(CommandContext bc) {
      // ignore @next
      if (bc.Message.Contains(" @next") || bc.Message.Contains(" @n")) {
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
        rsn = bc.From.Rsn;

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      int AvgSkill = p.Skills[0].Level / (p.Skills.Count - 2);

      int expected_max_slayer_exp = (int)((p.Skills[Skill.HITP].Exp - 1154) * 3 / 4.0);

      int combatLevel, combatF2pLevel;
      string combatClass;
      if (VLevel) {
        combatClass = Utils.CombatClass(p.Skills, true);
        combatLevel = Utils.CalculateCombat(p.Skills, true, false);
        combatF2pLevel = Utils.CalculateCombat(p.Skills, true, true);
      } else {
        combatClass = Utils.CombatClass(p.Skills, false);
        combatLevel = Utils.CalculateCombat(p.Skills, false, false);
        combatF2pLevel = Utils.CalculateCombat(p.Skills, false, true);
      }

      string reply = "\\b{0}\\b \\c07combat\\c | level: \\c07{1}\\c (f2p: \\c07{2}\\c) | exp: \\c07{3:e}\\c | combat%: \\c07{4:0.##}%\\c | slayer%: \\c07{5:0.##}%\\c | class: \\c07{6}\\c".FormatWith(
                                   rsn, combatLevel, combatF2pLevel, p.Skills[Skill.COMB],
                                   (double)p.Skills[Skill.COMB].Exp / (double)p.Skills[Skill.OVER].Exp * 100.0,
                                   (double)p.Skills[Skill.SLAY].Exp / (double)expected_max_slayer_exp * 100.0,
                                   combatClass);

      // Add up SS rank if applicable
      Players ssplayers = new Players("SS");
      if (ssplayers.Contains(rsn)) {
        ssplayers.SortBySkill(Skill.COMB, false);
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
                next = Utils.NextCombatAttStr(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = Utils.NextCombatAttStr(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.DEFE:
            case Skill.HITP:
              if (VLevel)
                next = Utils.NextCombatDefHp(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = Utils.NextCombatDefHp(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.PRAY:
              if (VLevel)
                next = Utils.NextCombatPray(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = Utils.NextCombatPray(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.SUMM:
              if (VLevel)
                next = Utils.NextCombatSum(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = Utils.NextCombatSum(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.MAGI:
              if (VLevel)
                next = Utils.NextCombatMag(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = Utils.NextCombatMag(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
              break;
            case Skill.RANG:
              if (VLevel)
                next = Utils.NextCombatRan(p.Skills[Skill.ATTA].VLevel, p.Skills[Skill.STRE].VLevel, p.Skills[Skill.DEFE].VLevel, p.Skills[Skill.HITP].VLevel, p.Skills[Skill.RANG].VLevel, p.Skills[Skill.PRAY].VLevel, p.Skills[Skill.MAGI].VLevel, p.Skills[Skill.SUMM].VLevel);
              else
                next = Utils.NextCombatRan(p.Skills[Skill.ATTA].Level, p.Skills[Skill.STRE].Level, p.Skills[Skill.DEFE].Level, p.Skills[Skill.HITP].Level, p.Skills[Skill.RANG].Level, p.Skills[Skill.PRAY].Level, p.Skills[Skill.MAGI].Level, p.Skills[Skill.SUMM].Level);
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
          perf = _GetPerformance("Today", p_old.Skills[Skill.COMB], p.Skills[Skill.COMB]);
          if (perf != null)
            reply += perf + " | ";
        }
        p_old = new Player(rsn, lastupdate.AddDays(-((int)lastupdate.DayOfWeek)));
        if (p_old.Ranked) {
          perf = _GetPerformance("Week", p_old.Skills[Skill.COMB], p.Skills[Skill.COMB]);
          if (perf != null)
            reply += perf + " | ";
        }
        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.Day));
        if (p_old.Ranked) {
          perf = _GetPerformance("Month", p_old.Skills[Skill.COMB], p.Skills[Skill.COMB]);
          if (perf != null)
            reply += perf + " | ";
        }
        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.DayOfYear));
        if (p_old.Ranked) {
          perf = _GetPerformance("Year", p_old.Skills[Skill.COMB], p.Skills[Skill.COMB]);
          if (perf != null)
            reply += perf;
        }
        if (reply.Length > 0)
          bc.SendReply(reply.EndsWithI(" | ") ? reply.Substring(0, reply.Length - 3) : reply);
      }
    }

  } //class Command
} //namespace BigSister