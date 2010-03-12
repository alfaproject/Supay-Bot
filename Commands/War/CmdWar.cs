using System;
using System.Data.SQLite;

namespace Supay.Bot {
  static class CmdWar {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static void TopAll(CommandContext bc) {
      if (!bc.IsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      string skill = Database.Lookup<string>("skill", "wars", "channel=@chan", new[] { new SQLiteParameter("@chan", bc.Channel) });
      if (skill == null) {
        bc.SendReply("There isn't a war going on in this channel.");
        return;
      }

      bc.SendReply("Please wait while the bot gathers all players stats...");

      // Create a list of the war players
      Players warPlayers = new Players();
      SQLiteDataReader warPlayersDr = Database.ExecuteReader("SELECT rsn, startrank, startlevel, startexp FROM warplayers WHERE channel='" + bc.Channel + "';");
      while (warPlayersDr.Read()) {
        Player warPlayer = new Player(warPlayersDr.GetString(0));
        if (!warPlayer.Ranked) { continue; }
        warPlayer.Skills[skill] -= new Skill(skill, warPlayersDr.GetInt32(1), warPlayersDr.GetInt32(2), warPlayersDr.GetInt32(3));
        warPlayers.Add(warPlayer);
      }
      warPlayers.SortBySkill(skill, true);

      string reply = null;
      int i = 0;
      while (i < warPlayers.Count) {
        if (i % 5 == 0) {
          if (reply != null)
            bc.SendReply(reply);
          reply = @"War \u{0}\u ranking:".FormatWith(skill.ToLowerInvariant());
        }
        reply += @" \c07#{0}\c {1} ({2:e});".FormatWith(i + 1, warPlayers[i].Name, warPlayers[i].Skills[skill]);
        i++;
      }

      if (reply != null)
        bc.SendReply(reply);
    }

  } //class CmdWar
} //namespace Supay.Bot