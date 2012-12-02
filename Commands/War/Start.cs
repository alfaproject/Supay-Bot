using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task WarStart(CommandContext bc)
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

            // get skill name
            string skillName = Skill.OVER;
            if (bc.MessageTokens.Length < 2 || !Skill.TryParse(bc.MessageTokens[1], ref skillName))
            {
                await bc.SendReply(@"\bSyntax:\b !WarStart <skill name> [#channel name]");
                return;
            }

            string reply = string.Empty;
            var warPlayers = await Database.FetchAll("SELECT rsn FROM warPlayers WHERE channel=@channel", new MySqlParameter("@channel", channelName));
            for (int count = 1; count <= warPlayers.Count; count++)
            {
                var p = await Player.FromHiscores(warPlayers[count - 1].GetString(0));
                Database.Update("warPlayers", "channel='" + channelName + "' AND rsn='" + p.Name + "'", "startLevel", p.Skills[skillName].Level.ToStringI(), "startExp", p.Skills[skillName].Exp.ToStringI(), "startRank", p.Skills[skillName].Rank.ToStringI());
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

            Database.ExecuteNonQuery("DELETE FROM wars WHERE channel='" + channelName + "'");
            Database.Insert("wars", "channel", channelName, "skill", skillName, "startDate", DateTime.UtcNow.ToStringI("yyyyMMddHHmm"));

            await bc.SendReply(@"\b{0}\b war started on \u{1}\u for these players. \bYou can now login and good luck!\b", skillName, DateTime.UtcNow);
        }
    }
}
