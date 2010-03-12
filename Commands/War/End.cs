using System;
using System.Data.SQLite;

namespace Supay.Bot {
  static partial class Command {

    public static void WarEnd(CommandContext bc) {
      if (!bc.IsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      string skill = Database.Lookup<string>("skill", "wars", "channel=@chan", new[] { new SQLiteParameter("@chan", bc.Channel) });
      if (skill == null) {
        bc.SendReply("You have to start a war in this channel first using !WarStart <skill>.");
        return;
      }

      int count = 0;
      string reply = string.Empty;
      SQLiteDataReader warPlayers = Database.ExecuteReader("SELECT rsn FROM warPlayers WHERE channel='" + bc.Channel + "'");
      while (warPlayers.Read()) {
        Player p = new Player(warPlayers.GetString(0));
        if (!p.Ranked) {
          bc.SendReply("Player " + p.Name + " has changed his/her name or was banned during the war, and cannot be tracked.");
          continue;
        }
        if (count % 2 == 0) {
          reply += @"\c07{0} ({1:e});\c ".FormatWith(p.Name, p.Skills[skill]);
        } else {
          reply += "{0} ({1:e}); ".FormatWith(p.Name, p.Skills[skill]);
        }
        count++;
        if (count % 4 == 0) {
          bc.SendReply(reply);
          count = 0;
          reply = string.Empty;
        }
      }
      if (count > 0) {
        bc.SendReply(reply);
      }

      bc.SendReply(@"\b{0}\b war ended on \u{1}\u for these players.".FormatWith(skill, DateTime.Now));

      Database.ExecuteNonQuery("DELETE FROM wars WHERE channel='" + bc.Channel + "'");
      Database.ExecuteNonQuery("DELETE FROM warPlayers WHERE channel='" + bc.Channel + "'");
    }

  } //class Command
} //namespace Supay.Bot