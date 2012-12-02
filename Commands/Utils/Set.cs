using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal static partial class Command
    {
        public static async Task Set(CommandContext bc)
        {
            if (bc.MessageTokens.Length < 2)
            {
                await bc.SendReply("Syntax: !set <param> [skill] <value>");
                return;
            }

            switch (bc.MessageTokens[1].ToUpperInvariant())
            {
                case "RSN":
                case "NAME":
                    await _SetName(bc);
                    break;
                case "GOAL":
                    await _SetGoal(bc);
                    break;
                case "ITEM":
                    await _SetItem(bc);
                    break;
                case "SPEED":
                    await _SetSpeed(bc);
                    break;
                case "SKILL":
                    await _SetSkill(bc);
                    break;
                case "@":
                    await _SetSkillToggle(bc);
                    break;
                default:
                    await bc.SendReply("Error: Unknown parameter.");
                    break;
            }
        }

        private static async Task _SetName(CommandContext bc)
        {
            if (bc.MessageTokens.Length < 3)
            {
                await bc.SendReply("Syntax: !set name <rsn>");
                return;
            }

            string rsn = bc.MessageTokens.Join(2).ValidatePlayerName();

            // add/update to database
            if (await Database.Lookup<long>("COUNT(*)", "users", "fingerprint=@fp", new[] { new MySqlParameter("@fp", bc.From.FingerPrint) }) > 0)
            {
                Database.Update("users", "fingerprint='" + bc.From.FingerPrint + "'", "rsn", rsn);
            }
            else
            {
                Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", rsn);
            }

            await bc.SendReply(@"Your default RuneScape name is now \b{0}\b. This RSN is associated with the address \u*!*{1}\u.", rsn, bc.From.FingerPrint);
        }

        private static async Task _SetGoal(CommandContext bc)
        {
            if (bc.MessageTokens.Length < 3)
            {
                await bc.SendReply("Syntax: !set goal <skill> [goal]");
                return;
            }

            string skill = null;
            string goal = null;
            if (Skill.TryParse(bc.MessageTokens[2], ref skill))
            {
                if (bc.MessageTokens.Length > 3)
                {
                    // !set goal <skill> <goal>
                    goal = bc.MessageTokens[3];
                } // else { // !set goal <skill> }
            }
            else if (bc.MessageTokens.Length > 3 && Skill.TryParse(bc.MessageTokens[3], ref skill))
            {
                // !set goal <goal> <skill>
                goal = bc.MessageTokens[2];
            }
            else
            {
                await bc.SendReply("Error: Invalid skill name.");
                return;
            }

            if (goal == null)
            {
                // Get goal from database
                goal = await Database.GetStringParameter("users", "goals", "fingerprint='" + bc.From.FingerPrint + "'", skill, "nl");
            }
            else if (!Regex.Match(goal, @"n(?:l|r)|r?\d+(?:\.\d+)?(?:m|k)?").Success)
            {
                await bc.SendReply("Error: Invalid goal.");
                return;
            }

            // Add this player to database if he never set a default name.
            if (await Database.Lookup<long>("COUNT(*)", "users", "fingerprint=@fp", new[] { new MySqlParameter("@fp", bc.From.FingerPrint) }) < 1)
            {
                Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", await bc.GetPlayerName(bc.From.Nickname));
            }

            if (goal.EqualsI("nl"))
            {
                await bc.SendReply(@"Your goal for \b{0}\b is currently set to \unext level\u.", skill);
            }
            else if (goal.EqualsI("nr"))
            {
                await bc.SendReply(@"Your goal for \b{0}\b is currently set to \unext rank\u.", skill);
            }
            else if (goal.StartsWithI("r"))
            {
                int goalRank = goal.Substring(1).ToInt32();
                if (goalRank > 0 && goalRank <= 2000000)
                {
                    await bc.SendReply(@"Your goal for \b{0}\b is currently set to \urank {1}\u.", skill, goalRank.ToShortString(1));
                    goal = "r" + goalRank.ToStringI();
                }
                else
                {
                    await bc.SendReply(@"Your goal for \b{0}\b is currently set to \unext rank\u.", skill);
                    goal = "nr";
                }
            }
            else
            {
                int goalLevel = goal.ToInt32();
                if (goalLevel > 1 && goalLevel < 127)
                {
                    await bc.SendReply(@"Your goal for \b{0}\b is currently set to \ulevel {1}\u.", skill, goalLevel);
                    goal = goalLevel.ToStringI();
                }
                else if (goalLevel == 127)
                {
                    await bc.SendReply(@"Your goal for \b{0}\b is currently set to \u200m exp\u.", skill);
                    goal = goalLevel.ToStringI();
                }
                else if (goalLevel > 127 && goalLevel <= 200000000)
                {
                    await bc.SendReply(@"Your goal for \b{0}\b is currently set to \u{1} exp\u.", skill, goalLevel.ToShortString(1));
                    goal = goalLevel.ToStringI();
                }
                else
                {
                    await bc.SendReply(@"Your goal for \b{0}\b is currently set to \unext level\u.", skill);
                    goal = "nl";
                }
            }
            await Database.SetStringParameter("users", "goals", "fingerprint='" + bc.From.FingerPrint + "'", skill, goal);
        }

        private static async Task _SetItem(CommandContext bc)
        {
            if (bc.MessageTokens.Length < 4)
            {
                await bc.SendReply("Syntax: !set item <skill> <item>");
                return;
            }

            string skill = Skill.OVER;
            if (!Skill.TryParse(bc.MessageTokens[2], ref skill))
            {
                await bc.SendReply("Error: Invalid skill name.");
                return;
            }

            string item = bc.MessageTokens.Join(3).Replace(";", string.Empty).ToLowerInvariant();

            // Add this player to database if he never set a default name.
            if (await Database.Lookup<long>("COUNT(*)", "users", "fingerprint=@fp", new[] { new MySqlParameter("@fp", bc.From.FingerPrint) }) < 1)
            {
                Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", await bc.GetPlayerName(bc.From.Nickname));
            }

            await Database.SetStringParameter("users", "items", "fingerprint='" + bc.From.FingerPrint + "'", skill, item);
            await bc.SendReply(@"Your default item for \b{0}\b is currently set to \u{1}\u.", skill, item);
        }

        private static async Task _SetSpeed(CommandContext bc)
        {
            if (bc.MessageTokens.Length < 4)
            {
                await bc.SendReply("Syntax: !set speed <skill> <average exp. per hour>");
                return;
            }

            string skill = Skill.OVER;
            if (!Skill.TryParse(bc.MessageTokens[2], ref skill))
            {
                await bc.SendReply("Error: Invalid skill name.");
                return;
            }

            string speed = bc.MessageTokens[3].ToLowerInvariant();
            if (!Regex.Match(speed, @"\d+(?:\.\d+)?(?:m|k)?").Success)
            {
                await bc.SendReply("Error: Invalid average exp. per hour.");
                return;
            }

            // Add this player to database if he never set a default name.
            if (await Database.Lookup<long>("COUNT(*)", "users", "fingerprint=@fp", new[] { new MySqlParameter("@fp", bc.From.FingerPrint) }) < 1)
            {
                Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", await bc.GetPlayerName(bc.From.Nickname));
            }

            int speedValue = speed.ToInt32();
            await Database.SetStringParameter("users", "speeds", "fingerprint='" + bc.From.FingerPrint + "'", skill, speedValue.ToStringI());

            if (speedValue > 0)
            {
                await bc.SendReply(@"Your speed for \b{0}\b is currently set to \u{1} average exp. per hour\u.", skill, speedValue.ToShortString(1));
            }
            else
            {
                await bc.SendReply(@"Your speed for \b{0}\b was deleted.", skill);
            }
        }

        private static async Task _SetSkillToggle(CommandContext bc)
        {
            if (bc.MessageTokens.Length < 3 || (bc.MessageTokens[2].ToLowerInvariant() != "on" && bc.MessageTokens[2].ToLowerInvariant() != "off"))
            {
                await bc.SendReply("Syntax: !set @ <on|off>");
                return;
            }
            string state = bc.MessageTokens[2].ToLowerInvariant();

            // Add this player to database if he never set a default name.
            if (await Database.Lookup<long>("COUNT(*)", "users", "fingerprint=@fp", new[] { new MySqlParameter("@fp", bc.From.FingerPrint) }) < 1)
            {
                Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", await bc.GetPlayerName(bc.From.Nickname));
            }
            string publicSkill = "1";
            if (state == "off")
            {
                publicSkill = "0";
            }
            Database.Update("users", "fingerprint='" + bc.From.FingerPrint + "'", "publicSkill", publicSkill);
            await bc.SendReply("Your public trigger-only command have been turned " + state + ".");
        }

        private static async Task _SetSkill(CommandContext bc)
        {
            if (bc.MessageTokens.Length < 3)
            {
                await bc.SendReply("Syntax: !set skill <skill>");
                return;
            }

            if (bc.MessageTokens[2].ToLowerInvariant() == "on" || bc.MessageTokens[2].ToLowerInvariant() == "off")
            {
                await _SetSkillToggle(bc);
                return;
            }

            string skill = Skill.OVER;
            if (!Skill.TryParse(bc.MessageTokens[2], ref skill))
            {
                await bc.SendReply("Error: Invalid skill name.");
                return;
            }

            // Add this player to database if he never set a default name.
            if (await Database.Lookup<long>("COUNT(*)", "users", "fingerprint=@fp", new[] { new MySqlParameter("@fp", bc.From.FingerPrint) }) < 1)
            {
                Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", await bc.GetPlayerName(bc.From.Nickname));
            }

            Database.Update("users", "fingerprint='" + bc.From.FingerPrint + "'", "skill", skill);
            await bc.SendReply(@"Your default skill is currently set to \b{0}\b.", skill);
        }
    }
}
