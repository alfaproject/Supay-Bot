using System;
using System.Collections.Generic;

namespace Supay.Bot
{
    internal class Players : List<Player>
    {
        public Players(string clan, bool onlyRanked = true)
        {
            foreach (var rs in Database.ExecuteReader("SELECT rsn, lastUpdate FROM players WHERE clan LIKE '%" + clan + "%'"))
            {
                var player = new Player(rs.GetString(0), rs.GetString(1).ToDateTime());
                if (!onlyRanked || player.Ranked)
                {
                    this.Add(player);
                }
            }
        }

        // performance constructor
        public Players(string clan, DateTime firstDay, DateTime lastDay)
        {
            foreach (var rs in Database.ExecuteReader("SELECT rsn FROM players WHERE clan LIKE '%" + clan + "%'"))
            {
                var playerBegin = new Player(rs.GetString(0), firstDay);
                if (playerBegin.Ranked)
                {
                    var playerEnd = new Player(rs.GetString(0), lastDay);
                    if (playerEnd.Ranked)
                    {
                        for (int i = 0; i < playerEnd.Skills.Count; i++)
                        {
                            playerEnd.Skills[i] -= playerBegin.Skills[i];
                        }
                        this.Add(playerEnd);
                    }
                }
            }
        }
    }
}
