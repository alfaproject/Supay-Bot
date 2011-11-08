using System;
using System.Globalization;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  internal static partial class Command {
    public static void ClanStats(CommandContext bc) {
      string skill = Skill.OVER;
      if (bc.MessageTokens.Length > 1) {
        Skill.TryParse(bc.MessageTokens[1], ref skill);
      }

      int totallevel = 0;
      long totalexp = 0;
      var ssplayers = new Players("SS");
      foreach (Player p in ssplayers) {
        if (p.Ranked) {
          totallevel += p.Skills[skill].Level;
          totalexp += p.Skills[skill].Exp;
        }
      }

      bc.SendReply(@"\bSupreme Skillers\b | Homepage: \c12www.supremeskillers.co.nr\c | \u{0}\u average level: \c07{1}\c (\c07{2:N0}\c average exp.) | Members (\c07{3}\c): \c12http://runehead.com/clans/ml.php?clan=supreme\c".FormatWith(skill, totallevel / ssplayers.Count, totalexp / ssplayers.Count, ssplayers.Count));
    }

    public static void Event(CommandContext bc) {
      try {
        string eventPage = new WebClient().DownloadString("http://ss.rsportugal.org/parser.php?type=event");
        JObject nextEvent = JObject.Parse(eventPage);
        DateTime startTime = DateTime.ParseExact((string) nextEvent["startTime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        var desc = (string) nextEvent["desc"];
        var url = (string) nextEvent["url"];
        bc.SendReply("Next event: \\c07{0}\\c starts in \\c07{1}\\c for more information: \\c12{2}\\c".FormatWith(desc, (startTime - DateTime.UtcNow).ToLongString(), url));
      } catch {
        bc.SendReply("Error retrieving next event.");
      }
    }
  }
}
