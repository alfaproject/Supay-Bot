using System.Threading.Tasks;
using Supay.Irc;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Whois(CommandContext bc)
        {
            if (bc.MessageTokens.Length <= 1)
            {
                await bc.SendReply(@"{0}'s RSN is \b{1}\b", bc.From.Nickname, bc.GetPlayerName(bc.From.Nickname));
                return;
            }

            string nick = bc.MessageTokens.Join(1, "_");

            User u;
            if (bc.Users.TryGetValue(nick, out u))
            {
                await bc.SendReply(@"{0}'s RSN is \b{1}\b", u.Nickname, bc.GetPlayerName(nick));
            }
            else
            {
                await bc.SendReply(@"\c07{0}\c must be in a channel monitored by the bot for you to look up their RSN.", nick);
            }
        }
    }
}
