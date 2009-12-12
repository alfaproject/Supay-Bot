using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  static partial class Command {

    public static void Stats(CommandContext bc) {
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
        rsn = bc.From.Rsn;

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
        double AvgSkilldouble = Math.Round((double)totalLevel / (double)(p.Skills.Count - 2), 1);
        if (Exp) {
          AvgSkilldouble = (double)((int)((double)p.Skills[0].Exp / (double)(p.Skills.Count - 2))).ToLevel();
        }

        string reply = "\\b{0}\\b \\c07overall\\c | level: \\c07{1:N0}\\c (\\c07{2}\\c avg.) | exp: \\c07{3:e}\\c (\\c07{4}%\\c of {5}) | rank: \\c07{3:R}\\c".FormatWith(
                                     rsn,
                                     totalLevel,
                                     AvgSkilldouble,
                                     p.Skills[0],
                                     Math.Round((double)oa_exp / (13034431 * (p.Skills.Count - 2)) * 100.0, 1),
                                     (p.Skills.Count - 2) * 99);

        int AvgSkill = (int)AvgSkilldouble;

        // add up SS rank if applicable
        Players ssplayers = new Players("SS");
        if (ssplayers.Contains(p.Name)) {
          ssplayers.SortBySkill(Skill.OVER, false);
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

  } //class Command
} ////namespace Supay.Bot