using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static class Players
    {
        public async static Task<IList<Player>> FromClan(string clan, bool onlyRanked = true)
        {
            var players = new List<Player>();
            foreach (var rs in await Database.FetchAll("SELECT rsn, lastUpdate FROM players WHERE clan LIKE '%" + clan + "%'"))
            {
                var player = await Player.FromDatabase(rs.GetString(0), rs.GetString(1).ToDateTime());
                if (!onlyRanked || player.Ranked)
                {
                    players.Add(player);
                }
            }
            return players;
        }

        public async static Task<IList<Player>> FromClanAsPeriod(string clan, DateTime firstDay, DateTime lastDay)
        {
            var players = new List<Player>();
            foreach (var rs in await Database.FetchAll("SELECT rsn FROM players WHERE clan LIKE '%" + clan + "%'"))
            {
                var playerBegin = await Player.FromDatabase(rs.GetString(0), firstDay);
                if (playerBegin.Ranked)
                {
                    var playerEnd = await Player.FromDatabase(rs.GetString(0), lastDay);
                    if (playerEnd.Ranked)
                    {
                        for (int i = 0; i < playerEnd.Skills.Count; i++)
                        {
                            playerEnd.Skills[i] -= playerBegin.Skills[i];
                        }
                        players.Add(playerEnd);
                    }
                }
            }
            return players;
        }
    }
}
