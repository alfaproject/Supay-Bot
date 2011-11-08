using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  internal static partial class Command {
    public static void ClanUpdate(CommandContext bc) {
      var clanMembers = new List<string>(500);

      string pageRuneHead;
      string clanInitials;
      string clanName;
      try {
        if (bc.Message.ContainsI("SS")) {
          clanInitials = "SS";
          clanName = "Supreme Skillers";
          pageRuneHead = new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=supreme");
        } else if (bc.Message.ContainsI("TS")) {
          clanInitials = "TS";
          clanName = "True Skillers";
          pageRuneHead = new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=trueskillers");
        } else {
          clanInitials = "PT";
          clanName = "Portugal";
          pageRuneHead = new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal");
          pageRuneHead += new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal2");
          pageRuneHead += new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal3");
          pageRuneHead += new WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=portugalf2p");
        }
      } catch {
        bc.SendReply("Update failed. Runehead appears to be down.");
        return;
      }

      clanMembers.AddRange(from Match clanMember in Regex.Matches(pageRuneHead, "\\?name=([^&]+)&")
        select clanMember.Groups[1].Value.ValidatePlayerName());

      var clanPlayers = new Players(clanInitials);
      // remove players from clan that were removed from clan listing
      foreach (Player p in clanPlayers) {
        if (!clanMembers.Contains(p.Name)) {
          Database.Update("players", "id=" + p.Id, "clan", string.Empty);
          bc.SendReply("\\b{0}\\b is now being tracked under no clan.".FormatWith(p.Name));
        }
      }

      // add players that were added to clan listing to clan
      foreach (string rsn in clanMembers) {
        if (!clanPlayers.Contains(rsn)) {
          bool f2p = false;
          try {
            Database.Insert("players", "rsn", rsn.ValidatePlayerName(), "clan", clanInitials, "lastupdate", string.Empty);
            var p = new Player(rsn);
            if (p.Ranked) {
              f2p = p.Skills.F2pExp == p.Skills[Skill.OVER].Exp;
              p.SaveToDB(DateTime.UtcNow.ToStringI("yyyyMMdd"));
            }
          } catch {
            Database.Update("players", "rsn LIKE '" + rsn + "'", "clan", clanInitials);
          }
          string reply = @"\b{0}\b is now being tracked under \c07{1}\c clan. \c{2}\c".FormatWith(rsn, clanName, f2p ? "14[F2P]" : "7[P2P]");
          bc.SendReply(reply);
        }
      }
      bc.SendReply("Clan \\b{0}\\b is up to date.".FormatWith(clanName));
    }
  }
}
