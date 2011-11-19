using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Supay.Bot
{
  internal static partial class Command
  {
    public static void ClanCompare(CommandContext bc)
    {
      if (bc.MessageTokens.Length < 3)
      {
        bc.SendReply("Syntax: !ClanCompare <clan1> <clan2>");
        return;
      }

      string clan1 = bc.MessageTokens[1];
      string clan2 = bc.MessageTokens.Join(2);

      try
      {
        // clan1 info
        string pageClan1 = new WebClient().DownloadString("http://runehead.com/feeds/lowtech/searchclan.php?type=2&search=" + clan1);
        List<string[]> clans = pageClan1.Split('\n').Select(clan => clan.Split('|')).Where(clanInfo => clanInfo.Length == 16).ToList();
        string clan1Name;
        string clan1Initial;
        int clan1Members;
        int clan1Combat;
        int clan1Total;
        if (clans.Count > 0)
        {
          clan1Name = clans[0][0];
          clan1Initial = clans[0][4];
          clan1Members = clans[0][5].ToInt32();
          clan1Combat = (int) double.Parse(clans[0][6], CultureInfo.InvariantCulture);
          clan1Total = clans[0][8].ToInt32();
        }
        else
        {
          bc.SendReply("\\c12www.runehead.com\\c doesn't have any record for \\b{0}\\b.".FormatWith(clan1));
          return;
        }

        // clan2 info
        string pageClan2 = new WebClient().DownloadString("http://runehead.com/feeds/lowtech/searchclan.php?type=2&search=" + clan2);
        List<string[]> clans2 = pageClan2.Split('\n').Select(clan => clan.Split('|')).Where(clanInfo => clanInfo.Length == 16).ToList();
        string clan2Name;
        string clan2Initial;
        int clan2Members;
        int clan2Combat;
        int clan2Total;
        if (clans2.Count > 0)
        {
          clan2Name = clans2[0][0];
          clan2Initial = clans2[0][4];
          clan2Members = clans2[0][5].ToInt32();
          clan2Combat = (int) double.Parse(clans2[0][6], CultureInfo.InvariantCulture);
          clan2Total = clans2[0][8].ToInt32();
        }
        else
        {
          bc.SendReply("\\c12www.runehead.com\\c doesn't have any record for \\b{0}\\b.".FormatWith(clan2));
          return;
        }

        // members
        string Members;
        if (clan1Members == clan2Members)
        {
          Members = @"| Both clans have \c07" + clan1Members + @"\c members ";
        }
        else if (clan1Members > clan2Members)
        {
          Members = @"| \c07{0}\c(\c07{1}\c) have \c07{2}\c more members than \c07{3}\c(\c07{4}\c) ".FormatWith(clan1Initial, clan1Members, clan1Members - clan2Members, clan2Initial, clan2Members);
        }
        else
        {
          Members = @"| \c07{0}\c(\c07{1}\c) have \c07{2}\c more members than \c07{3}\c(\c07{4}\c) ".FormatWith(clan2Initial, clan2Members, clan2Members - clan1Members, clan1Initial, clan1Members);
        }

        // Combat
        string Combat;
        if (clan1Combat == clan2Combat)
        {
          Combat = @"| Both clans have \c07" + clan1Combat + @"\c combat ";
        }
        else if (clan1Combat > clan2Combat)
        {
          Combat = @"| \c07{0}\c(\c07{1}\c) have \c07{2}\c more combat levels than \c07{3}\c(\c07{4}\c) ".FormatWith(clan1Initial, clan1Combat, clan1Combat - clan2Combat, clan2Initial, clan2Combat);
        }
        else
        {
          Combat = @"| \c07{0}\c(\c07{1}\c) have \c07{2}\c more combat levels than \c07{3}\c(\c07{4}\c) ".FormatWith(clan2Initial, clan2Combat, clan2Combat - clan1Combat, clan1Initial, clan1Combat);
        }

        // Total
        string Total;
        if (clan1Total == clan2Total)
        {
          Total = @"| Both clans have \c07" + clan1Total + @"\c overall ";
        }
        else if (clan1Total > clan2Total)
        {
          Total = @"| \c07{0}\c(\c07{1}\c) have \c07{2}\c more overall levels than \c07{3}\c(\c07{4}\c) ".FormatWith(clan1Initial, clan1Total, clan1Total - clan2Total, clan2Initial, clan2Total);
        }
        else
        {
          Total = @"| \c07{0}\c(\c07{1}\c) have \c07{2}\c more overall levels than \c07{3}\c(\c07{4}\c) ".FormatWith(clan2Initial, clan2Total, clan2Total - clan1Total, clan1Initial, clan1Total);
        }
        string reply = @"[\c07{0}\c]\c07{1}\c V [\c07{2}\c]\c07{3}\c ".FormatWith(clan1Initial, clan1Name, clan2Initial, clan2Name);
        reply += Members;
        reply += Combat;
        reply += Total;

        bc.SendReply(reply);
      }
      catch
      {
        bc.SendReply("\\c12www.runehead.com\\c seems to be down.");
      }
    }
  }
}
