using System;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Price(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                await bc.SendReply("Syntax: !Price [qty] <item>");
                return;
            }

            string search_term;
            double qty;
            if (bc.MessageTokens.Length > 2 && MathParser.TryCalc(bc.MessageTokens[1], out qty))
            {
                qty = Math.Round(qty, 1);
                search_term = bc.MessageTokens.Join(2);
            }
            else
            {
                qty = 1;
                search_term = bc.MessageTokens.Join(1);
            }

            var price_list = new Prices(search_term);
            if (price_list.Count == 0)
            {
                await bc.SendReply(@"\c12www.runescape.com\c doesn't have any record for '{0}'.", search_term);
                return;
            }

            var reply = @"\c12www.runescape.com\c found \c07{0}\c results".FormatWith(price_list.TotalItems);
            for (int i = 0; i < Math.Min(10, price_list.Count); i++)
            {
                reply += @" | {0}: \c07{1}\c".FormatWith(price_list[i].Name, (qty * price_list[i].MarketPrice).ToShortString(1));
                if (price_list[i].ChangeToday > 0)
                {
                    reply += @" \c3[+{0}]\c".FormatWith(price_list[i].ChangeToday.ToShortString(1));
                }
                else if (price_list[i].ChangeToday < 0)
                {
                    reply += @" \c4[{0}]\c".FormatWith(price_list[i].ChangeToday.ToShortString(1));
                }
            }
            if (price_list.TotalItems > 10)
            {
                reply += " | (...)";
            }
            await bc.SendReply(reply);
        }
    }
}
