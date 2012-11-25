using System;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task CoinShare(CommandContext bc)
        {
            if (bc.MessageTokens.Length < 3)
            {
                await bc.SendReply("Syntax: !CoinShare <players> <item>");
                return;
            }

            int players;
            string query = null;
            if (int.TryParse(bc.MessageTokens[1], out players))
            {
                query = bc.MessageTokens.Join(2);
            }
            else if (int.TryParse(bc.MessageTokens[bc.MessageTokens.Length - 1], out players))
            {
                query = string.Join(" ", bc.MessageTokens, 1, bc.MessageTokens.Length - 2);
            }
            if (players < 1 || players > 100)
            {
                await bc.SendReply("Error: Invalid number of players.");
                return;
            }

            Price price;

            int id;
            if (int.TryParse(query.TrimStart('#'), out id))
            {
                // !CoinShare <players> <id>
                price = new Price(id);
            }
            else
            {
                var prices = new Prices();
                prices.SearchExact(query);

                switch (prices.Count)
                {
                    case 0:
                        await bc.SendReply(@"Grand Exchange doesn't have any item matching '\c07{0}\c'.", query);
                        return;
                    case 1:
                        price = prices[0];
                        break;
                    default:
                        string reply = @"Grand Exchange results: \c07{0}\c".FormatWith(prices.Count);
                        for (int i = 0; i < Math.Min(15, prices.Count); i++)
                        {
                            reply += @" | \c07#{0}\c {1}".FormatWith(prices[i].Id, prices[i].Name);
                        }
                        if (prices.Count > 15)
                        {
                            reply += " | ...";
                        }
                        await bc.SendReply(reply);
                        return;
                }
            }

            price.LoadFromGE();
            if (price.Name == null)
            {
                await bc.SendReply(@"Grand Exchange doesn't have the item \c07#{0}\c.", price.Id);
            }
            else
            {
                await bc.SendReply(@"Name: \c07{0}\c | Minimum price: \c07{1}\c | Players: \c07{2:N0}\c | Player share: \c07{3:N0}\c | \c12http://services.runescape.com/m=itemdb_rs/viewitem.ws?obj={4}\c", price.Name, price.MarketPrice.ToShortString(1), players, price.MarketPrice / players, price.Id);
            }
        }
    }
}
