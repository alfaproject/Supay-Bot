using System;
using System.Data.SQLite;

namespace BigSister {
  class CmdTimers {

    public static void Start(CommandContext bc) {
      // get rsn
      string rsn = bc.From.RSN;

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply(string.Format("\\b{0}\\b doesn't feature Hiscores.", rsn));
        return;
      }

      // get timer name
      string name = string.Empty;
      int indexofsharp = bc.Message.IndexOf("#");
      if (indexofsharp > 0) {
        name = bc.Message.Substring(indexofsharp + 1);
        bc.Message = bc.Message.Substring(0, indexofsharp - 1);
      }

      // get skill
      string skill = "Overall";
      if (bc.MessageTokens.Length > 1)
        Skill.TryParse(bc.MessageTokens[1], ref skill);

      // remove previous timer with this name, if any
      DataBase.ExecuteNonQuery("DELETE FROM timers_exp WHERE fingerprint='" + bc.From.FingerPrint + "' AND name='" + name.Replace("'", "''") + "';");

      // start a new timer with this name
      DataBase.Insert("timers_exp", "fingerprint", bc.From.FingerPrint,
                                    "name", name,
                                    "skill", skill,
                                    "exp", p.Skills[skill].Exp.ToString(),
                                    "datetime", DateTime.Now.ToString("yyyyMMddHHmmss"));
      bc.SendReply(string.Format("\\b{0}\\b starting exp of \\c07{1:e}\\c in \\u{1:n}\\u has been recorded{2}.", rsn, p.Skills[skill], name.Length > 0 ? " on timer \\c07" + name + "\\c" : string.Empty));
    }

    public static void Check(CommandContext bc) {
      // get rsn
      string rsn = bc.From.RSN;

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply(string.Format("\\b{0}\\b doesn't feature Hiscores.", rsn));
        return;
      }

      // get timer name
      string name = string.Empty;
      int indexofsharp = bc.Message.IndexOf("#");
      if (indexofsharp > 0) {
        name = bc.Message.Substring(indexofsharp + 1);
        bc.Message = bc.Message.Substring(0, indexofsharp - 1);
      }

      SQLiteDataReader rs = DataBase.ExecuteReader("SELECT skill, exp, datetime FROM timers_exp WHERE fingerprint='" + bc.From.FingerPrint + "' AND name='" + name.Replace("'", "''") + "' LIMIT 1;");
      if (rs.Read()) {
        string skill = rs.GetString(0);

        int gained_exp = p.Skills[skill].Exp - rs.GetInt32(1);
        TimeSpan time = DateTime.Now - rs.GetString(2).ToDateTime();

        string reply = string.Format("You gained \\c07{0:N0}\\c \\u{1}\\u exp in \\c07{2}\\c. That's \\c07{3:N0}\\c exp/h.", gained_exp, skill.ToLowerInvariant(), time.ToLongString(), (double)gained_exp / (double)time.TotalHours);
        if (skill != "Overall" && skill != "Combat" && p.Skills[skill].VLevel < 126)
          reply += string.Format(" Estimated time to level up: \\c07{0}\\c", TimeSpan.FromSeconds((double)p.Skills[skill].ExpToVLevel * (double)time.TotalSeconds / (double)gained_exp).ToLongString());
        bc.SendReply(reply);
      } else {
        if (name.Length > 0)
          bc.SendReply("You must start timing a skill on that timer first.");
        else
          bc.SendReply("You must start timing a skill first.");
      }
    }

    public static void Stop(CommandContext bc) {
      // get rsn
      string rsn = bc.From.RSN;

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply(string.Format("\\b{0}\\b doesn't feature Hiscores.", rsn));
        return;
      }

      // get timer name
      string name = string.Empty;
      int indexofsharp = bc.Message.IndexOf("#");
      if (indexofsharp > 0) {
        name = bc.Message.Substring(indexofsharp + 1);
        bc.Message = bc.Message.Substring(0, indexofsharp - 1);
      }

      SQLiteDataReader rs = DataBase.ExecuteReader("SELECT skill, exp, datetime FROM timers_exp WHERE fingerprint='" + bc.From.FingerPrint + "' AND name='" + name.Replace("'", "''") + "' LIMIT 1;");
      if (rs.Read()) {
        string skill = rs.GetString(0);

        int gained_exp = p.Skills[skill].Exp - rs.GetInt32(1);
        TimeSpan time = DateTime.Now - rs.GetString(2).ToDateTime();

        string reply = string.Format("You gained \\c07{0:N0}\\c \\u{1}\\u exp in \\c07{2}\\c. That's \\c07{3:N0}\\c exp/h.", gained_exp, skill.ToLowerInvariant(), time.ToLongString(), (double)gained_exp / (double)time.TotalHours);
        if (skill != "Overall" && skill != "Combat" && p.Skills[skill].VLevel < 126)
          reply += string.Format(" Estimated time to level up: \\c07{0}\\c", TimeSpan.FromSeconds((double)p.Skills[skill].ExpToVLevel / ((double)gained_exp / (double)time.TotalSeconds)).ToLongString());
        bc.SendReply(reply);

        // remove the timer with this name
        DataBase.ExecuteNonQuery("DELETE FROM timers_exp WHERE fingerprint='" + bc.From.FingerPrint + "' AND name='" + name.Replace("'", "''") + "';");
      } else {
        if (name.Length > 0)
          bc.SendReply("You must start timing a skill on that timer first.");
        else
          bc.SendReply("You must start timing a skill first.");
      }
    }

    public static void Timer(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        SQLiteDataReader rsTimer = DataBase.ExecuteReader("SELECT name, duration, started FROM timers WHERE fingerprint='" + bc.From.FingerPrint + "' OR nick='" + bc.From.Nick + "';");
        int timers = 0;
        string reply = string.Empty;
        while (rsTimer.Read()) {
          timers++;
          DateTime start = rsTimer.GetString(2).ToDateTime();
          DateTime end = start.AddSeconds(rsTimer.GetDouble(1));
          reply += string.Format(" \\b#{0}\\b timer (\\c07{1}\\c) ends in \\c07{2}\\c, at \\c07{3}\\c;", timers, rsTimer.GetString(0), (end - DateTime.Now).ToLongString(), end.ToString("yyyy/MM/dd HH:mm:ss"));
        }
        rsTimer.Close();
        if (timers > 0)
          bc.SendReply(string.Format("Found \\c07{0}\\c timers:", timers) + reply);
        else
          bc.SendReply("Syntax: !timer <duration>");

        return;
      }

      // get duration
      int duration;
      string name = null;
      switch (bc.MessageTokens[1].ToUpperInvariant()) {
        case "FARM":
        case "HERB":
        case "HERBS":
          duration = 75;
          name = bc.MessageTokens[1].ToLowerInvariant();
          break;
        case "DAY":
          duration = 1440;
          name = bc.MessageTokens[1].ToLowerInvariant();
          break;
        case "WEEK":
        case "TOG":
          duration = 10080;
          name = bc.MessageTokens[1].ToLowerInvariant();
          break;
        default:
          if (!int.TryParse(bc.MessageTokens[1], out duration)) {
            bc.SendReply("Error: Invalid duration. Duration must be in minutes.");
            return;
          }
          name = duration + " mins";
          break;
      }

      // start a new timer for this duration
      DataBase.Insert("timers", "fingerprint", bc.From.FingerPrint,
                                "nick", bc.From.Nick,
                                "name", name,
                                "duration", (duration * 60).ToString(),
                                "started", DateTime.Now.ToString("yyyyMMddHHmmss"));
      bc.SendReply(string.Format("Timer started to \\b{0}\\b. Timer will end at \\c07{1}\\c.", bc.From.Nick, DateTime.Now.AddMinutes(duration).ToString("yyyy/MM/dd HH:mm:ss")));
    }

  } //class CmdTimers
} //namespace BigSister