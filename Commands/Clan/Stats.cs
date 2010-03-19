using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Supay.Bot {
  static partial class Command {

    public static void ClanStats(CommandContext bc) {
      string skill = Skill.OVER;
      if (bc.MessageTokens.Length > 1)
        Skill.TryParse(bc.MessageTokens[1], ref skill);

      int totallevel = 0;
      long totalexp = 0;
      Players ssplayers = new Players("SS");
      foreach (Player p in ssplayers) {
        if (p.Ranked) {
          totallevel += p.Skills[skill].Level;
          totalexp += p.Skills[skill].Exp;
        }
      }

      bc.SendReply(@"\bSupreme Skillers\b | Homepage: \c12www.supremeskillers.co.nr\c | \u{0}\u average level: \c07{1}\c (\c07{2:N0}\c average exp.) | Members (\c07{3}\c): \c12http://runehead.com/clans/ml.php?clan=lovvel\c".FormatWith(skill, totallevel / ssplayers.Count, totalexp / ssplayers.Count, ssplayers.Count));
    }

    public static void Event(CommandContext bc) {
      try {
        string desc, url;
        DateTime startTime;
        string eventPage = new System.Net.WebClient().DownloadString("http://ss.rsportugal.org/parser.php?type=event&channel=" + System.Web.HttpUtility.UrlEncode("#skillers"));
        JObject nextEvent = JObject.Parse(eventPage);

        startTime = DateTime.ParseExact((string)nextEvent["startTime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        desc = (string)nextEvent["desc"];
        url = (string)nextEvent["url"];
        bc.SendReply("Next event: \\c07{0}\\c starts in \\c07{1}\\c for more information: \\c12{2}\\c".FormatWith(desc, (startTime - DateTime.UtcNow).ToLongString(), url));
      } catch {
        bc.SendReply("Error retrieving next event.");
        return;
      }
    }

  } //class Command
} //namespace Supay.Bot