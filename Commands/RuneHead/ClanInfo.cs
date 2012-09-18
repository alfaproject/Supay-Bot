using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task ClanInfo(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                bc.SendReply("Syntax: !ClanInfo <clan name|clan initials>");
            }

            // Get the clan to lookup
            string query = bc.MessageTokens.Join(1);

            try
            {
                string pageClans = new WebClient().DownloadString("http://runehead.com/feeds/lowtech/searchclan.php?type=2&search=" + query);
                List<string[]> clans = pageClans.Split('\n').Select(clan => clan.Split('|')).Where(clanInfo => clanInfo.Length == 16).ToArray().ToList();

                // order by overall, make sure ss gets the top spot :p
                try
                {
                    clans.Sort((c1, c2) => -int.Parse(c1[8]).CompareTo(int.Parse(c2[8])));
                }
                catch
                {
                }

                if (clans.Count > 0)
                {
                    bc.SendReply(@"[\c07{0}\c] \c07{1}\c (\c12{2}\c) | Members: \c07{3}\c | Avg: Cmb: (F2P: \c07{4}\c | P2P: \c07{5}\c) Hp: \c07{6}\c Magic: \c07{7}\c Ranged: \c07{8}\c Skill Total: \c07{9}\c | \c07{10}\c based (Homeworld \c07{11}\c) | Cape: \c07{12}\c | RuneHead: \c12{13}\c", clans[0][4], clans[0][0], clans[0][1], clans[0][5], clans[0][15], clans[0][6], clans[0][7], clans[0][9], clans[0][10], clans[0][8], clans[0][11], clans[0][14], clans[0][13], clans[0][2]);
                }
                else
                {
                    bc.SendReply(@"\c12www.runehead.com\c doesn't have any record for \b{0}\b.", query);
                }
            }
            catch
            {
                bc.SendReply(@"\c12www.runehead.com\c seems to be down.");
            }
        }
    }
}
