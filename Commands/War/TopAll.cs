using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task WarTopAll(CommandContext bc)
        {
            if (!bc.IsAdmin)
            {
                await bc.SendReply("You need to be a bot administrator to use this command.");
                return;
            }

            var skill = Database.Lookup<string>("skill", "wars", "channel=@chan", new[] { new MySqlParameter("@chan", bc.Channel) });
            if (skill == null)
            {
                await bc.SendReply("There isn't a war going on in this channel.");
                return;
            }

            await bc.SendReply("Please wait while the bot gathers all players stats...");

            // Create a list of the war players
            var warPlayers = new List<Player>();
            foreach (var warPlayersDr in Database.ExecuteReader("SELECT rsn, startrank, startlevel, startexp FROM warplayers WHERE channel='" + bc.Channel + "'"))
            {
                var warPlayer = new Player(warPlayersDr.GetString(0));
                if (!warPlayer.Ranked)
                {
                    continue;
                }
                warPlayer.Skills[skill] -= new Skill(skill, warPlayersDr.GetInt32(1), warPlayersDr.GetInt32(2), warPlayersDr.GetInt32(3));
                warPlayers.Add(warPlayer);
            }
            warPlayers = warPlayers.OrderByDescending(p => p.Skills[skill].Exp).ToList();

            string reply = null;
            for (int i = 0; i < warPlayers.Count; i++)
            {
                if (i % 5 == 0)
                {
                    if (reply != null)
                    {
                        await bc.SendReply(reply);
                    }
                    reply = @"War \u{0}\u ranking:".FormatWith(skill.ToLowerInvariant());
                }
                reply += @" \c07#{0}\c {1} ({2:e});".FormatWith(i + 1, warPlayers[i].Name, warPlayers[i].Skills[skill]);
            }
            if (reply != null)
            {
                await bc.SendReply(reply);
            }
        }
    }
}
