using System;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task WarEnd(CommandContext bc)
        {
            if (!bc.IsAdmin)
            {
                await bc.SendReply("You need to be a bot administrator to use this command.");
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

            // get skill name
            var skillName = Database.Lookup<string>("skill", "wars", "channel=@channelName", new[] { channelNameParameter });
            if (skillName == null)
            {
                await bc.SendReply("You have to start a war in this channel first using !WarStart <skill>.");
                return;
            }

            string reply = string.Empty;
            var warPlayers = await Database.FetchAll("SELECT rsn FROM warPlayers WHERE channel=@channel", new MySqlParameter("@channel", channelName));
            for (int count = 1; count <= warPlayers.Count; count++)
            {
                var p = await Player.FromHiscores(warPlayers[count - 1].GetString(0));
                if (!p.Ranked)
                {
                    await bc.SendReply(@"Player \b" + p.Name + "\b has changed his/her name or was banned during the war, and couldn't be tracked.");
                    continue;
                }
                if (count % 2 == 0)
                {
                    reply += @"\c07{0} ({1:e});\c ".FormatWith(p.Name, p.Skills[skillName]);
                }
                else
                {
                    reply += "{0} ({1:e}); ".FormatWith(p.Name, p.Skills[skillName]);
                }
                if (count % 5 == 0)
                {
                    await bc.SendReply(reply);
                    reply = string.Empty;
                }
            }
            if (!string.IsNullOrEmpty(reply))
            {
                await bc.SendReply(reply);
            }

            await bc.SendReply(@"\b{0}\b war ended on \u{1}\u for these players.", skillName, DateTime.UtcNow);

            Database.ExecuteNonQuery("DELETE FROM wars WHERE channel='" + channelName + "'");
            Database.ExecuteNonQuery("DELETE FROM warPlayers WHERE channel='" + channelName + "'");
        }
    }
}
