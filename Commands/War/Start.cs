using System;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  internal static partial class Command {
    public static void WarStart(CommandContext bc) {
      if (!bc.IsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      // get channel name
      string channelName = bc.Channel;
      Match matchChannel = Regex.Match(bc.Message, @"#(\S+)");
      if (matchChannel.Success) {
        channelName = matchChannel.Groups[1].Value;
        bc.Message = bc.Message.Replace(matchChannel.Value, string.Empty);
      }

      // get skill name
      string skillName = Skill.OVER;
      if (bc.MessageTokens.Length < 2 || !Skill.TryParse(bc.MessageTokens[1], ref skillName)) {
        bc.SendReply("\bSyntax:\b !WarStart <skill name> [#channel name]");
        return;
      }

      string reply = string.Empty;
      SQLiteDataReader warPlayers = Database.ExecuteReader("SELECT rsn FROM warPlayers WHERE channel='" + channelName + "'");
      for (int count = 1; warPlayers.Read(); count++) {
        var p = new Player(warPlayers.GetString(0));
        Database.Update("warPlayers", "channel='" + channelName + "' AND rsn='" + p.Name + "'", "startLevel", p.Skills[skillName].Level.ToStringI(), "startExp", p.Skills[skillName].Exp.ToStringI(), "startRank", p.Skills[skillName].Rank.ToStringI());
        if (count % 2 == 0) {
          reply += @"\c07{0} ({1:e});\c ".FormatWith(p.Name, p.Skills[skillName]);
        } else {
          reply += "{0} ({1:e}); ".FormatWith(p.Name, p.Skills[skillName]);
        }
        if (count % 5 == 0) {
          bc.SendReply(reply);
          reply = string.Empty;
        }
      }
      if (!string.IsNullOrEmpty(reply)) {
        bc.SendReply(reply);
      }

      Database.ExecuteNonQuery("DELETE FROM wars WHERE channel='" + channelName + "';");
      Database.Insert("wars", "channel", channelName, "skill", skillName, "startDate", DateTime.UtcNow.ToStringI("yyyyMMddHHmm"));

      bc.SendReply(@"\b{0}\b war started on \u{1}\u for these players. \bYou can now login and good luck!\b".FormatWith(skillName, DateTime.UtcNow));
    }
  }
}
