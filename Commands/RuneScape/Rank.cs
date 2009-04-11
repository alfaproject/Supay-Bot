namespace BigSister {
  static partial class Command {

    public static void Rank(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        // !rank
        bc.SendReply("Syntax: !rank <skill/minigame> <rank>");
        return;
      }

      int rank = 1;
      string skill = null, minigame = null;

      if (bc.MessageTokens.Length > 1) {
        if (int.TryParse(bc.MessageTokens[1], out rank)) {
          if (bc.MessageTokens.Length > 2 && (Skill.TryParse(bc.MessageTokens[2], ref skill) || BigSister.Minigame.TryParse(bc.MessageTokens[2], ref minigame))) {
            // !rank <rank> <skill/minigame>
          } else {
            // !rank <rank>
            skill = Skill.OVER;
          }
        } else if (Skill.TryParse(bc.MessageTokens[1], ref skill) || BigSister.Minigame.TryParse(bc.MessageTokens[1], ref minigame)) {
          if (bc.MessageTokens.Length > 2 && int.TryParse(bc.MessageTokens[2], out rank)) {
            // !rank <skill/minigame> <rank>
          } else {
            // !rank <skill/minigame>
            rank = 1;
          }
        } else {
          bc.SendReply("Syntax: !rank <skill/minigame> <rank>");
          return;
        }
      }

      // get the rsn for this rank
      string rsn = null;
      foreach (Hiscore h in new Hiscores(skill, minigame, rank))
        if (h.Rank == rank) {
          rsn = h.RSN;
          break;
        }

      if (rsn == null) {
        bc.SendReply("\\u{0}\\u hiscores don't have rank \\c07{1}\\c.".FormatWith(skill == null ? minigame : skill, rank));
        return;
      }

      // redirect command to the apropriate sections
      if (minigame == null) {
        bc.Message = skill + " " + rsn;
        Command.SkillInfo(bc);
      } else {
        bc.Message = minigame.Replace(" ", string.Empty) + " " + rsn;
        Command.Minigame(bc);
      }
    }

  } //class Command
} //namespace BigSister