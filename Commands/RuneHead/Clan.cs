using System.Collections.Generic;

namespace Supay.Bot {
  static partial class Command {

    public static void Clan(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.NickToRSN(bc.MessageTokens.Join(1));
      else
        rsn = bc.FromRsn;

      try {
        int clanCount = 0;

        // Clans
        List<string[]> clans = _GetClans(rsn, "http://runehead.com/feeds/lowtech/searchuser.php?user=");
        clanCount += clans.Count;
        _OutputClans(bc, "clan", rsn, clans);

        // Non-clans
        clans = _GetClans(rsn, "http://runehead.com/feeds/lowtech/searchuser.php?type=1&user=");
        clanCount += clans.Count;
        _OutputClans(bc, "non-clan", rsn, clans);

        // User not found
        if (clanCount == 0)
          bc.SendReply("\\c12www.runehead.com\\c doesn't have any record for \\b{0}\\b.".FormatWith(rsn));

      } catch {
        bc.SendReply("\\c12www.runehead.com\\c seems to be down.");
      }
    }

    private static List<string[]> _GetClans(string rsn, string url) {
      List<string[]> clans = new List<string[]>(5);
      string pageClan = new System.Net.WebClient().DownloadString(url + rsn);
      foreach (string clan in pageClan.Split('\n')) {
        string[] clanInfo = clan.Split('|');
        if (clanInfo.Length == 2)
          clans.Add(clanInfo);
      }
      return clans;
    }

    private static void _OutputClans(CommandContext bc, string type, string rsn, List<string[]> clans) {
      if (clans.Count == 0)
        return;

      string reply;
      if (clans.Count == 1) {
        reply = "\\b{0}\\b is in \\c07{1}\\c {2} (\\c12{3}\\c).".FormatWith(rsn, clans[0][0], type, clans[0][1]);
      } else {
        reply = "\\b{0}\\b is in \\c07{1}\\c {2}s:".FormatWith(rsn, clans.Count, type);
        foreach (string[] clan in clans)
          reply += " \\c07{0}\\c;".FormatWith(clan[0]);
      }
      bc.SendReply(reply);
    }

  } //class Command
} ////namespace Supay.Bot