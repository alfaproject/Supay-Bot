using MySql.Data.MySqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task WarAdd(CommandContext bc)
        {
            if (!bc.IsAdmin)
            {
                await bc.SendReply("You need to be a bot administrator to use this command.");
                return;
            }

            if (bc.MessageTokens.Length < 2)
            {
                await bc.SendReply(@"\bSyntax:\b !WarAdd <player name> [#channel name]");
                return;
            }

            // get channel name
            string channelName = bc.Channel;
            Match matchChannel = Regex.Match(bc.Message, @"#(\S+)");
            if (matchChannel.Success)
            {
                channelName = matchChannel.Groups[1].Value;
                bc.Message = bc.Message.Replace(matchChannel.Value, string.Empty);
            }
            var channelNameParameter = new MySqlParameter("@channelName", channelName);

            string[] playerNames = bc.MessageTokens.Join(1).Split(new[] { ',', ';', '+', '|' });
            foreach (string playerName in playerNames.Select(name => name.ValidatePlayerName()))
            {
                if (Database.Lookup<string>("rsn", "warPlayers", "channel=@channelName", new[] { channelNameParameter }) == playerName)
                {
                    await bc.SendReply(@"\b{0}\b is already signed to current war.", playerName);
                }
                else
                {
                    var player = new Player(playerName);
                    if (player.Ranked)
                    {
                        var skillName = Database.Lookup<string>("skill", "wars", "channel=@channelName", new[] { channelNameParameter });
                        if (skillName == null)
                        {
                            Database.Insert("warPlayers", "channel", channelName, "rsn", playerName);
                        }
                        else
                        {
                            Database.Insert("warPlayers", "channel", channelName, "rsn", playerName, "startLevel", player.Skills[skillName].Level.ToStringI(), "startExp", player.Skills[skillName].Exp.ToStringI(), "startRank", player.Skills[skillName].Rank.ToStringI());
                        }
                        await bc.SendReply(@"\b{0}\b is now signed to current war.", playerName);
                        await Task.Delay(1000);
                    }
                    else
                    {
                        await bc.SendReply(@"\b{0}\b doesn't feature hiscores.", playerName);
                    }
                }
            }
        }
    }
}
