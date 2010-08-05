using System;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  static partial class Command {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void End(CommandContext bc) {
      // get rsn
      string rsn = bc.GetPlayerName(bc.From.Nickname);

      Player p = new Player(rsn);
      if (!p.Ranked) {
        bc.SendReply(@"\b{0}\b doesn't feature Hiscores.".FormatWith(rsn));
        return;
      }

      // get @set or @save
      bool set = bc.Message.ContainsI("@set") || bc.Message.ContainsI("@save");
      if (set) {
        bc.Message = Regex.Replace(bc.Message, @"\s*@s(?:et|ave)\s*", string.Empty);
      }

      // get timer name
      string name = string.Empty;
      int indexOfSharp = bc.Message.IndexOf('#');
      if (indexOfSharp > 0) {
        name = bc.Message.Substring(indexOfSharp + 1);
        bc.Message = bc.Message.Substring(0, indexOfSharp - 1);
      }

      SQLiteDataReader rs = Database.ExecuteReader("SELECT skill, exp, datetime FROM timers_exp WHERE fingerprint='" + bc.From.FingerPrint + "' AND name='" + name.Replace("'", "''") + "' LIMIT 1;");
      if (rs.Read()) {
        string skill = rs.GetString(0);

        long gainedExp = p.Skills[skill].Exp - rs.GetInt64(1);
        TimeSpan time = DateTime.UtcNow - rs.GetString(2).ToDateTime();

        string reply = @"You gained \c07{0:N0}\c \u{1}\u exp in \c07{2}\c. That's \c07{3:N0}\c exp/h.".FormatWith(gainedExp, skill.ToLowerInvariant(), time.ToLongString(), (double)gainedExp / time.TotalHours);
        if (gainedExp > 0 && skill != Skill.OVER && skill != Skill.COMB && p.Skills[skill].VLevel < 126) {
          reply += @" Estimated time to level up: \c07{0}\c".FormatWith(TimeSpan.FromSeconds((double)p.Skills[skill].ExpToVLevel / ((double)gainedExp / time.TotalSeconds)).ToLongString());
        }
        bc.SendReply(reply);

        if (gainedExp > 0) {
          // Add this player to database if he never set a default name.
          if (Database.Lookup<long>("COUNT(*)", "users", "fingerprint=@fp", new[] { new SQLiteParameter("@fp", bc.From.FingerPrint) }) < 1) {
            Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", bc.GetPlayerName(bc.From.Nickname));
          }

          // Set exp. made in an hour in this skill.
          Database.SetStringParameter("users", "speeds", "fingerprint='" + bc.From.FingerPrint + "'", skill, ((int)((double)gainedExp / time.TotalHours)).ToStringI());
        }

        // remove the timer with this name
        Database.ExecuteNonQuery("DELETE FROM timers_exp WHERE fingerprint='" + bc.From.FingerPrint + "' AND name='" + name.Replace("'", "''") + "';");
      } else {
        if (name.Length > 0) {
          bc.SendReply("You must start timing a skill on that timer first.");
        } else {
          bc.SendReply("You must start timing a skill first.");
        }
      }
    }

  } //class Command
} //namespace Supay.Bot