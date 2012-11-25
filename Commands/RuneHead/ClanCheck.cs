using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task ClanCheck(CommandContext bc)
        {
            if (!bc.IsAdmin)
            {
                await bc.SendReply("You need to be a bot administrator to use this command.");
                return;
            }

            if (bc.MessageTokens.Length == 1)
            {
                await bc.SendReply("Syntax: !ClanCheck <runehead alias>");
            }

            // get @f2p
            bool f2p = false;
            if (bc.Message.Contains(" @f2p"))
            {
                f2p = true;
                bc.Message = bc.Message.Replace(" @f2p", string.Empty);
            }

            // get @p2p
            bool p2p = false;
            if (bc.Message.Contains(" @p2p"))
            {
                p2p = true;
                bc.Message = bc.Message.Replace(" @p2p", string.Empty);
            }

            string pageRuneHead = new WebClient().DownloadString("http://runehead.com/clans/ml.php?sort=name&clan=" + bc.MessageTokens[1]);
            foreach (Match clanMember in Regex.Matches(pageRuneHead, "\\?name=([^&]+)"))
            {
                var p = new Player(clanMember.Groups[1].Value.ValidatePlayerName());
                if (!p.Ranked)
                {
                    await bc.SendReply(@"\b{0}\b is not ranked.", p.Name);
                    continue;
                }
                if (p.Name.StartsWithI("_") || p.Name.EndsWithI("_"))
                {
                    await bc.SendReply(@"\b{0}\b has unneeded underscores. Please change it to \b{1}\c.", p.Name, p.Name.Trim('_'));
                }

                if (f2p && p.Skills.F2pExp == p.Skills[Skill.OVER].Exp)
                {
                    await bc.SendReply(@"\b{0}\b is \c14F2P\c.", p.Name);
                }

                if (p2p && p.Skills.F2pExp != p.Skills[Skill.OVER].Exp)
                {
                    await bc.SendReply(@"\b{0}\b is \c07P2P\c.", p.Name);
                }
            }

            await bc.SendReply(@"Clan \b{0}\b is checked.", bc.MessageTokens[1]);
        }
    }
}
