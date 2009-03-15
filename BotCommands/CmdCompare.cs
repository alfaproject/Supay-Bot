using System.Globalization;

namespace BigSister {
  static class CmdCompare {

    public static void Compare(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !compare [skill] <player1> [player2]");
        return;
      }

      string skill1 = null;
      string minigame1 = null;
      string rsn1, rsn2;
      if (Skill.TryParse(bc.MessageTokens[1], ref skill1)) {
        if (bc.MessageTokens.Length == 3) {
          // !compare <skill> <player2>
          rsn1 = bc.From.RSN;
          rsn2 = bc.NickToRSN(bc.MessageTokens[2]);
        } else if (bc.MessageTokens.Length > 3) {
          // !compare <skill> <player1> <player2>
          rsn1 = bc.NickToRSN(bc.MessageTokens[2]);
          rsn2 = bc.NickToRSN(bc.MessageTokens.Join(3));
        } else {
          // !compare <player2>
          skill1 = "Overall";
          rsn1 = bc.From.RSN;
          rsn2 = bc.NickToRSN(bc.MessageTokens[1]);
        }
      } else if (Minigame.TryParse(bc.MessageTokens[1], ref minigame1)) {
        if (bc.MessageTokens.Length == 3) {
          // !compare <minigame> <player2>
          rsn1 = bc.From.RSN;
          rsn2 = bc.NickToRSN(bc.MessageTokens[2]);
        } else if (bc.MessageTokens.Length > 3) {
          // !compare <minigame> <player1> <player2>
          rsn1 = bc.NickToRSN(bc.MessageTokens[2]);
          rsn2 = bc.NickToRSN(bc.MessageTokens.Join(3));
        } else {
          // !compare <player2>
          skill1 = "Overall";
          rsn1 = bc.From.RSN;
          rsn2 = bc.NickToRSN(bc.MessageTokens[1]);
        }
      } else if (bc.MessageTokens.Length == 2) {
        // !compare <player2>
        skill1 = "Overall";
        rsn1 = bc.From.RSN;
        rsn2 = bc.NickToRSN(bc.MessageTokens[1]);
      } else {
        // !compare <player1> <player2>
        skill1 = "Overall";
        rsn1 = bc.NickToRSN(bc.MessageTokens[1]);
        rsn2 = bc.NickToRSN(bc.MessageTokens.Join(2));
      }

      Player p1 = new Player(rsn1);
      if (!p1.Ranked) {
        bc.SendReply(string.Format("\\b{0}\\b doesn't feature Hiscores.", rsn1));
        return;
      }

      Player p2 = new Player(rsn2);
      if (!p2.Ranked) {
        bc.SendReply(string.Format("\\b{0}\\b doesn't feature Hiscores.", rsn2));
        return;
      }

      string reply;
      if (minigame1 == null) {
        // compare skills
        Skill pskill1 = p1.Skills[skill1];
        Skill pskill2 = p2.Skills[skill1];

        if (pskill1.Level == pskill2.Level) {
          reply = string.Format("Both \\b{0}\\b and \\b{1}\\b have level \\c07{2}\\c", p1.Name, p2.Name, pskill1.Level);
          if (pskill1.Exp == pskill2.Exp)
            reply += string.Format(CultureInfo.InvariantCulture, " and \\c07{0:e}\\c experience.", pskill1);
          else if (pskill1.Exp > pskill2.Exp)
            reply += string.Format(", but \\b{0}\\b has \\c07{1:N0}\\c more experience.", p1.Name, pskill1.Exp - pskill2.Exp);
          else
            reply += string.Format(", but \\b{0}\\b has \\c07{1:N0}\\c less experience.", p1.Name, pskill2.Exp - pskill1.Exp);
        } else if (pskill1.Level > pskill2.Level) {
          reply = string.Format("\\b{0}\\b has \\c07{2}\\c more level{3} than \\b{1}\\b.", p1.Name, p2.Name, pskill1.Level - pskill2.Level, pskill1.Level - pskill2.Level == 1 ? string.Empty : "s");
          if (pskill1.Exp == pskill2.Exp)
            reply += string.Format(", but both have \\c07{0:e}\\c experience.", pskill1);
          else if (pskill1.Exp > pskill2.Exp)
            reply += string.Format(" and has \\c07{0:N0}\\c more experience.", pskill1.Exp - pskill2.Exp);
          else
            reply += string.Format(", but \\b{0}\\b has \\c07{1:N0}\\c less experience.", p1.Name, pskill2.Exp - pskill1.Exp);
        } else {
          reply = string.Format("\\b{0}\\b has \\c07{2}\\c less level{3} than \\b{1}\\b.", p1.Name, p2.Name, pskill2.Level - pskill1.Level, pskill2.Level - pskill1.Level == 1 ? string.Empty : "s");
          if (pskill1.Exp == pskill2.Exp)
            reply += string.Format(", but both have \\c07{0:e}\\c experience.", pskill1);
          else if (pskill1.Exp > pskill2.Exp)
            reply += string.Format(", but \\b{0}\\b has \\c07{1:N0}\\c more experience.", p1.Name, pskill1.Exp - pskill2.Exp);
          else
            reply += string.Format(" and has \\c07{0:N0}\\c less experience.", pskill2.Exp - pskill1.Exp);
        }
        bc.SendReply(reply);

        // get these players last update time
        string dblastupdate = Database.LastUpdate(rsn1);
        if (dblastupdate != null && dblastupdate.Length == 8) {
          p1 = new Player(rsn1, dblastupdate.ToDateTime());
          if (p1.Ranked) {
            dblastupdate = Database.LastUpdate(rsn2);
            if (dblastupdate != null && dblastupdate.Length == 8) {
              p2 = new Player(rsn2, dblastupdate.ToDateTime());
              if (p2.Ranked) {
                Skill skilldif1 = pskill1 - p1.Skills[skill1];
                Skill skilldif2 = pskill2 - p2.Skills[skill1];
                bc.SendReply(string.Format("Today \\b{0}\\b did \\c07{1:e}\\c exp. while \\b{2}\\b did \\c07{3:e}\\c exp.", rsn1, skilldif1, rsn2, skilldif2));
              }
            }
          }
        }
      } else {
        // compare minigames
        Minigame pminigame1 = p1.Minigames[minigame1];
        Minigame pminigame2 = p2.Minigames[minigame1];

        if (pminigame1.Rank == -1) {
          bc.SendReply(string.Format("\\b{0}\\b doesn't feature Hiscores.", rsn1));
          return;
        }
        if (pminigame2.Rank == -1) {
          bc.SendReply(string.Format("\\b{0}\\b doesn't feature Hiscores.", rsn2));
          return;
        }

        if (pminigame1.Score == pminigame2.Score)
          reply = string.Format("Both \\b{0}\\b and \\b{1}\\b have \\c07{2}\\c score.", rsn1, rsn2, pminigame1.Score);
        else if (pminigame1.Score > pminigame2.Score)
          reply = string.Format("\\b{0}\\b has \\c07{2}\\c more score than \\b{1}\\b.", rsn1, rsn2, pminigame1.Score - pminigame2.Score);
        else
          reply = string.Format("\\b{0}\\b has \\c07{2}\\c less score than \\b{1}\\b.", rsn1, rsn2, pminigame2.Score - pminigame1.Score);
        bc.SendReply(reply);

        // get these players last update time
        string dblastupdate = Database.LastUpdate(rsn1);
        if (dblastupdate != null && dblastupdate.Length == 8) {
          p1 = new Player(rsn1, dblastupdate.ToDateTime());
          if (p1.Ranked && p1.Minigames[minigame1].Rank > 0) {
            dblastupdate = Database.LastUpdate(rsn2);
            if (dblastupdate != null && dblastupdate.Length == 8) {
              p2 = new Player(rsn2, dblastupdate.ToDateTime());
              if (p2.Ranked && p2.Minigames[minigame1].Rank > 0) {
                Minigame minigamedif1 = pminigame1 - p1.Minigames[minigame1];
                Minigame minigamedif2 = pminigame2 - p2.Minigames[minigame1];
                bc.SendReply(string.Format("Today \\b{0}\\b did \\c07{1:s}\\c score while \\b{2}\\b did \\c07{3:s}\\c score.", rsn1, minigamedif1, rsn2, minigamedif2));
              }
            }
          }
        }
      }
    }

  } //class CmdCompare
} //namespace BigSister