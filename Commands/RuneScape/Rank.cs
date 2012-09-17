using System.Linq;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static void Rank(CommandContext bc)
        {
            if (bc.MessageTokens.Length == 1)
            {
                // !rank
                bc.SendReply("Syntax: !rank <skill/activity> <rank>");
                return;
            }

            int rank = 1;
            string skill = null,
                   activity = null;

            if (bc.MessageTokens.Length > 1)
            {
                if (bc.MessageTokens[1].TryInt32(out rank))
                {
                    if (bc.MessageTokens.Length > 2 && (Skill.TryParse(bc.MessageTokens[2], ref skill) || Bot.Activity.TryParse(bc.MessageTokens[2], ref activity)))
                    {
                        // !rank <rank> <skill/activity>
                    }
                    else
                    {
                        // !rank <rank>
                        skill = Skill.OVER;
                    }
                }
                else if (Skill.TryParse(bc.MessageTokens[1], ref skill) || Bot.Activity.TryParse(bc.MessageTokens[1], ref activity))
                {
                    if (bc.MessageTokens.Length > 2 && bc.MessageTokens[2].TryInt32(out rank))
                    {
                        // !rank <skill/activity> <rank>
                    }
                    else
                    {
                        // !rank <skill/activity>
                        rank = 1;
                    }
                }
                else
                {
                    bc.SendReply("Syntax: !rank <skill/activity> <rank>");
                    return;
                }
            }

            // get the rsn for this rank
            string rsn = (from h in new Hiscores(skill, activity, rank)
                          where h.Rank == rank
                          select h.RSN).FirstOrDefault();

            if (rsn == null)
            {
                bc.SendReply("\\u{0}\\u hiscores don't have rank \\c07{1}\\c.".FormatWith(skill ?? activity, rank));
                return;
            }

            // redirect command to the apropriate sections
            if (activity == null)
            {
                bc.Message = skill + " " + rsn;
                SkillInfo(bc);
            }
            else
            {
                bc.Message = activity.Replace(" ", string.Empty) + " " + rsn;
                Activity(bc);
            }
        }
    }
}
