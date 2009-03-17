using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BigSister {
  static class CmdTracker {

    public static void Add(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !AddTracker <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ToRSN();
      try {
        Player p = new Player(rsn);
        if (p.Ranked) {
          Database.Insert("players", "rsn", rsn, "clan", string.Empty, "lastupdate", string.Empty);
          p.SaveToDB(DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
          bc.SendReply("\\b{0}\\b is now being tracked.".FormatWith(rsn));
        } else {
          bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        }
      } catch {
        bc.SendReply("\\b{0}\\b was already being tracked.".FormatWith(rsn));
      }
    }

    public static void Remove(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !RemoveTracker <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ToRSN();
      if (Database.GetValue("players", "id", "rsn='" + rsn + "'") != null) {
        int pid = Convert.ToInt32(Database.GetValue("players", "id", "rsn='" + rsn + "'"), CultureInfo.InvariantCulture);
        Database.ExecuteNonQuery("DELETE FROM tracker WHERE pid=" + pid + ";");
        Database.ExecuteNonQuery("DELETE FROM players WHERE id=" + pid + ";");
        bc.SendReply("\\b{0}\\b was removed from tracker.".FormatWith(rsn));
      } else {
        bc.SendReply("\\b{0}\\b wasn't being tracked.".FormatWith(rsn));
      }
    }

    public static void RemoveTrackerFromClan(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !RemoveTrackerFromClan <clan|@clanless>");
        return;
      }

      string clan = bc.MessageTokens.Join(1).ToUpperInvariant();
      if (clan == "@CLANLESS")
        clan = string.Empty;

      int playersRemoved = 0;
      SQLiteDataReader dr = Database.ExecuteReader("SELECT id FROM players WHERE clan='" + clan + "';");
      while (dr.Read()) {
        Database.ExecuteNonQuery("DELETE FROM tracker WHERE pid=" + dr.GetInt32(0) + ";");
        playersRemoved++;
      }
      dr.Close();
      Database.ExecuteNonQuery("DELETE FROM players WHERE clan ='" + clan + "';");
      bc.SendReply("\\b{0}\\b players were removed from tracker.".FormatWith(playersRemoved));

      Database.ExecuteNonQuery("VACUUM;");

      int playersLeft = Database.GetInteger("SELECT Count(*) FROM players;", 0);
      bc.SendReply("There are \\b{0}\\b players left in the tracker.".FormatWith(playersLeft));
    }

    public static void RemoveFromClan(CommandContext bc) {
      if (!bc.From.IsAdmin)
        return;

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !RemoveFromClan <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ToRSN();
      try {
        Database.Update("players", "rsn='" + rsn + "'", "clan", string.Empty);
        bc.SendReply("\\b{0}\\b is now being tracked under no clan.".FormatWith(rsn));
      } catch {
        bc.SendReply("\\b{0}\\b wasn't being tracked.".FormatWith(rsn));
      }
    }

    public static void Performance(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
      else
        rsn = bc.From.RSN;

      // get this player last update time
      DateTime lastupdate;
      string dblastupdate = Database.LastUpdate(rsn);
      if (dblastupdate == null || dblastupdate.Length < 8) {
        lastupdate = DateTime.Now.AddHours(-DateTime.Now.Hour + 6).AddMinutes(-DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second);
        if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
          lastupdate = lastupdate.AddDays(-1);
      } else {
        lastupdate = dblastupdate.ToDateTime();
      }

      // get performance interval
      int days;
      string interval;
      DateTime firstday, lastday;
      if (bc.MessageTokens[0].Contains("yesterday") || bc.MessageTokens[0].Contains("yday")) {
        interval = "Yesterday";
        lastday = lastupdate;
        firstday = lastday.AddDays(-1);
        days = 1;
      } else if (bc.MessageTokens[0].Contains("lastweek") | bc.MessageTokens[0].Contains("lweek")) {
        interval = "Last week";
        lastday = lastupdate.AddDays(-((int)lastupdate.DayOfWeek));
        firstday = lastday.AddDays(-7);
        days = 7;
      } else if (bc.MessageTokens[0].Contains("lastmonth") | bc.MessageTokens[0].Contains("lmonth")) {
        interval = "Last month";
        lastday = lastupdate.AddDays(1 - lastupdate.Day);
        firstday = lastday.AddMonths(-1);
        days = (lastday - firstday).Days;
      } else if (bc.MessageTokens[0].Contains("lastyear") | bc.MessageTokens[0].Contains("lyear")) {
        interval = "Last year";
        lastday = lastupdate.AddDays(1 - lastupdate.DayOfYear);
        firstday = lastday.AddYears(-1);
        days = (lastday - firstday).Days;
      } else if (bc.MessageTokens[0].Contains("week")) {
        interval = "Week";
        firstday = lastupdate.AddDays(-((int)lastupdate.DayOfWeek));
        lastday = DateTime.MaxValue;
        days = (int)lastupdate.DayOfWeek + 1;
      } else if (bc.MessageTokens[0].Contains("month")) {
        interval = "Month";
        firstday = lastupdate.AddDays(1 - lastupdate.Day);
        lastday = DateTime.MaxValue;
        days = lastupdate.Day;
      } else if (bc.MessageTokens[0].Contains("year")) {
        interval = "Year";
        firstday = lastupdate.AddDays(1 - lastupdate.DayOfYear);
        lastday = DateTime.MaxValue;
        days = lastupdate.DayOfYear;
      } else if (bc.MessageTokens[0].Contains("today")) {
        interval = "Today";
        firstday = lastupdate;
        lastday = DateTime.MaxValue;
        days = 1;
      } else {
        Match M = Regex.Match(bc.MessageTokens[0], "last(\\d+)days");
        if (M.Success) {
          days = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
          interval = "Last " + days + " days";
          lastday = lastupdate;
          firstday = lastday.AddDays(-days);
        } else {
          return;
        }
      }

      // Get old player 
      Player PlayerOld = new Player(rsn, firstday);
      if (!PlayerOld.Ranked) {
        // Get data from RuneScript
        PlayerOld = new Player(rsn, (int)(DateTime.Now - firstday).TotalSeconds);
        bc.SendReply("\\c07{0}\\c information retrieved from RuneScript database. (This data may not be 100% accurate)".FormatWith(firstday.ToString("yyyy/MMM/dd", CultureInfo.InvariantCulture)));
        if (!PlayerOld.Ranked) {
          bc.SendReply("\\b{0}\\b wasn't being tracked on \\c07{1}\\c.".FormatWith(rsn, firstday.ToString("yyyy/MMM/dd", CultureInfo.InvariantCulture)));
          return;
        }
      }

      // Get new player
      Player PlayerNew;
      if (lastday == DateTime.MaxValue)
        PlayerNew = new Player(rsn);
      else
        PlayerNew = new Player(rsn, lastday);
      if (!PlayerNew.Ranked) {
        if (lastday == DateTime.MaxValue) {
          bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
          return;
        } else {
          // Get data from RuneScript
          PlayerNew = new Player(rsn, (int)(DateTime.Now - lastday).TotalSeconds);
          bc.SendReply("\\c07{0}\\c information retrieved from RuneScript database. (This data may not be 100% accurate)".FormatWith(lastday.ToString("yyyy/MMM/dd", CultureInfo.InvariantCulture)));
          if (!PlayerNew.Ranked) {
            bc.SendReply("\\b{0}\\b wasn't being tracked on \\c07{1}\\c.".FormatWith(rsn, lastday.ToString("yyyy/MMM/dd", CultureInfo.InvariantCulture)));
            return;
          }
        }
      }

      // 1st line: overall / combat
      string ReplyMsg = "\\b{0}\\b \\u{1}\\u skills:".FormatWith(rsn, interval.ToLowerInvariant());
      Skill OverallDif = PlayerNew.Skills["Overall"] - PlayerOld.Skills["Overall"];
      if (OverallDif.Exp <= 0) {
        bc.SendReply("No performance for \\b{0}\\b within this period.".FormatWith(rsn));
      } else {
        Skill CombatDif = PlayerNew.Skills["Combat"] - PlayerOld.Skills["Combat"];

        string DifLevel = string.Empty;
        if (OverallDif.Level > 0)
          DifLevel = " [\\b+{0}\\b]".FormatWith(OverallDif.Level);
        if (days == 1) {
          ReplyMsg += " \\c07Overall\\c lvl {0} \\c03+{1}\\c xp (Avg. hourly exp.: \\c07{2}\\c)".FormatWith(PlayerNew.Skills["Overall"].Level + DifLevel, OverallDif.Exp.ToShortString(1), ((double)OverallDif.Exp / 24.0).ToShortString(0));
        } else {
          ReplyMsg += " \\c07Overall\\c lvl {0} \\c03+{1}\\c xp (Avg. daily exp.: \\c07{2}\\c)".FormatWith(PlayerNew.Skills["Overall"].Level + DifLevel, OverallDif.Exp.ToShortString(1), ((double)OverallDif.Exp / (double)days).ToShortString(0));
        }
        DifLevel = string.Empty;
        if (CombatDif.Level > 0) {
          DifLevel = " [\\b+{0}\\b]".FormatWith(CombatDif.Level);
        }
        ReplyMsg += "; \\c07Combat\\c lvl {0} \\c03+{1}\\c xp (\\c07{2}%\\c)".FormatWith(PlayerNew.Skills["Combat"].Level + DifLevel, CombatDif.Exp.ToShortString(1), ((double)CombatDif.Exp / (double)OverallDif.Exp * 100.0).ToShortString(1));

        ReplyMsg += "; Interval: \\c07{0}\\c -> \\c07{1}\\c".FormatWith(firstday.ToString("yyyy/MMM/dd", CultureInfo.InvariantCulture), lastday == DateTime.MaxValue ? "Now" : lastday.ToString("yyyy/MMM/dd", CultureInfo.InvariantCulture));
        bc.SendReply(ReplyMsg);

        // 2nd line: skills list
        List<Skill> SkillsDif = new List<Skill>();
        foreach (Skill SkillNow in PlayerNew.Skills.Values) {
          if (SkillNow.Name != "Overall" && SkillNow.Name != "Combat") {
            SkillsDif.Add(SkillNow - PlayerOld.Skills[SkillNow.Name]);
          }
        }
        SkillsDif.Sort();

        bool has_performance = false;
        ReplyMsg = "\\b{0}\\b \\u{1}\\u skills:".FormatWith(rsn, interval.ToLowerInvariant());
        for (int i = 0; i < 10; i++) {
          if (SkillsDif[i].Exp > 0) {
            has_performance = true;
            DifLevel = string.Empty;
            if (SkillsDif[i].Level > 0)
              DifLevel = " [\\b+{0}\\b]".FormatWith(SkillsDif[i].Level);
            ReplyMsg += " \\c7{0}\\c lvl {1} \\c3+{2}\\c xp;".FormatWith(SkillsDif[i].Name, PlayerNew.Skills[SkillsDif[i].Name].Level + DifLevel, SkillsDif[i].Exp.ToShortString(1));
          }
        }
        if (has_performance)
          bc.SendReply(ReplyMsg);

        // 3rd line: minigames list
        List<Minigame> MinigamesDif = new List<Minigame>();
        foreach (Minigame MinigameNew in PlayerNew.Minigames.Values) {
          if (PlayerOld.Minigames.ContainsKey(MinigameNew.Name))
            MinigamesDif.Add(MinigameNew - PlayerOld.Minigames[MinigameNew.Name]);
        }
        MinigamesDif.Sort();

        if (MinigamesDif.Count > 0) {
          has_performance = false;
          ReplyMsg = "\\b{0}\\b \\u{1}\\u minigames:".FormatWith(rsn, interval.ToLowerInvariant());
          for (int i = 0; i < MinigamesDif.Count; i++) {
            if (MinigamesDif[i].Score > 0) {
              has_performance = true;
              ReplyMsg += " \\c7{0}\\c \\c3+{1}\\c score;".FormatWith(MinigamesDif[i].Name, MinigamesDif[i].Score);
            }
          }
          if (has_performance)
            bc.SendReply(ReplyMsg);
        }
      }
    }

  } //class CmdTracker
} //namespace BigSister