using System;
using System.Globalization;
using System.Text.RegularExpressions;


namespace BigSister {
  class CmdClan {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void Top(CommandContext bc) {
      string clanInitials;
      string clanName;
      if (bc.Message.ToUpperInvariant().Contains("SS")) {
        clanInitials = "SS";
        clanName = "Supreme Skillers";
      } else if (bc.Message.ToUpperInvariant().Contains("TS")) {
        clanInitials = "TS";
        clanName = "True Skillers";
      } else {
        clanInitials = "PT";
        clanName = "Portugal";
      }

      string rsn = bc.From.RSN;
      string skill = null, minigame = null;
      int rank = 0;
      bool IsIndividual = false;

      // get @exp
      bool exp = false;
      if (bc.Message.Contains(" @exp") || bc.Message.Contains(" @xp")) {
        exp = true;
        bc.Message = bc.Message.Replace(" @exp", string.Empty);
        bc.Message = bc.Message.Replace(" @xp", string.Empty);
      }

      // Create a list of Clan players
      Players clanPlayers = new Players(clanInitials);

      // Parse command arguments
      if (bc.MessageTokens.Length == 1) {
        // !ClanTop
        IsIndividual = true;
      } else if (Minigame.TryParse(bc.MessageTokens[1], ref minigame) || Skill.TryParse(bc.MessageTokens[1], ref skill)) {
        // !ClanTop Skill/Minigame
        rank = 1;

        // Clean and sort clan members by specified skill
        if (minigame == null) {
          clanPlayers.RemoveAll(p => !p.Ranked || p.Skills[skill].Exp == 0);
          clanPlayers.SortBySkill(skill, exp);
        } else {
          clanPlayers.RemoveAll(p => !p.Ranked || p.Minigames[minigame].Score == 0);
          clanPlayers.SortByMinigame(minigame);
        }

        if (bc.MessageTokens.Length > 2) {
          if (int.TryParse(bc.MessageTokens[2], out rank)) {
            // !ClanTop Skill/Minigame Rank
          } else if (bc.MessageTokens.Length == 3 && bc.MessageTokens[2].ToUpperInvariant() == "@LAST") {
            // !ClanTop Skill/Minigame @last
            rank = clanPlayers.Count;
          } else {
            // !ClanTop Skill/Minigame RSN
            rsn = bc.NickToRSN(bc.MessageTokens.Join(2));
            if (clanPlayers.Contains(rsn))
              rank = clanPlayers.IndexOf(rsn) + 1;
          }
        }
      } else {
        // !ClanTop RSN
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
        IsIndividual = true;
      }

      if (IsIndividual) {
        Player p = clanPlayers.Find(rsn);
        if (p != null) {
          // individual skill ranks
          string reply = string.Format(CultureInfo.InvariantCulture, "[{0}] \\b{1}\\b skill ranks:", clanInitials, rsn);
          foreach (Skill s in p.Skills.Values) {
            if (s.Exp > 0) {
              clanPlayers.SortBySkill(s.Name, false);
              reply += " \\c07#" + (clanPlayers.IndexOf(p) + 1) + "\\c " + s.ShortName + ";";
            }
          }
          bc.SendReply(reply);

          // individual minigame ranks
          bool ranked = false;
          reply = string.Format(CultureInfo.InvariantCulture, "[{0}] \\b{1}\\b minigame ranks:", clanInitials, rsn);
          foreach (Minigame mg in p.Minigames.Values) {
            if (mg.Score > 0) {
              ranked = true;
              clanPlayers.SortByMinigame(mg.Name);
              reply += " \\c07#" + (clanPlayers.IndexOf(p) + 1) + "\\c " + mg.Name + ";";
            }
          }
          if (ranked)
            bc.SendReply(reply);
        } else {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, "\\b{0}\\b isn't at {1}.", rsn, clanName));
        }
      } else {
        // Get input player rank
        int input_player_rank = 0;
        if (clanPlayers.Contains(bc.From.RSN))
          input_player_rank = clanPlayers.IndexOf(bc.From.RSN) + 1;

        // fix rank
        if (rank < 1)
          rank = 1;
        else if (rank > clanPlayers.Count)
          rank = clanPlayers.Count;

        int MinRank;
        MinRank = rank - 6;
        if (MinRank < 0)
          MinRank = 0;
        else if (MinRank > clanPlayers.Count - 11)
          MinRank = clanPlayers.Count - 11;

        string skillFormat = "l";
        if (exp)
          skillFormat = "e";

        if (minigame == null) {
          if (clanPlayers.Count > 0) {
            string reply = "[" + clanInitials + "] \\u" + skill.ToLowerInvariant() + "\\u ranking:";
            if (input_player_rank > 0 && input_player_rank <= MinRank)
              reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Skills[skill].ToString(skillFormat, CultureInfo.InvariantCulture) + ");";

            for (int i = MinRank; i < Math.Min(MinRank + 11, clanPlayers.Count); i++) {
              reply += " ";
              if (i == rank - 1)
                reply += "\\b";
              reply += "\\c07#" + (i + 1) + "\\c ";
              if (i == input_player_rank - 1)
                reply += "\\u";
              reply += clanPlayers[i].Name;
              if (i == input_player_rank - 1)
                reply += "\\u";

              reply += " (" + clanPlayers[i].Skills[skill].ToString(skillFormat, CultureInfo.InvariantCulture) + ")";

              if (i == rank - 1)
                reply += "\\b";
              reply += ";";
            }

            if (input_player_rank > 0 && input_player_rank > MinRank + 11)
              reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Skills[skill].ToString(skillFormat, CultureInfo.InvariantCulture) + ");";

            bc.SendReply(reply);
          } else {
            bc.SendReply(clanName + " don't have any member ranked at this skill.");
          }
        } else {
          if (clanPlayers.Count > 0) {
            string reply = "[" + clanInitials + "] \\u" + minigame.ToLowerInvariant() + "\\u ranking:";
            if (input_player_rank > 0 && input_player_rank <= MinRank)
              reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Minigames[minigame].Score + ");";

            for (int i = MinRank; i < Math.Min(MinRank + 11, clanPlayers.Count); i++) {
              reply += " ";
              if (i == rank - 1)
                reply += "\\b";
              reply += "\\c07#" + (i + 1) + "\\c ";
              if (i == input_player_rank - 1)
                reply += "\\u";
              reply += clanPlayers[i].Name;
              if (i == input_player_rank - 1)
                reply += "\\u";
              reply += " (" + clanPlayers[i].Minigames[minigame].Score + ")";
              if (i == rank - 1)
                reply += "\\b";
              reply += ";";
            }

            if (input_player_rank > 0 && input_player_rank > MinRank + 11)
              reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Minigames[minigame].Score + ");";

            bc.SendReply(reply);
          } else {
            bc.SendReply(clanName + " don't have any member ranked at this minigame.");
          }
        }
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void Performance(CommandContext bc) {
      string clanInitials;
      string clanName;
      if (bc.Message.ToUpperInvariant().Contains("SS")) {
        clanInitials = "SS";
        clanName = "Supreme Skillers";
      } else if (bc.Message.ToUpperInvariant().Contains("TS")) {
        clanInitials = "TS";
        clanName = "True Skillers";
      } else {
        clanInitials = "PT";
        clanName = "Portugal";
      }

      string rsn = bc.From.RSN;
      string skill = null;
      int rank = 0;
      bool IsIndividual = false;

      // get last updated player date
      DateTime lastUpdate = Database.GetString("SELECT lastupdate FROM players ORDER BY lastupdate DESC LIMIT 1;", string.Empty).ToDateTime();

      DateTime firstDay, lastDay;
      if (bc.MessageTokens[0].Contains("yesterday") || bc.MessageTokens[0].Contains("yday")) {
        lastDay = lastUpdate;
        firstDay = lastDay.AddDays(-1);
      } else if (bc.MessageTokens[0].Contains("lastweek") | bc.MessageTokens[0].Contains("lweek")) {
        lastDay = lastUpdate.AddDays(-((int)lastUpdate.DayOfWeek));
        firstDay = lastDay.AddDays(-7);
      } else if (bc.MessageTokens[0].Contains("lastmonth") | bc.MessageTokens[0].Contains("lmonth")) {
        lastDay = lastUpdate.AddDays(1 - lastUpdate.Day);
        firstDay = lastDay.AddMonths(-1);
      } else if (bc.MessageTokens[0].Contains("lastyear") | bc.MessageTokens[0].Contains("lyear")) {
        lastDay = lastUpdate.AddDays(1 - lastUpdate.DayOfYear);
        firstDay = lastDay.AddYears(-1);
      } else if (bc.MessageTokens[0].Contains("week")) {
        firstDay = lastUpdate.AddDays(-((int)lastUpdate.DayOfWeek));
        lastDay = lastUpdate;
      } else if (bc.MessageTokens[0].Contains("month")) {
        firstDay = lastUpdate.AddDays(1 - lastUpdate.Day);
        lastDay = lastUpdate;
      } else if (bc.MessageTokens[0].Contains("year")) {
        firstDay = lastUpdate.AddDays(1 - lastUpdate.DayOfYear);
        lastDay = lastUpdate;
      } else {
        Match M = Regex.Match(bc.MessageTokens[0], "last(\\d+)days");
        if (M.Success) {
          lastDay = lastUpdate;
          firstDay = lastDay.AddDays(-int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture));
        } else {
          return;
        }
      }
      if (firstDay == lastDay)
        return;

      // Create a list of Clan players
      Players clanPlayers = new Players(clanInitials, firstDay, lastDay);

      // Parse command arguments
      if (bc.MessageTokens.Length == 1) {
        // !ClanTop
        IsIndividual = true;
      } else if (Skill.TryParse(bc.MessageTokens[1], ref skill)) {
        // !ClanTop Skill
        rank = 1;

        // Clean and sort clan members by specified skill
        clanPlayers.RemoveAll(p => p.Skills[skill].Exp == 0);
        clanPlayers.SortBySkill(skill, true);

        if (bc.MessageTokens.Length > 2) {
          if (int.TryParse(bc.MessageTokens[2], out rank)) {
            // !ClanTop Skill Rank
          } else if (bc.MessageTokens.Length == 3 && bc.MessageTokens[2].ToUpperInvariant() == "@LAST") {
            // !ClanTop Skill @last
            rank = clanPlayers.Count;
          } else {
            // !ClanTop Skill
            rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
            if (clanPlayers.Contains(rsn))
              rank = clanPlayers.IndexOf(rsn) + 1;
          }
        }
      } else {
        // !ClanTop RSN
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
        IsIndividual = true;
      }

      if (IsIndividual) {
        Player p = clanPlayers.Find(rsn);
        if (p != null) {
          // individual skill ranks
          string reply = string.Format(CultureInfo.InvariantCulture, "[{0}] \\b{1}\\b skill ranks:", clanInitials, rsn);
          foreach (Skill s in p.Skills.Values) {
            if (s.Exp > 0) {
              clanPlayers.SortBySkill(s.Name, true);
              reply += " \\c07#" + (clanPlayers.IndexOf(p) + 1) + "\\c " + s.ShortName + ";";
            }
          }
          bc.SendReply(reply);
        } else {
          bc.SendReply(string.Format(CultureInfo.InvariantCulture, "\\b{0}\\b wasn't at {1}.", rsn, clanName));
        }
      } else {
        // Get input player rank
        int input_player_rank = 0;
        if (clanPlayers.Contains(bc.From.RSN))
          input_player_rank = clanPlayers.IndexOf(bc.From.RSN) + 1;

        // fix rank
        if (rank < 1)
          rank = 1;
        else if (rank > clanPlayers.Count)
          rank = clanPlayers.Count;

        int MinRank;
        MinRank = rank - 6;
        if (MinRank < 0)
          MinRank = 0;
        else if (MinRank > clanPlayers.Count - 11)
          MinRank = clanPlayers.Count - 11;

        if (clanPlayers.Count > 0) {
          string reply = "[" + clanInitials + "] \\u" + skill.ToLowerInvariant() + "\\u ranking:";
          if (input_player_rank > 0 && input_player_rank <= MinRank)
            reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Skills[skill].ToString("e", CultureInfo.InvariantCulture) + ");";

          for (int i = MinRank; i < Math.Min(MinRank + 11, clanPlayers.Count); i++) {
            reply += " ";
            if (i == rank - 1)
              reply += "\\b";
            reply += "\\c07#" + (i + 1) + "\\c ";
            if (i == input_player_rank - 1)
              reply += "\\u";
            reply += clanPlayers[i].Name;
            if (i == input_player_rank - 1)
              reply += "\\u";
            reply += " (" + clanPlayers[i].Skills[skill].ToString("e", CultureInfo.InvariantCulture) + ")";
            if (i == rank - 1)
              reply += "\\b";
            reply += ";";
          }

          if (input_player_rank > 0 && input_player_rank > MinRank + 11)
            reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Skills[skill].ToString("e", CultureInfo.InvariantCulture) + ");";

          bc.SendReply(reply);
        } else {
          bc.SendReply(clanName + " didn't have any member ranked at this skill.");
        }
      }
    }

    public static void Stats(CommandContext bc) {
      string skill = "Overall";
      if (bc.MessageTokens.Length > 1)
        Skill.TryParse(bc.MessageTokens[1], ref skill);

      int totallevel = 0;
      long totalexp = 0;
      Players ssplayers = new Players("SS");
      foreach (Player p in ssplayers) {
        if (p.Ranked) {
          totallevel += p.Skills[skill].Level;
          totalexp += p.Skills[skill].Exp;
        }
      }

      bc.SendReply(string.Format(CultureInfo.InvariantCulture, "\\bSupreme Skillers\\b | Forum: \\c12www.supremeskillers.com\\c | \\u{0}\\u average level: \\c07{1}\\c (\\c07{2:N0}\\c average exp.) | Members (\\c07{3}\\c): \\c12http://runehead.com/clans/ml.php?clan=lovvel\\c", skill, totallevel / ssplayers.Count, totalexp / ssplayers.Count, ssplayers.Count));
    }

  } //class CmdClan
} //namespace BigSister