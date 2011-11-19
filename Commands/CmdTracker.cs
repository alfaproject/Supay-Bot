using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Supay.Bot
{
  internal static class CmdTracker
  {
    public static void Add(CommandContext bc)
    {
      if (!bc.IsAdmin)
      {
        return;
      }

      if (bc.MessageTokens.Length <= 1)
      {
        bc.SendReply("Syntax: !AddTracker <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ValidatePlayerName();
      try
      {
        var p = new Player(rsn);
        if (p.Ranked)
        {
          Database.Insert("players", "rsn", rsn, "clan", string.Empty, "lastupdate", string.Empty);
          p.SaveToDB(DateTime.UtcNow.ToStringI("yyyyMMdd"));
          bc.SendReply("\\b{0}\\b is now being tracked.".FormatWith(rsn));
        }
        else
        {
          bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
        }
      }
      catch
      {
        bc.SendReply("\\b{0}\\b was already being tracked.".FormatWith(rsn));
      }
    }

    public static void Remove(CommandContext bc)
    {
      if (!bc.IsAdmin)
      {
        return;
      }

      if (bc.MessageTokens.Length <= 1)
      {
        bc.SendReply("Syntax: !RemoveTracker <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ValidatePlayerName();
      long playerId = Database.Lookup("id", "players", "rsn LIKE @name", new[] { new SQLiteParameter("@name", rsn) }, -1L);
      if (playerId != -1)
      {
        Database.ExecuteNonQuery("DELETE FROM tracker WHERE pid=" + playerId + ";");
        Database.ExecuteNonQuery("DELETE FROM players WHERE id=" + playerId + ";");
        bc.SendReply("\\b{0}\\b was removed from the tracker database.".FormatWith(rsn));
      }
      else
      {
        bc.SendReply("\\b{0}\\b was not found on the tracker database.".FormatWith(rsn));
      }
    }

    public static void Rename(CommandContext bc)
    {
      if (!bc.IsAdmin)
      {
        return;
      }

      if (bc.MessageTokens.Length < 2)
      {
        bc.SendReply("Syntax: !Rename <old_rsn> <new_rsn>");
        return;
      }

      string oldRsn = bc.MessageTokens[1].ValidatePlayerName();
      string newRsn = bc.MessageTokens.Join(2).ValidatePlayerName();

      long oldPlayerId = Database.Lookup("id", "players", "rsn=@rsn", new[] { new SQLiteParameter("@rsn", oldRsn) }, -1L);
      if (oldPlayerId == -1)
      {
        bc.SendReply(@"Player \b{0}\b wasn't being tracked.".FormatWith(oldRsn));
        return;
      }

      // check if the old player still exists in hiscores
      var oldPlayer = new Player(oldRsn);
      if (oldPlayer.Ranked)
      {
        bc.SendReply(@"Player \b{0}\b is still ranked in hiscores.".FormatWith(oldRsn));
        return;
      }

      // check if the new player is in hiscores
      var newPlayer = new Player(newRsn);
      if (!newPlayer.Ranked)
      {
        bc.SendReply(@"Player \b{0\b doesn't feature in hiscores.".FormatWith(newRsn));
        return;
      }

      long newPlayerId = Database.Lookup("id", "players", "rsn=@rsn", new[] { new SQLiteParameter("@rsn", newRsn) }, -1L);

      // check if the new player already exists in the database
      if (newPlayerId != -1)
      {
        // delete the first record of the new player to prevent merge conflicts
        Database.ExecuteNonQuery("DELETE FROM tracker WHERE pid=" + newPlayerId + " AND date=(SELECT date FROM tracker WHERE pid=" + newPlayerId + " ORDER BY date LIMIT 1)");

        // merge both players data under the new player id
        Database.Update("tracker", "pid=" + oldPlayerId, "pid", newPlayerId.ToStringI());

        // remove the old player name
        Database.ExecuteNonQuery("DELETE FROM players WHERE id=" + oldPlayerId);
      }
      else
      {
        // rename the old player name with the new one
        Database.Update("players", "id=" + oldPlayerId, "rsn", newRsn);
      }

      bc.SendReply(@"Player \b{0}\b was renamed or merged to \b{1}\b.".FormatWith(oldRsn, newRsn));
    }

    public static void RemoveTrackerFromClan(CommandContext bc)
    {
      if (!bc.IsAdmin)
      {
        return;
      }

      if (bc.MessageTokens.Length == 1)
      {
        bc.SendReply("Syntax: !RemoveTrackerFromClan <clan|@clanless>");
        return;
      }

      string clan = bc.MessageTokens.Join(1).ToUpperInvariant();
      if (clan == "@CLANLESS")
      {
        clan = string.Empty;
      }

      int playersRemoved = 0;
      SQLiteDataReader dr = Database.ExecuteReader("SELECT id FROM players WHERE clan='" + clan + "';");
      while (dr.Read())
      {
        Database.ExecuteNonQuery("DELETE FROM tracker WHERE pid=" + dr.GetInt32(0) + ";");
        playersRemoved++;
      }
      dr.Close();
      Database.ExecuteNonQuery("DELETE FROM players WHERE clan ='" + clan + "';");
      bc.SendReply("\\b{0}\\b players were removed from tracker.".FormatWith(playersRemoved));

      Database.ExecuteNonQuery("VACUUM;");

      long playersLeft = Database.Lookup("COUNT(*)", "players", null, null, 0L);
      bc.SendReply("There are \\b{0}\\b players left in the tracker.".FormatWith(playersLeft));
    }

    public static void Performance(CommandContext bc)
    {
      bool showAll = false;
      if (bc.Message.Contains(" @all"))
      {
        showAll = true;
        bc.Message = bc.Message.Replace(" @all", string.Empty);
      }

      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
      {
        rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
      }
      else
      {
        rsn = bc.GetPlayerName(bc.From.Nickname);
      }

      // get this player last update time
      DateTime lastupdate;
      string dblastupdate = Database.LastUpdate(rsn);
      if (dblastupdate == null || dblastupdate.Length < 8)
      {
        lastupdate = DateTime.UtcNow.AddHours(-DateTime.UtcNow.Hour + 6).AddMinutes(-DateTime.UtcNow.Minute).AddSeconds(-DateTime.UtcNow.Second);
        if (DateTime.UtcNow.Hour >= 0 && DateTime.UtcNow.Hour < 6)
        {
          lastupdate = lastupdate.AddDays(-1);
        }
      }
      else
      {
        lastupdate = dblastupdate.ToDateTime();
      }

      // get performance interval
      int days;
      string interval;
      DateTime firstday,
               lastday;
      if (bc.MessageTokens[0].Contains("yesterday") || bc.MessageTokens[0].Contains("yday"))
      {
        interval = "Yesterday";
        lastday = lastupdate;
        firstday = lastday.AddDays(-1);
        days = 1;
      }
      else if (bc.MessageTokens[0].Contains("lastweek") | bc.MessageTokens[0].Contains("lweek"))
      {
        interval = "Last week";
        lastday = lastupdate.AddDays(-((int) lastupdate.DayOfWeek));
        firstday = lastday.AddDays(-7);
        days = 7;
      }
      else if (bc.MessageTokens[0].Contains("lastmonth") | bc.MessageTokens[0].Contains("lmonth"))
      {
        interval = "Last month";
        lastday = lastupdate.AddDays(1 - lastupdate.Day);
        firstday = lastday.AddMonths(-1);
        days = (lastday - firstday).Days;
      }
      else if (bc.MessageTokens[0].Contains("lastyear") | bc.MessageTokens[0].Contains("lyear"))
      {
        interval = "Last year";
        lastday = lastupdate.AddDays(1 - lastupdate.DayOfYear);
        firstday = lastday.AddYears(-1);
        days = (lastday - firstday).Days;
      }
      else if (bc.MessageTokens[0].Contains("week"))
      {
        interval = "Week";
        firstday = lastupdate.AddDays(-((int) lastupdate.DayOfWeek));
        lastday = DateTime.MaxValue;
        days = (int) lastupdate.DayOfWeek + 1;
      }
      else if (bc.MessageTokens[0].Contains("month"))
      {
        interval = "Month";
        firstday = lastupdate.AddDays(1 - lastupdate.Day);
        lastday = DateTime.MaxValue;
        days = lastupdate.Day;
      }
      else if (bc.MessageTokens[0].Contains("year"))
      {
        interval = "Year";
        firstday = lastupdate.AddDays(1 - lastupdate.DayOfYear);
        lastday = DateTime.MaxValue;
        days = lastupdate.DayOfYear;
      }
      else if (bc.MessageTokens[0].Contains("today"))
      {
        interval = "Today";
        firstday = lastupdate;
        lastday = DateTime.MaxValue;
        days = 1;
      }
      else
      {
        Match M = Regex.Match(bc.MessageTokens[0], "last(\\d+)days");
        if (M.Success)
        {
          days = int.Parse(M.Groups[1].Value, CultureInfo.InvariantCulture);
          interval = "Last " + days + " days";
          lastday = lastupdate;
          firstday = lastday.AddDays(-days);
        }
        else
        {
          return;
        }
      }

      // Get old player 
      var PlayerOld = new Player(rsn, firstday);
      if (!PlayerOld.Ranked)
      {
        // Get data from RuneScript
        PlayerOld = new Player(rsn, (int) (DateTime.UtcNow - firstday).TotalSeconds);
        bc.SendReply("\\c07{0}\\c information retrieved from RuneScript database. (This data may not be 100% accurate)".FormatWith(firstday.ToStringI("yyyy/MMM/dd")));
        if (!PlayerOld.Ranked)
        {
          bc.SendReply("\\b{0}\\b wasn't being tracked on \\c07{1}\\c.".FormatWith(rsn, firstday.ToStringI("yyyy/MMM/dd")));
          return;
        }
        foreach (Skill skill in PlayerOld.Skills.Values)
        {
          PlayerOld.Skills[skill.Name].Level = Math.Max(0, skill.Level);
        }
      }

      // Get new player
      Player PlayerNew;
      if (lastday == DateTime.MaxValue)
      {
        PlayerNew = new Player(rsn);
      }
      else
      {
        PlayerNew = new Player(rsn, lastday);
      }
      if (!PlayerNew.Ranked)
      {
        if (lastday == DateTime.MaxValue)
        {
          bc.SendReply("\\b{0}\\b doesn't feature Hiscores.".FormatWith(rsn));
          return;
        }

        // Get data from RuneScript
        PlayerNew = new Player(rsn, (int) (DateTime.UtcNow - lastday).TotalSeconds);
        bc.SendReply("\\c07{0}\\c information retrieved from RuneScript database. (This data may not be 100% accurate)".FormatWith(lastday.ToStringI("yyyy/MMM/dd")));
        if (!PlayerNew.Ranked)
        {
          bc.SendReply("\\b{0}\\b wasn't being tracked on \\c07{1}\\c.".FormatWith(rsn, lastday.ToStringI("yyyy/MMM/dd")));
          return;
        }
      }

      // 1st line: overall / combat
      string ReplyMsg = "\\b{0}\\b \\u{1}\\u skills:".FormatWith(rsn, interval.ToLowerInvariant());
      Skill OverallDif = PlayerNew.Skills[Skill.OVER] - PlayerOld.Skills[Skill.OVER];
      if (OverallDif.Exp <= 0)
      {
        bc.SendReply("No performance for \\b{0}\\b within this period.".FormatWith(rsn));
      }
      else
      {
        Skill CombatDif = PlayerNew.Skills[Skill.COMB] - PlayerOld.Skills[Skill.COMB];

        string DifLevel = string.Empty;
        if (OverallDif.Level > 0)
        {
          DifLevel = " [\\b+{0}\\b]".FormatWith(OverallDif.Level);
        }
        if (days == 1)
        {
          ReplyMsg += " \\c07Overall\\c lvl {0} \\c03+{1}\\c xp (Avg. hourly exp.: \\c07{2}\\c)".FormatWith(PlayerNew.Skills[Skill.OVER].Level + DifLevel, OverallDif.Exp.ToShortString(1), (OverallDif.Exp / 24.0).ToShortString(0));
        }
        else
        {
          ReplyMsg += " \\c07Overall\\c lvl {0} \\c03+{1}\\c xp (Avg. daily exp.: \\c07{2}\\c)".FormatWith(PlayerNew.Skills[Skill.OVER].Level + DifLevel, OverallDif.Exp.ToShortString(1), (OverallDif.Exp / (double) days).ToShortString(0));
        }
        DifLevel = string.Empty;
        if (CombatDif.Level > 0)
        {
          DifLevel = " [\\b+{0}\\b]".FormatWith(CombatDif.Level);
        }
        ReplyMsg += "; \\c07Combat\\c lvl {0} \\c03+{1}\\c xp (\\c07{2}%\\c)".FormatWith(PlayerNew.Skills[Skill.COMB].Level + DifLevel, CombatDif.Exp.ToShortString(1), (CombatDif.Exp / (double) OverallDif.Exp * 100.0).ToShortString(1));

        ReplyMsg += "; Interval: \\c07{0}\\c -> \\c07{1}\\c".FormatWith(firstday.ToStringI("yyyy/MMM/dd"), lastday == DateTime.MaxValue ? "Now" : lastday.ToStringI("yyyy/MMM/dd"));
        bc.SendReply(ReplyMsg);

        // 2nd line: skills list
        List<Skill> SkillsDif = (from SkillNow in PlayerNew.Skills.Values
                                 where SkillNow.Name != Skill.OVER && SkillNow.Name != Skill.COMB
                                 select SkillNow - PlayerOld.Skills[SkillNow.Name]).ToList();
        SkillsDif.Sort();

        int skillLength = showAll ? SkillsDif.Count : 10;
        bool has_performance = false;
        ReplyMsg = "\\b{0}\\b \\u{1}\\u skills:".FormatWith(rsn, interval.ToLowerInvariant());
        for (int i = 0; i < skillLength; i++)
        {
          if (SkillsDif[i].Exp > 0)
          {
            has_performance = true;
            DifLevel = string.Empty;
            if (SkillsDif[i].Level > 0)
            {
              DifLevel = " [\\b+{0}\\b]".FormatWith(SkillsDif[i].Level);
            }
            ReplyMsg += " \\c07{0}\\c lvl {1} \\c3+{2}\\c xp;".FormatWith(SkillsDif[i].Name, PlayerNew.Skills[SkillsDif[i].Name].Level + DifLevel, SkillsDif[i].Exp.ToShortString(1));
          }
          if ((i + 1) % 10 == 0)
          {
            bc.SendReply(ReplyMsg);
            has_performance = false;
            ReplyMsg = "\\b{0}\\b \\u{1}\\u skills:".FormatWith(rsn, interval.ToLowerInvariant());
          }
        }
        if (has_performance)
        {
          bc.SendReply(ReplyMsg);
        }

        // 3rd line: activities list
        List<Activity> activitiesDelta = (from newActivity in PlayerNew.Activities.Values
                                          where PlayerOld.Activities.ContainsKey(newActivity.Name)
                                          select newActivity - PlayerOld.Activities[newActivity.Name]).ToList();
        activitiesDelta.Sort();

        if (activitiesDelta.Count > 0)
        {
          has_performance = false;
          ReplyMsg = "\\b{0}\\b \\u{1}\\u activities:".FormatWith(rsn, interval.ToLowerInvariant());
          foreach (Activity t in activitiesDelta)
          {
            if (t.Score > 0)
            {
              has_performance = true;
              ReplyMsg += " \\c07{0}\\c \\c3+{1}\\c score;".FormatWith(t.Name, t.Score);
            }
          }
          if (has_performance)
          {
            bc.SendReply(ReplyMsg);
          }
        }
      }
    }
  }
}
