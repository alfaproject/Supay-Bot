using System;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task End(CommandContext bc)
        {
            // get rsn
            string rsn = bc.GetPlayerName(bc.From.Nickname);

            var p = await Player.FromHiscores(rsn);
            if (!p.Ranked)
            {
                await bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                return;
            }

            // get @set or @save
            bool set = bc.Message.ContainsI("@set") || bc.Message.ContainsI("@save");
            if (set)
            {
                bc.Message = Regex.Replace(bc.Message, @"\s*@s(?:et|ave)\s*", string.Empty);
            }

            // get timer name
            string name = string.Empty;
            int indexOfSharp = bc.Message.IndexOf('#');
            if (indexOfSharp > 0)
            {
                name = bc.Message.Substring(indexOfSharp + 1);
                bc.Message = bc.Message.Substring(0, indexOfSharp - 1);
            }

            var rs = await Database.FetchFirst("SELECT skill,exp,datetime FROM timers_exp WHERE fingerprint=@fingerprint AND name=@name LIMIT 1", new MySqlParameter("@fingerprint", bc.From.FingerPrint), new MySqlParameter("@name", name));
            if (rs != null)
            {
                string skill = rs.GetString(0);

                var gainedExp = p.Skills[skill].Exp - (uint) rs["exp"];
                TimeSpan time = DateTime.UtcNow - rs.GetString(2).ToDateTime();

                string reply = @"You gained \c07{0:N0}\c \u{1}\u exp in \c07{2}\c. That's \c07{3:N0}\c exp/h.".FormatWith(gainedExp, skill.ToLowerInvariant(), time.ToLongString(), (double) gainedExp / time.TotalHours);
                if (gainedExp > 0 && skill != Skill.OVER && skill != Skill.COMB && p.Skills[skill].VLevel < 126)
                {
                    reply += @" Estimated time to level up: \c07{0}\c".FormatWith(TimeSpan.FromSeconds(p.Skills[skill].ExpToVLevel / (gainedExp / time.TotalSeconds)).ToLongString());
                }
                await bc.SendReply(reply);

                if (gainedExp > 0)
                {
                    // Add this player to database if he never set a default name.
                    if (Database.Lookup<long>("COUNT(*)", "users", "fingerprint=@fp", new[] { new MySqlParameter("@fp", bc.From.FingerPrint) }) < 1)
                    {
                        Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", bc.GetPlayerName(bc.From.Nickname));
                    }

                    // Set exp. made in an hour in this skill.
                    Database.SetStringParameter("users", "speeds", "fingerprint='" + bc.From.FingerPrint + "'", skill, ((int) (gainedExp / time.TotalHours)).ToStringI());
                }

                // remove the timer with this name
                Database.ExecuteNonQuery("DELETE FROM timers_exp WHERE fingerprint='" + bc.From.FingerPrint + "' AND name='" + name.Replace("'", "''") + "'");
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
