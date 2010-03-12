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

      string[] rsns = bc.MessageTokens.Join(1).Split(new char[] { ',', ';', '+' });
      foreach (string dirtyRsn in rsns) {
        string rsn = dirtyRsn.ValidatePlayerName();

        if (Database.Lookup<string>("rsn", "warPlayers", "channel=@chan", new[] { new SQLiteParameter("@chan", bc.Channel) }) == rsn) {
          bc.SendReply(@"\b{0}\b was already signed to current war.".FormatWith(rsn));
        } else {
          Player p = new Player(rsn);
          if (p.Ranked) {
            string skill = Database.Lookup<string>("skill", "wars", "channel=@chan", new[] { new SQLiteParameter("@chan", bc.Channel) });
            if (skill == null) {
              Database.Insert("warPlayers", "channel", bc.Channel, "rsn", rsn);
            } else {
              Database.Insert("warPlayers", "channel", bc.Channel, "rsn", rsn,
                                            "startLevel", p.Skills[skill].Level.ToStringI(),
                                            "startExp", p.Skills[skill].Exp.ToStringI(),
                                            "startRank", p.Skills[skill].Rank.ToStringI());
            }
            bc.SendReply(@"\b{0}\b is now signed to current war.".FormatWith(rsn));
            System.Threading.Thread.Sleep(1000);
          } else {
            bc.SendReply(@"\b{0}\b doesn't feature hiscores.".FormatWith(rsn));
          }
        }
      }
    }

  } //class Command
} //namespace Supay.Bot