using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task WarTop(CommandContext bc)
        {
            // get channel name
            string channelName = bc.Channel;
            Match matchChannel = Regex.Match(bc.Message, @"#(\S+)");
            if (matchChannel.Success)
            {
                channelName = matchChannel.Groups[1].Value;
                bc.Message = bc.Message.Replace(matchChannel.Value, string.Empty);
            }
            var channelNameParameter = new MySqlParameter("@channelName", channelName);

            // get skill name
            var skill = Database.Lookup<string>("skill", "wars", "channel=@channelName", new[] { channelNameParameter });
            if (skill == null)
            {
                await bc.SendReply("There isn't a war going on in this channel.");
                return;
            }

            await bc.SendReply("Please wait while the bot gathers all players stats...");

            // Create a list of the war players
            var warPlayers = new List<Player>();
            foreach (var warPlayersDr in Database.ExecuteReader("SELECT rsn, startrank, startlevel, startexp FROM warplayers WHERE channel='" + channelName + "'"))
            {
                var warPlayer = new Player(warPlayersDr.GetString(0));
                if (warPlayer.Ranked)
                {
                    warPlayer.Skills[skill] -= new Skill(skill, warPlayersDr.GetInt32(1), warPlayersDr.GetInt32(2), warPlayersDr.GetInt32(3));
                    warPlayers.Add(warPlayer);
                }
            }
            warPlayers = warPlayers.OrderByDescending(p => p.Skills[skill].Exp).ToList();

            // parse command arguments
            int rank = 1;
            if (bc.MessageTokens.Length > 1)
            {
                if (int.TryParse(bc.MessageTokens[1], out rank))
                {
                    // !War <rank>
                }
                else if (bc.MessageTokens[1].EqualsI("@last"))
                {
                    // !War @last
                    rank = warPlayers.Count;
                }
                else
                {
                    // !War <rsn>
                    string rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
                    var indexOfPlayer = warPlayers.FindIndex(p => p.Name.EqualsI(rsn));
                    if (indexOfPlayer != -1)
                    {
                        rank = indexOfPlayer + 1;
                    }
                }
            }

            // get input player rank
            var inputPlayerRank = warPlayers.FindIndex(p => p.Name.EqualsI(bc.GetPlayerName(bc.From.Nickname))) + 1;

            // fix rank
            if (rank < 1)
            {
                rank = 1;
            }
            else if (rank > warPlayers.Count)
            {
                rank = warPlayers.Count;
            }

            int minRank = rank - 6;
            if (minRank < 0)
            {
                minRank = 0;
            }
            else if (minRank > warPlayers.Count - 11)
            {
                minRank = warPlayers.Count - 11;
            }

            string reply = @"War \u{0}\u ranking:".FormatWith(skill.ToLowerInvariant());
            if (inputPlayerRank > 0 && inputPlayerRank <= minRank)
            {
                reply += @" \c07#{0}\c \u{1}\u ({2:e});".FormatWith(inputPlayerRank, warPlayers[inputPlayerRank - 1].Name, warPlayers[inputPlayerRank - 1].Skills[skill]);
            }

            for (int i = minRank; i < Math.Min(minRank + 11, warPlayers.Count); i++)
            {
                reply += " ";
                if (i == rank - 1)
                {
                    reply += @"\b";
                }
                reply += @"\c07#" + (i + 1) + @"\c ";
                if (i == inputPlayerRank - 1)
                {
                    reply += @"\u";
                }
                reply += warPlayers[i].Name;
                if (i == inputPlayerRank - 1)
                {
                    reply += @"\u";
                }
                reply += " (" + warPlayers[i].Skills[skill].ToStringI("e") + ")";
                if (i == rank - 1)
                {
                    reply += @"\b";
                }
                reply += ";";
            }

            if (inputPlayerRank > 0 && inputPlayerRank > minRank + 11)
            {
                reply += @" \c07#{0}\c \u{1}\u ({2:e});".FormatWith(inputPlayerRank, warPlayers[inputPlayerRank - 1].Name, warPlayers[inputPlayerRank - 1].Skills[skill]);
            }

            await bc.SendReply(reply);
        }
    }
}
