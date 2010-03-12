using System.Data.SQLite;

namespace Supay.Bot {
  static partial class Command {

    public static void WarRemove(CommandContext bc) {
      if (!bc.IsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !WarRemove <player name>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ValidatePlayerName();

      if (Database.Lookup<string>("rsn", "warPlayers", "channel=@chan AND rsn=@rsn", new[] { new SQLiteParameter("@chan", bc.Channel), new SQLiteParameter("@rsn", rsn) }) != null) {
        Database.ExecuteNonQuery("DELETE FROM warPlayers WHERE channel='" + bc.Channel + "' AND rsn='" + rsn + "'");
        bc.SendReply(@"\b{0}\b was removed from current war.".FormatWith(rsn));
      } else {
        bc.SendReply(@"\b{0}\b isn't signed to current war.".FormatWith(rsn));
      }
    }

  } //class Command
} //namespace Supay.Bot