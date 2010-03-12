using System;
using System.Data.SQLite;

namespace Supay.Bot {
  static partial class Command {

    public static void WarStart(CommandContext bc) {
      if (!bc.IsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      string skill = Skill.OVER;
      if (bc.MessageTokens.Length < 2 || !Skill.TryParse(bc.MessageTokens[1], ref skill)) {
        bc.SendReply("Syntax: !WarStart <skill>");
        return;
      }

      int count = 0;
      string reply = string.Empty;
      SQLiteDataReader warPlayers = Database.ExecuteReader("SELECT rsn FROM warPlayers WHERE channel='" + bc.Channel + "'");
      while (warPlayers.Read()) {
        Player p = new Player(warPlayers.GetString(0));
        Database.Update("warPlayers", "channel='" + bc.Channel + "' AND rsn='" + p.Name + "'",
                                      "startLevel", p.Skills[skill].Level.ToStringI(),
                                      "startExp", p.Skills[skill].Exp.ToStringI(),
                                      "startRank", p.Skills[skill].Rank.ToStringI());
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

      Database.ExecuteNonQuery("DELETE FROM wars WHERE channel='" + bc.Channel + "';");
      Database.Insert("wars", "channel", bc.Channel, "skill", skill, "startDate", DateTime.UtcNow.ToStringI("yyyyMMddHHmm"));

      bc.SendReply(@"\b{0}\b war started on \u{1}\u for these players. \bYou can now login and good luck!\b".FormatWith(skill, DateTime.Now));
    }

  } //class Command
} //namespace Supay.Bot