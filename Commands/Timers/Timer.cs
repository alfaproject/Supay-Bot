using System;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Timer(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                int timers = 0;
                string reply = string.Empty;
                foreach (var rsTimer in Database.ExecuteReader("SELECT name, duration, started FROM timers WHERE fingerprint='" + bc.From.FingerPrint + "' OR nick='" + bc.From.Nickname + "'"))
                {
                    timers++;
                    DateTime start = rsTimer.GetString(2).ToDateTime();
                    DateTime end = start.AddSeconds(rsTimer.GetDouble(1));
                    reply += @" \b#{0}\b timer (\c07{1}\c) ends in \c07{2}\c, at \c07{3}\c;".FormatWith(timers, rsTimer.GetString(0), (end - DateTime.UtcNow).ToLongString(), end.ToStringI("yyyy/MM/dd HH:mm:ss"));
                }
                if (timers > 0)
                {
                    await bc.SendReply(@"Found \c07{0}\c timers: {1}", timers, reply);
                }
                else
                {
                    await bc.SendReply("Syntax: !timer <duration>");
                }

                return;
            }

            // get duration
            int duration;
            string name;
            switch (bc.MessageTokens[1].ToUpperInvariant())
            {
                case "FARM":
                case "HERB":
                case "HERBS":
                    duration = 75;
                    name = bc.MessageTokens[1].ToLowerInvariant();
                    break;
                case "DAY":
                    duration = 1440;
                    name = bc.MessageTokens[1].ToLowerInvariant();
                    break;
                case "WEEK":
                case "TOG":
                    duration = 10080;
                    name = bc.MessageTokens[1].ToLowerInvariant();
                    break;
                default:
                    if (!int.TryParse(bc.MessageTokens[1], out duration))
                    {
                        await bc.SendReply("Error: Invalid duration. Duration must be in minutes.");
                        return;
                    }
                    name = duration + " mins";
                    break;
            }

            // start a new timer for this duration
            Database.Insert("timers", "fingerprint", bc.From.FingerPrint, "nick", bc.From.Nickname, "name", name, "duration", (duration * 60).ToStringI(), "started", DateTime.UtcNow.ToStringI("yyyyMMddHHmmss"));
            await bc.SendReply(@"Timer started to \b{0}\b. Timer will end at \c07{1}\c.", bc.From.Nickname, DateTime.UtcNow.AddMinutes(duration).ToStringI("yyyy/MM/dd HH:mm:ss"));
        }
    }
}
