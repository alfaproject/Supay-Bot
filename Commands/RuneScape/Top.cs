using System;
using System.Globalization;

namespace BigSister {
  static partial class Command {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void Top(CommandContext bc) {
      string rsn = bc.From.Rsn;
      string skill = null, minigame = null;
      int rank = 0;

      // get @level
      bool level = false;
      if (bc.Message.Contains(" @level") || bc.Message.Contains(" @lvl") || bc.Message.Contains(" @l")) {
        level = true;
        bc.Message = bc.Message.Replace(" @level", string.Empty);
        bc.Message = bc.Message.Replace(" @lvl", string.Empty);
        bc.Message = bc.Message.Replace(" @l", string.Empty);
      }

      // Parse command arguments
      if (bc.MessageTokens.Length == 1) {
        // !Top
        rank = 1;
        skill = Skill.OVER;
      } else if (BigSister.Minigame.TryParse(bc.MessageTokens[1], ref minigame) || Skill.TryParse(bc.MessageTokens[1], ref skill)) {
        // !Top Skill/Minigame
        rank = 1;

        if (bc.MessageTokens.Length > 2) {
          if (int.TryParse(bc.MessageTokens[2], out rank)) {
            // !Top Skill/Minigame Rank
          } else {
            // !Top Skill/Minigame RSN
            rsn = bc.NickToRSN(bc.MessageTokens.Join(2));
            Player p = new Player(rsn);
            if (p.Ranked) {
              if (skill == null)
                rank = p.Minigames[minigame].Rank;
              else
                rank = p.Skills[skill].Rank;
            }
          }
        }
      } else if (int.TryParse(bc.MessageTokens[1], out rank)) {
        // !Top Rank
        skill = Skill.OVER;
      } else {
        // !Top RSN
        rank = 1;
        skill = Skill.OVER;
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
        Player p = new Player(rsn);
        if (p.Ranked) {
          if (skill == null)
            rank = p.Minigames[minigame].Rank;
          else
            rank = p.Skills[skill].Rank;
        }
      }
      if (rank < 0)
        rank = 1;

      Hiscores hiscores = new Hiscores(skill, minigame, rank);

      string reply = "RS \\u" + hiscores.Name.ToLowerInvariant() + "\\u rankings:";
      if (minigame == null) {
        for (int i = 0; i < Math.Min(12, hiscores.Count); i++) {
          reply += " ";
          if (hiscores[i].Rank == rank)
            reply += "\\b";

          if (level)
            reply += "\\c07#{0:r}\\c {1} ({0:l})".FormatWith((Skill)hiscores[i], hiscores[i].RSN);
          else
            reply += "\\c07#{0:r}\\c {1} ({0:e})".FormatWith((Skill)hiscores[i], hiscores[i].RSN);

          if (hiscores[i].Rank == rank)
            reply += "\\b";
          reply += ";";
        }
      } else {
        for (int i = 0; i < Math.Min(12, hiscores.Count); i++) {
          reply += " ";
          if (hiscores[i].Rank == rank)
            reply += "\\b";
          reply += "\\c07#{0}\\c {1} ({2})".FormatWith(hiscores[i].Rank, hiscores[i].RSN, ((Minigame)hiscores[i]).Score);
          if (hiscores[i].Rank == rank)
            reply += "\\b";
          reply += ";";
        }
      }
      bc.SendReply(reply);
    }

  } //class Command
} //namespace BigSister