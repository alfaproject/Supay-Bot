using System;

namespace Supay.Bot {
  internal static partial class Command {
    public static void Top(CommandContext bc) {
      string rsn;
      string skill = null,
        activity = null;
      int rank;

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
      } else if (Bot.Activity.TryParse(bc.MessageTokens[1], ref activity) || Skill.TryParse(bc.MessageTokens[1], ref skill)) {
        // !Top Skill/Activity
        rank = 1;

        if (bc.MessageTokens.Length > 2) {
          if (int.TryParse(bc.MessageTokens[2], out rank)) {
            // !Top Skill/Activity Rank
          } else {
            // !Top Skill/Activity RSN
            rsn = bc.GetPlayerName(bc.MessageTokens.Join(2));
            var p = new Player(rsn);
            if (p.Ranked) {
              if (skill == null) {
                rank = p.Activities[activity].Rank;
              } else {
                rank = p.Skills[skill].Rank;
              }
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
        rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
        var p = new Player(rsn);
        if (p.Ranked) {
          rank = p.Skills[skill].Rank;
        }
      }
      if (rank < 0) {
        rank = 1;
      }

      var hiscores = new Hiscores(skill, activity, rank);

      string reply = "RS \\u" + hiscores.Name.ToLowerInvariant() + "\\u rankings:";
      if (activity == null) {
        for (int i = 0; i < Math.Min(12, hiscores.Count); i++) {
          reply += " ";
          if (hiscores[i].Rank == rank) {
            reply += "\\b";
          }

          if (level) {
            reply += "\\c07#{0:r}\\c {1} ({0:l})".FormatWith((Skill) hiscores[i], hiscores[i].RSN);
          } else {
            reply += "\\c07#{0:r}\\c {1} ({0:e})".FormatWith((Skill) hiscores[i], hiscores[i].RSN);
          }

          if (hiscores[i].Rank == rank) {
            reply += "\\b";
          }
          reply += ";";
        }
      } else {
        for (int i = 0; i < Math.Min(12, hiscores.Count); i++) {
          reply += " ";
          if (hiscores[i].Rank == rank) {
            reply += "\\b";
          }
          reply += "\\c07#{0}\\c {1} ({2})".FormatWith(hiscores[i].Rank, hiscores[i].RSN, ((Activity) hiscores[i]).Score);
          if (hiscores[i].Rank == rank) {
            reply += "\\b";
          }
          reply += ";";
        }
      }
      bc.SendReply(reply);
    }
  }
}
