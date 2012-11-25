using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Clan(CommandContext bc)
        {
            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
            }

            try
            {
                int clanCount = 0;

                // Clans
                List<string[]> clans = _GetClans(rsn, "http://runehead.com/feeds/lowtech/searchuser.php?user=");
                clanCount += clans.Count;
                await _OutputClans(bc, "clan", rsn, clans);

                // Non-clans
                clans = _GetClans(rsn, "http://runehead.com/feeds/lowtech/searchuser.php?type=1&user=");
                clanCount += clans.Count;
                await _OutputClans(bc, "non-clan", rsn, clans);

                // User not found
                if (clanCount == 0)
                {
                    await bc.SendReply(@"\c12www.runehead.com\c doesn't have any record for \b{0}\b.", rsn);
                }
                return;
            }
            catch
            {
            }
            await bc.SendReply(@"\c12www.runehead.com\c seems to be down.");
        }

        private static List<string[]> _GetClans(string rsn, string url)
        {
            var clans = new List<string[]>(10);
            using (var webClient = new WebClient())
            {
                string pageClan = webClient.DownloadString(url + rsn);
                clans.AddRange(pageClan.Split('\n').Select(clan => clan.Split('|')).Where(clanInfo => clanInfo.Length == 2));
            }
            return clans;
        }

        private static async Task _OutputClans(CommandContext bc, string type, string rsn, List<string[]> clans)
        {
            if (clans.Count == 0)
            {
                return;
            }

            string reply;
            if (clans.Count == 1)
            {
                reply = @"\b{0}\b is in \c07{1}\c {2} (\c12{3}\c).".FormatWith(rsn, clans[0][0], type, clans[0][1]);
            }
            else
            {
                reply = @"\b{0}\b is in \c07{1}\c {2}s:".FormatWith(rsn, clans.Count, type);
                reply = clans.Aggregate(reply, (current, clan) => current + @" \c07{0}\c;".FormatWith(clan[0]));
            }
            await bc.SendReply(reply);
        }
    }
}
