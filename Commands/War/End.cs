using System;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  static partial class Command {

    public static void WarEnd(CommandContext bc) {
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
      SQLiteParameter channelNameParameter = new SQLiteParameter("@channelName", channelName);

      // get skill name
      string skillName = Database.Lookup<string>("skill", "wars", "channel=@channelName", new[] { channelNameParameter });
      if (skillName == null) {
        bc.SendReply("You have to start a war in this channel first using !WarStart <skill>.");
        return;
      }

      string reply = string.Empty;
      SQLiteDataReader warPlayers = Database.ExecuteReader("SELECT rsn FROM warPlayers WHERE channel='" + channelName + "'");
      for (int count = 1; warPlayers.Read(); count++) {
        Player p = new Player(warPlayers.GetString(0));
        if (!p.Ranked) {
          bc.SendReply(@"Player \b" + p.Name + "\b has changed his/her name or was banned during the war, and couldn't be tracked.");
          continue;
        }
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

      bc.SendReply(@"\b{0}\b war ended on \u{1}\u for these players.".FormatWith(skillName, DateTime.UtcNow));

      Database.ExecuteNonQuery("DELETE FROM wars WHERE channel='" + channelName + "'");
      Database.ExecuteNonQuery("DELETE FROM warPlayers WHERE channel='" + channelName + "'");
    }

  } //class Command
} //namespace Supay.Bot