using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  static partial class Command {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void ClanPerformance(CommandContext bc) {
      string clanInitials;
      string clanName;
      if (bc.Message.ContainsI("SS")) {
        clanInitials = "SS";
        clanName = "Supreme Skillers";
      } else if (bc.Message.ContainsI("TS")) {
        clanInitials = "TS";
        clanName = "True Skillers";
      } else {
        clanInitials = "PT";
        clanName = "Portugal";
      }

      string rsn = bc.From.Rsn;
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
          string reply = "[{0}] \\b{1}\\b skill ranks:".FormatWith(clanInitials, rsn);
          foreach (Skill s in p.Skills.Values) {
            if (s.Exp > 0) {
              clanPlayers.SortBySkill(s.Name, true);
              reply += " \\c07#" + (clanPlayers.IndexOf(p) + 1) + "\\c " + s.ShortName + ";";
            }
          }
          bc.SendReply(reply);
        } else {
          bc.SendReply("\\b{0}\\b wasn't at {1}.".FormatWith(rsn, clanName));
        }
      } else {
        // Get input player rank
        int input_player_rank = 0;
        if (clanPlayers.Contains(bc.From.Rsn))
          input_player_rank = clanPlayers.IndexOf(bc.From.Rsn) + 1;

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
            reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Skills[skill].ToStringI("e") + ");";

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
            reply += " (" + clanPlayers[i].Skills[skill].ToStringI("e") + ")";
            if (i == rank - 1)
              reply += "\\b";
            reply += ";";
          }

          if (input_player_rank > 0 && input_player_rank > MinRank + 11)
            reply += " \\c07#" + input_player_rank + "\\c \\u" + clanPlayers[input_player_rank - 1].Name + "\\u (" + clanPlayers[input_player_rank - 1].Skills[skill].ToStringI("e") + ");";

          bc.SendReply(reply);
        } else {
          bc.SendReply(clanName + " didn't have any member ranked at this skill.");
        }
      }
    }

  } //class Command
} //namespace Supay.Bot