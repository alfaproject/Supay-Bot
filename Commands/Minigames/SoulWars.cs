using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task SoulWars(CommandContext bc)
        {
            // get rsn
            string rsn = string.Empty;
            string skill = string.Empty;
            int level = 0;
            if (bc.MessageTokens.Length == 2)
            {
                rsn = bc.GetPlayerName(bc.From.Nickname);
                Skill.TryParse(bc.MessageTokens[1], ref skill);
            }
            else if (bc.MessageTokens.Length == 3)
            {
                if (int.TryParse(bc.MessageTokens[1], out level))
                {
                    Skill.TryParse(bc.MessageTokens[2], ref skill);
                }
                else if (int.TryParse(bc.MessageTokens[2], out level))
                {
                    Skill.TryParse(bc.MessageTokens[1], ref skill);
                }
                else
                {
                    if (Skill.TryParse(bc.MessageTokens[1], ref skill))
                    {
                        rsn = bc.GetPlayerName(bc.MessageTokens[2]);
                    }
                    else if (Skill.TryParse(bc.MessageTokens[2], ref skill))
                    {
                        rsn = bc.GetPlayerName(bc.MessageTokens[1]);
                    }
                }
            }
            else if (bc.MessageTokens.Length > 3)
            {
                if (Skill.TryParse(bc.MessageTokens[1], ref skill))
                {
                    rsn = bc.GetPlayerName(bc.MessageTokens.Join(2).Trim());
                }
                else if (Skill.TryParse(bc.MessageTokens[bc.MessageTokens.Length - 1], ref skill))
                {
                    bc.MessageTokens[bc.MessageTokens.Length - 1] = string.Empty;
                    rsn = bc.GetPlayerName(bc.MessageTokens.Join(1).Trim());
                }
            }
            if (string.IsNullOrEmpty(skill) || (string.IsNullOrEmpty(rsn) && level == 0))
            {
                await bc.SendReply("Syntax: !soulwars <level> <skill>");
                return;
            }

            if (!string.IsNullOrEmpty(rsn))
            {
                var p = await Player.FromHiscores(rsn);
                if (!p.Ranked)
                {
                    await bc.SendReply(@"\b{0}\b doesn't feature Hiscores.", rsn);
                    return;
                }
                Skill skillInfo = p.Skills[skill];
                level = skillInfo.Level;
            }

            switch (skill)
            {
                case Skill.ATTA:
                case Skill.STRE:
                case Skill.DEFE:
                case Skill.HITP:
                case Skill.RANG:
                case Skill.MAGI:
                case Skill.PRAY:
                case Skill.SLAY:
                    int exp = Utils.SoulWarsExpPerZeal(skill, level);
                    await bc.SendReply(@"For each point at level \c07{0}\c you will gain \c07{1:N0} {2}\c experience.", level, exp, skill);
                    break;
                default:
                    await bc.SendReply("You can only calculate experience for Attack, Strength, Defence, Constitution, Prayer and Slayer at the moment.");
                    break;
            }
        }
    }
}
