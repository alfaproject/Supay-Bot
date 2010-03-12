﻿using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  static partial class Command {

    public static void WarRemove(CommandContext bc) {
      if (!bc.IsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      if (bc.MessageTokens.Length < 2) {
        bc.SendReply("\bSyntax:\b !WarRemove <player name> [#channel name]");
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

      string playerName = bc.MessageTokens.Join(1).ValidatePlayerName();
      SQLiteParameter playerNameParameter = new SQLiteParameter("@playerName", playerName);

      if (Database.Lookup<string>("rsn", "warPlayers", "channel=@channelName AND rsn=@playerName", new[] { channelNameParameter, playerNameParameter }) != null) {
        Database.ExecuteNonQuery("DELETE FROM warPlayers WHERE channel='" + channelName + "' AND rsn='" + playerName + "'");
        bc.SendReply(@"\b{0}\b was removed from current war.".FormatWith(playerName));
      } else {
        bc.SendReply(@"\b{0}\b isn't signed to current war.".FormatWith(playerName));
      }
    }

  } //class Command
} //namespace Supay.Bot