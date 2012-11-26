using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Supay.Bot
{
    internal class Players : List<Player>
    {
        public Players(string clan, bool onlyRanked = true)
        {
            SQLiteDataReader rs = Database.ExecuteReader("SELECT rsn, lastUpdate FROM players WHERE clan LIKE '%" + clan + "%'");
            while (rs.Read())
            {
                var player = new Player(rs.GetString(0), rs.GetString(1).ToDateTime());
                if (!onlyRanked || player.Ranked)
                {
                    this.Add(player);
                }
            }
            rs.Close();
        }

        // performance constructor
        public Players(string clan, DateTime firstDay, DateTime lastDay)
        {
            SQLiteDataReader rs = Database.ExecuteReader("SELECT rsn FROM players WHERE clan LIKE '%" + clan + "%'");
            while (rs.Read())
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
            rs.Close();
        }
    }
}
