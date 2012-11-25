using System;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task LastUpdate(CommandContext bc)
        {
            DateTime lastUpdate = Database.Lookup("lastUpdate", "prices", "ORDER BY lastUpdate DESC", null, DateTime.UtcNow.ToStringI("yyyyMMddHHmm")).ToDateTime();
            await bc.SendReply(@"The GE was last updated \c07{0}\c ago. ({1:R})", (DateTime.UtcNow - lastUpdate).ToLongString(), lastUpdate);
        }
    }
}
