using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task ClanStats(CommandContext bc)
        {
            string skill = Skill.OVER;
            if (bc.MessageTokens.Length > 1)
            {
                Skill.TryParse(bc.MessageTokens[1], ref skill);
            }

            int totallevel = 0;
            long totalexp = 0;
            var ssplayers = await Players.FromClan("SS");
            foreach (Player p in ssplayers)
            {
                totallevel += p.Skills[skill].Level;
                totalexp += p.Skills[skill].Exp;
            }

            await bc.SendReply(@"\bSupreme Skillers\b | Homepage: \c12http://supremeskillers.net\c | \u{0}\u average level: \c07{1}\c (\c07{2:N0}\c average exp.) | Members (\c07{3}\c): \c12http://services.runescape.com/m=clan-hiscores/members.ws?clanId=314\c", skill, totallevel / ssplayers.Count, totalexp / ssplayers.Count, ssplayers.Count);
        }

        public static async Task Event(CommandContext bc)
        {
            bool all = bc.Message.Contains("@all");
            try
            {
                string eventPage = new WebClient().DownloadString("http://supremeskillers.net/api/?module=events&action=getNext");
                JObject nextEvent = JObject.Parse(eventPage);

                if (nextEvent["data"] == null)
                {
                    await bc.SendReply(@"No events currently set for Supreme Skillers. \c12http://supremeskillers.net/forum/public-events/\c");
                    return;
                }

                var events = new string[10];
                int i = -1;
                foreach (JObject eventData in nextEvent["data"])
                {
                    events[++i] = eventData.ToString();
                }

                DateTime startTime;
                if (!all)
                {
                    nextEvent = JObject.Parse(events[0]);
                    startTime = DateTime.ParseExact((string) nextEvent["startTime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    await bc.SendReply(@"Next event: \c07{0}\c starts in \c07{1}\c for more information: \c12{2}\c", nextEvent["description"], (startTime - DateTime.UtcNow).ToLongString(), nextEvent["url"]);
                }
                else
                {
                    string reply = "Upcoming SS Events: ";
                    for (i = 0; i < 10; i++)
                    {
                        if (events[i] == null)
                        {
                            break;
                        }
                        nextEvent = JObject.Parse(events[i]);
                        startTime = DateTime.ParseExact((string) nextEvent["startTime"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        if (i == 0)
                        {
                            reply += @"{0} (\c07{1}\c); ".FormatWith(nextEvent["description"], (startTime - DateTime.UtcNow).ToLongString());
                        }
                        else
                        {
                            reply += @"{0} (\c07{1}\c); ".FormatWith(nextEvent["description"], string.Format("{0: dddd d MMMM}", startTime).Trim());
                        }
                    }
                    await bc.SendReply(reply.Trim());
                }
                return;
            }
            catch
            {
            }
            await bc.SendReply("Error retrieving next event.");
        }
    }
}
