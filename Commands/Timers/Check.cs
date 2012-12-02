using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Check(CommandContext bc)
        {
            // get rsn
            string rsn = bc.GetPlayerName(bc.From.Nickname);

            var p = await Player.FromHiscores(rsn);
            if (!p.Ranked)
            {
                await bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }

            // get timer name
            string name = string.Empty;
            int indexofsharp = bc.Message.IndexOf('#');
            if (indexofsharp > 0)
            {
                name = bc.Message.Substring(indexofsharp + 1);
                bc.Message = bc.Message.Substring(0, indexofsharp - 1);
            }

            var rs = await Database.FetchFirst("SELECT skill,exp,datetime FROM timers_exp WHERE fingerprint=@fingerprint AND name=@name LIMIT 1", new MySqlParameter("@fingerprint", bc.From.FingerPrint), new MySqlParameter("@name", name));
            if (rs != null)
            {
                string skill = rs.GetString(0);

                long gained_exp = p.Skills[skill].Exp - (uint) rs["exp"];
                TimeSpan time = DateTime.UtcNow - rs.GetString(2).ToDateTime();

                var reply = @"You gained \c07{0:N0}\c \u{1}\u exp in \c07{2}\c. That's \c07{3:N0}\c exp/h.".FormatWith(gained_exp, skill.ToLowerInvariant(), time.ToLongString(), (double) gained_exp / time.TotalHours);
                if (gained_exp > 0 && skill != Skill.OVER && skill != Skill.COMB && p.Skills[skill].VLevel < 126)
                {
                    reply += @" Estimated time to level up: \c07{0}\c".FormatWith(TimeSpan.FromSeconds(p.Skills[skill].ExpToVLevel * time.TotalSeconds / gained_exp).ToLongString());
                }
                await bc.SendReply(reply);
            }
            else
            {
                if (name.Length > 0)
                {
                    await bc.SendReply("You must start timing a skill on that timer first.");
                }
                else
                {
                    await bc.SendReply("You must start timing a skill first.");
                }
            }
        }
    }
}
