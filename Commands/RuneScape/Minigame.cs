using System;

namespace Supay.Bot {
  static partial class Command {

    public static void Minigame(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
      else
        rsn = bc.GetPlayerName(bc.From.Nickname);

      Player p = new Player(rsn);
      if (p.Ranked) {
        Minigame minigame = p.Minigames[Supay.Bot.Minigame.Parse(bc.MessageTokens[0])];
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
              perf = _GetPerformance("Today", p_old.Minigames[minigame.Name], minigame);
              if (perf != null)
                reply += perf + " | ";
            }
            p_old = new Player(rsn, lastupdate.AddDays(-((int)lastupdate.DayOfWeek)));
            if (p_old.Ranked) {
              perf = _GetPerformance("Week", p_old.Minigames[minigame.Name], minigame);
              if (perf != null)
                reply += perf + " | ";
            }
            p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.Day));
            if (p_old.Ranked) {
              perf = _GetPerformance("Month", p_old.Minigames[minigame.Name], minigame);
              if (perf != null)
                reply += perf + " | ";
            }
            p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.DayOfYear));
            if (p_old.Ranked) {
              perf = _GetPerformance("Year", p_old.Minigames[minigame.Name], minigame);
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

    private static string _GetPerformance(string interval, Minigame mg_old, Minigame mg_new) {
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

  } //class Command
} //namespace Supay.Bot