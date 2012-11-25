using System;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Activity(CommandContext bc)
        {
            // get rsn
            string rsn;
            if (bc.MessageTokens.Length > 1)
            {
                rsn = bc.GetPlayerName(bc.MessageTokens.Join(1));
            }
            else
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
            }

            var p = new Player(rsn);
            if (p.Ranked)
            {
                Activity activity = p.Activities[Bot.Activity.Parse(bc.MessageTokens[0])];
                if (activity.Rank > 0)
                {
                    string reply = @"\b{0}\b \c07{1:n}\c | score: \c07{1:s}\c | rank: \c07{1:R}\c".FormatWith(rsn, activity);

                    // Add up SS rank if applicable
                    var ssplayers = new Players("SS");
                    if (ssplayers.Contains(p.Name))
                    {
                        ssplayers.SortByActivity(activity.Name);
                        reply += @" (SS rank: \c07{0}\c)".FormatWith(ssplayers.IndexOf(rsn) + 1);
                    }

                    await bc.SendReply(reply);

                    // Show player performance if applicable
                    string dblastupdate = Database.LastUpdate(rsn);
                    if (dblastupdate != null && dblastupdate.Length == 8)
                    {
                        DateTime lastupdate = dblastupdate.ToDateTime();
                        string perf;
                        reply = string.Empty;

                        var p_old = new Player(rsn, lastupdate);
                        if (p_old.Ranked)
                        {
                            perf = _GetPerformance("Today", p_old.Activities[activity.Name], activity);
                            if (perf != null)
                            {
                                reply += perf + " | ";
                            }
                        }
                        p_old = new Player(rsn, lastupdate.AddDays(-((int) lastupdate.DayOfWeek)));
                        if (p_old.Ranked)
                        {
                            perf = _GetPerformance("Week", p_old.Activities[activity.Name], activity);
                            if (perf != null)
                            {
                                reply += perf + " | ";
                            }
                        }
                        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.Day));
                        if (p_old.Ranked)
                        {
                            perf = _GetPerformance("Month", p_old.Activities[activity.Name], activity);
                            if (perf != null)
                            {
                                reply += perf + " | ";
                            }
                        }
                        p_old = new Player(rsn, lastupdate.AddDays(1 - lastupdate.DayOfYear));
                        if (p_old.Ranked)
                        {
                            perf = _GetPerformance("Year", p_old.Activities[activity.Name], activity);
                            if (perf != null)
                            {
                                reply += perf;
                            }
                        }
                        if (reply.Length > 0)
                        {
                            await bc.SendReply(reply.EndsWithI(" | ") ? reply.Substring(0, reply.Length - 3) : reply);
                        }
                    }

                    return;
                }
            }
            await bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
        }

        private static string _GetPerformance(string interval, Activity mg_old, Activity mg_new)
        {
            Activity mg_dif = mg_new - mg_old;
            if (mg_dif.Score > 0 || mg_dif.Rank != 0)
            {
                string result = @"\u{0}:\u ".FormatWith(interval);

                if (mg_dif.Score > 0)
                {
                    result += @"\c03{0}\c score, ".FormatWith(mg_dif.Score);
                }

                if (mg_dif.Rank > 0)
                {
                    result += @"\c3+{0}\c rank{1};".FormatWith(mg_dif.Rank, mg_dif.Rank > 1 ? "s" : string.Empty);
                }
                else if (mg_dif.Rank < 0)
                {
                    result += @"\c4{0}\c rank{1};".FormatWith(mg_dif.Rank, mg_dif.Rank < -1 ? "s" : string.Empty);
                }

                return result.EndsWithI(", ") ? result.Substring(0, result.Length - 2) + ";" : result;
            }
            return null;
        }
    }
}
