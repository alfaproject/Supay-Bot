using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static class CmdLinks
    {
        public static async Task Qfc(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                await bc.SendReply("Syntax: !Qfc <qfc>");
                return;
            }

            Match qfc = Regex.Match(bc.MessageTokens.Join(1), "(\\d+).(\\d+).(\\d+).(\\d+)");
            if (!qfc.Success)
            {
                await bc.SendReply("Syntax: !Qfc <qfc>");
            }
            else
            {
                await bc.SendReply(@"Quick find code \c07{0}-{1}-{2}-{3}\c: \c12http://forum.runescape.com/forums.ws?{0},{1},{2},{3}\c", qfc.Groups[1].Value, qfc.Groups[2].Value, qfc.Groups[3].Value, qfc.Groups[4].Value);
            }
        }
    }
}
