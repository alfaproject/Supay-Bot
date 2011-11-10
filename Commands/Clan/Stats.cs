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

      bc.SendReply(@"\bSupreme Skillers\b | Homepage: \c12http://supremeskillers.net\c | \u{0}\u average level: \c07{1}\c (\c07{2:N0}\c average exp.) | Members (\c07{3}\c): \c12http://services.runescape.com/m=clan-hiscores/members.ws?clanId=314\c".FormatWith(skill, totallevel / ssplayers.Count, totalexp / ssplayers.Count, ssplayers.Count));
    }

    public static void Event(CommandContext bc) {
      bool all = bc.Message.Contains("@all");
      try {
        string eventPage = new WebClient().DownloadString("http://supremeskillers.net/api/?module=events&action=getNext");
        JObject nextEvent = JObject.Parse(eventPage);

        if (nextEvent["data"] == null) {
          bc.SendReply("No events currently set for Supreme Skillers. \\c12http://supremeskillers.net/forum/public-events/");
          return;
        }

        var events = new string[10];
        int i = -1;
        foreach (JObject eventData in nextEvent["data"]) {
          events[++i] = eventData.ToString();
        }

        DateTime startTime;
        if (!all) {
          nextEvent = JObject.Parse(events[0]);
          startTime = DateTime.ParseExact((string) nextEvent["startTime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
          bc.SendReply("Next event: \\c07{0}\\c starts in \\c07{1}\\c for more information: \\c12{2}\\c".FormatWith((string) nextEvent["description"], (startTime - DateTime.UtcNow).ToLongString(), (string) nextEvent["url"]));
        } else {
          string reply = "Upcoming SS Events: ";
          for (i = 0; i < 10; i++) {
            if (events[i] == null) {
              break;
            }
            nextEvent = JObject.Parse(events[i]);
            startTime = DateTime.ParseExact((string) nextEvent["startTime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            if (i == 0) {
              reply += "{0} (\\c07{1}\\c); ".FormatWith((string) nextEvent["description"], (startTime - DateTime.UtcNow).ToLongString());
            } else {
              reply += "{0} (\\c07{1}\\c); ".FormatWith((string) nextEvent["description"], String.Format("{0: dddd d MMMM}", startTime).Trim());
            }
          }
          bc.SendReply(reply.Trim());
        }
      } catch {
        bc.SendReply("Error retrieving next event.");
      }
    }
  }
}
