using System.Linq;
using System.Data.SQLite;

namespace Supay.Bot {
  static partial class Command {

    public static void WarAdd(CommandContext bc) {
      if (!bc.IsAdmin) {
        bc.SendReply("You need to be a bot administrator to use this command.");
        return;
      }

      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !WarAdd <player name>");
        return;
      }

      string[] playerNames = bc.MessageTokens.Join(1).Split(new[] { ',', ';', '+', '|' });
      foreach (string playerName in playerNames.Select(name => name.ValidatePlayerName())) {
        if (Database.Lookup<string>("rsn", "warPlayers", "channel=@chan", new[] { new SQLiteParameter("@chan", bc.Channel) }) == playerName) {
          bc.SendReply(@"\b{0}\b is already signed to current war.".FormatWith(playerName));
        } else {
          Player player = new Player(playerName);
          if (player.Ranked) {
            string skillName = Database.Lookup<string>("skill", "wars", "channel=@chan", new[] { new SQLiteParameter("@chan", bc.Channel) });
            if (skillName == null) {
              Database.Insert("warPlayers", "channel", bc.Channel, "rsn", playerName);
            } else {
              Database.Insert("warPlayers", "channel", bc.Channel, "rsn", playerName,
                                            "startLevel", player.Skills[skillName].Level.ToStringI(),
                                            "startExp", player.Skills[skillName].Exp.ToStringI(),
                                            "startRank", player.Skills[skillName].Rank.ToStringI());
            }
            bc.SendReply(@"\b{0}\b is now signed to current war.".FormatWith(playerName));
            System.Threading.Thread.Sleep(1000);
          } else {
            bc.SendReply(@"\b{0}\b doesn't feature hiscores.".FormatWith(playerName));
          }
        }
      }
    }

  } //class Command
} //namespace Supay.Bot