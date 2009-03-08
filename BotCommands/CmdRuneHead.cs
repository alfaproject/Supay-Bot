using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BigSister {
  class CmdRuneHead {

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
        reply = string.Format("\\b{0}\\b is in \\c07{1}\\c {2} (\\c12{3}\\c).", rsn, clans[0][0], type, clans[0][1]);
      } else {
        reply = string.Format("\\b{0}\\b is in \\c07{1}\\c {2}s:", rsn, clans.Count, type);
        foreach (string[] clan in clans)
          reply += string.Format(" \\c07{0}\\c;", clan[0]);
      }
      bc.SendReply(reply);
    }

    public static void Clan(CommandContext bc) {
      // get rsn
      string rsn;
      if (bc.MessageTokens.Length > 1)
        rsn = bc.NickToRSN(Util.JoinTokens(bc.MessageTokens, 1));
      else
        rsn = bc.From.RSN;

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
          bc.SendReply(string.Format("\\c12www.runehead.com\\c doesn't have any record for \\b{0}\\b.", rsn));

      } catch {
        bc.SendReply("\\c12www.runehead.com\\c seems to be down.");
      }
    }

    public static void ClanInfo(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !ClanInfo <clan name|clan initials>");
      }
      
      // Get the clan to lookup
      string query = Util.JoinTokens(bc.MessageTokens, 1);

      try {
        List<string[]> clans = new List<string[]>();
        string pageClans = new System.Net.WebClient().DownloadString("http://runehead.com/feeds/lowtech/searchclan.php?type=2&search=" + query);
        foreach (string clan in pageClans.Split('\n')) {
          string[] clanInfo = clan.Split('|');
          if (clanInfo.Length == 16)
            clans.Add(clanInfo);
        }

        if (clans.Count > 0) {
          bc.SendReply(string.Format("[\\c07{0}\\c] \\c07{1}\\c (\\c12{2}\\c) | Members: \\c07{3}\\c | Avg: Cmb: (F2P: \\c07{4}\\c | P2P: \\c07{5}\\c) Hp: \\c07{6}\\c Magic: \\c07{7}\\c Ranged: \\c07{8}\\c Skill Total: \\c07{9}\\c | \\c07{10}\\c based (Homeworld \\c07{11}\\c) | Cape: \\c07{12}\\c | RuneHead: \\c12{13}\\c",
            clans[0][4], clans[0][0], clans[0][1], clans[0][5], clans[0][15], clans[0][6], clans[0][7], clans[0][9], clans[0][10], clans[0][8], clans[0][11], clans[0][14], clans[0][13], clans[0][2]));
        } else {
          bc.SendReply(string.Format("\\c12www.runehead.com\\c doesn't have any record for \\b{0}\\b.", query));
        }
      } catch {
        bc.SendReply("\\c12www.runehead.com\\c seems to be down.");
      }
    }

    public static void ParseClan(CommandContext bc) {
      List<string> clanMembers = new List<string>(500);

      string pageRuneHead;
      string clanInitials;
      string clanName;
      if (bc.Message.ToUpperInvariant().Contains("SS")) {
        clanInitials = "SS";
        clanName = "Supreme Skillers";
        pageRuneHead = new System.Net.WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=lovvel");
      } else if (bc.Message.ToUpperInvariant().Contains("TS")) {
        clanInitials = "TS";
        clanName = "True Skillers";
        pageRuneHead = new System.Net.WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=trueskillers");
      } else {
        clanInitials = "PT";
        clanName = "Portugal";
        pageRuneHead = new System.Net.WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal");
        pageRuneHead += new System.Net.WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal2");
        pageRuneHead += new System.Net.WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugal3");
        pageRuneHead += new System.Net.WebClient().DownloadString("http://runehead.com/clans/ml.php?clan=rsportugalf2p");
      }

      foreach (Match clanMember in Regex.Matches(pageRuneHead, "\\?name=([^&]+)&"))
        clanMembers.Add(clanMember.Groups[1].Value.ToRSN());

      Players clanPlayers = new Players(clanInitials);

      // remove players from clan that were removed from clan listing
      foreach (Player p in clanPlayers) {
        if (!clanMembers.Contains(p.Name)) {
          DataBase.Update("players", "id=" + p.Id, "clan", string.Empty);
          bc.SendReply(string.Format("\\b{0}\\b is now being tracked under no clan.", p.Name));
        }
      }

      // add players that were added to clan listing to clan
      foreach (string rsn in clanMembers) {
        if (!clanPlayers.Contains(rsn)) {
          try {
            DataBase.Insert("players", "rsn", rsn, "clan", clanInitials, "lastupdate", string.Empty);
            Player p = new Player(rsn);
            if (p.Ranked)
              p.SaveToDB(DateTime.UtcNow.ToString("yyyyMMdd"));
          } catch {
            DataBase.Update("players", "rsn='" + rsn + "'", "clan", clanInitials);
          }
          bc.SendReply(string.Format("\\b{0}\\b is now being tracked under \\c07{1}\\c clan.", rsn, clanName));
        }
      }
    }

  } //class CmdRuneHead
} //namespace BigSister