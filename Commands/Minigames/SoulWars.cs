namespace BigSister {
  static partial class Command {

    public static void SoulWars(CommandContext bc) {
      // get rsn
      string rsn = string.Empty;
      string skill = string.Empty;
      int level = 0;
      if (bc.MessageTokens.Length == 2) {
        rsn = bc.From.Rsn;
        Skill.TryParse(bc.MessageTokens[1], ref skill);
      } else if (bc.MessageTokens.Length == 3) {
        if (int.TryParse(bc.MessageTokens[1], out level)) {
          Skill.TryParse(bc.MessageTokens[2], ref skill);
        } else if (int.TryParse(bc.MessageTokens[2], out level)) {
          Skill.TryParse(bc.MessageTokens[1], ref skill);
        } else {
          if (Skill.TryParse(bc.MessageTokens[1], ref skill)) {
            rsn = bc.NickToRSN(bc.MessageTokens[2]);
          } else if (Skill.TryParse(bc.MessageTokens[2], ref skill)) {
            rsn = bc.NickToRSN(bc.MessageTokens[1]);
          }
        }
      } else if (bc.MessageTokens.Length > 3) {
        if (Skill.TryParse(bc.MessageTokens[1], ref skill)) {
          rsn = bc.NickToRSN(bc.MessageTokens.Join(2).Trim());
        } else if (Skill.TryParse(bc.MessageTokens[bc.MessageTokens.Length - 1], ref skill)) {
          bc.MessageTokens[bc.MessageTokens.Length - 1] = string.Empty;
          rsn = bc.NickToRSN(bc.MessageTokens.Join(1).Trim());
        }
      }
      if (skill == string.Empty || (rsn == string.Empty && level == 0)) {
        bc.SendReply("Syntax: !soulwars <level> <skill>");
        return;
      }

      if (rsn != string.Empty) {
        Player p = new Player(rsn);
        if (!p.Ranked) {
          bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
          return;
        }
        Skill skillInfo = p.Skills[skill];
        level = skillInfo.Level;
      }

      int points = 0;
      if (skill == "Attack" || skill == "Strength" || skill == "Defence" || skill == "Hitpoints") {
        points = (int)(level * level / 600) * 525;
      } else if (skill == "Ranged" || skill == "Magic") {
        points = (int)(level * level / 600) * 480;
      } else if (skill == "Prayer") {
        points = (int)(level * level / 600) * 270;
      } else {
        // Slayer formula
      }

      bc.SendReply("For each point at level \\c07" + level + "\\c you will gain \\c07" + points.ToStringI("#,##0") + " " + skill + "\\c experience");
    }

  } //class Command
} //namespace BigSister