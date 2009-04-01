using System.Text.RegularExpressions;

namespace BigSister {
  static partial class Command {

    public static void Set(CommandContext bc) {
      if (bc.MessageTokens.Length < 2) {
        bc.SendReply("Syntax: !set <param> [skill] <value>");
        return;
      }

      switch (bc.MessageTokens[1].ToUpperInvariant()) {
        case "RSN":
        case "NAME":
          SetName(bc);
          break;
        case "GOAL":
          SetGoal(bc);
          break;
        case "ITEM":
          SetItem(bc);
          break;
        case "SPEED":
          SetSpeed(bc);
          break;
        case "SKILL":
          SetSkill(bc);
          break;
        default:
          bc.SendReply("Error: Unknown parameter.");
          break;
      }
    }

    private static void SetName(CommandContext bc) {
      if (bc.MessageTokens.Length < 3) {
        bc.SendReply("Syntax: !set name <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(2).ToRsn();

      // add/update to database
      if (Database.GetValue("users", "rsn", "fingerprint='" + bc.From.FingerPrint + "'") == null) {
        Database.Insert("users", "fingerprint", bc.From.FingerPrint,
                                 "rsn", rsn);
      } else {
        Database.Update("users", "fingerprint='" + bc.From.FingerPrint + "'",
                                 "rsn", rsn);
      }

      bc.SendReply("Your default RuneScape name is now \\b{0}\\b. This RSN is associated with the address \\u*!*{1}\\u.".FormatWith(rsn, bc.From.FingerPrint));
    }

    private static void SetGoal(CommandContext bc) {
      if (bc.MessageTokens.Length < 4) {
        bc.SendReply("Syntax: !set goal <skill> <goal>");
        return;
      }

      string skill = Skill.OVER;
      if (!Skill.TryParse(bc.MessageTokens[2], ref skill)) {
        bc.SendReply("Error: Invalid skill name.");
        return;
      }

      string goal = bc.MessageTokens[3].ToLowerInvariant();
      if (!Regex.Match(goal, @"n(?:l|r)|r?\d+(?:\.\d+)?(?:m|k)?").Success) {
        bc.SendReply("Error: Invalid goal.");
        return;
      }

      // Add this player to database if he never set a default name.
      if (Database.GetString("SELECT fingerprint FROM users WHERE fingerprint='" + bc.From.FingerPrint + "'", null) == null) {
        Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", bc.From.Rsn);
      }

      if (goal.EqualsI("nl")) {
        bc.SendReply(@"Your goal for \b{0}\b is now set to \unext level\u.".FormatWith(skill));
      } else if (goal.EqualsI("nr")) {
        bc.SendReply(@"Your goal for \b{0}\b is now set to \unext rank\u.".FormatWith(skill));
      } else if (goal.StartsWithI("r")) {
        int goalRank = goal.Substring(1).ToInt32();
        if (goalRank > 0 && goalRank <= 2000000) {
          bc.SendReply(@"Your goal for \b{0}\b is now set to \urank {1}\u.".FormatWith(skill, goalRank.ToShortString(1)));
          goal = "r" + goalRank.ToString();
        } else {
          bc.SendReply(@"Your goal for \b{0}\b is now set to \unext rank\u.".FormatWith(skill));
          goal = "nr";
        }
      } else {
        int goalLevel = goal.ToInt32();
        if (goalLevel > 1 && goalLevel < 127) {
          bc.SendReply(@"Your goal for \b{0}\b is now set to \ulevel {1}\u.".FormatWith(skill, goalLevel));
          goal = goalLevel.ToString();
        } else if (goalLevel == 127) {
          bc.SendReply(@"Your goal for \b{0}\b is now set to \u200m exp\u.".FormatWith(skill));
          goal = goalLevel.ToString();
        } else if (goalLevel > 127 && goalLevel <= 200000000) {
          bc.SendReply(@"Your goal for \b{0}\b is now set to \u{1} exp\u.".FormatWith(skill, goalLevel.ToShortString(1)));
          goal = goalLevel.ToString();
        } else {
          bc.SendReply(@"Your goal for \b{0}\b is now set to \unext level\u.".FormatWith(skill));
          goal = "nl";
        }
      }
      Database.SetStringParam("users", "goals", "fingerprint='" + bc.From.FingerPrint + "'", skill, goal);
    }

    private static void SetItem(CommandContext bc) {
      if (bc.MessageTokens.Length < 4) {
        bc.SendReply("Syntax: !set item <skill> <item>");
        return;
      }

      string skill = Skill.OVER;
      if (!Skill.TryParse(bc.MessageTokens[2], ref skill)) {
        bc.SendReply("Error: Invalid skill name.");
        return;
      }

      string item = bc.MessageTokens.Join(3).Replace(";", string.Empty).ToLowerInvariant();

      // Add this player to database if he never set a default name.
      if (Database.GetString("SELECT fingerprint FROM users WHERE fingerprint='" + bc.From.FingerPrint + "'", null) == null) {
        Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", bc.From.Rsn);
      }

      Database.SetStringParam("users", "items", "fingerprint='" + bc.From.FingerPrint + "'", skill, item);
      bc.SendReply(@"Your default item for \b{0}\b is now set to \u{1}\u.".FormatWith(skill, item));
    }

    private static void SetSpeed(CommandContext bc) {
      if (bc.MessageTokens.Length < 4) {
        bc.SendReply("Syntax: !set speed <skill> <average exp. per hour>");
        return;
      }

      string skill = Skill.OVER;
      if (!Skill.TryParse(bc.MessageTokens[2], ref skill)) {
        bc.SendReply("Error: Invalid skill name.");
        return;
      }

      string speed = bc.MessageTokens[3].ToLowerInvariant();
      if (!Regex.Match(speed, @"\d+(?:\.\d+)?(?:m|k)?").Success) {
        bc.SendReply("Error: Invalid average exp. per hour.");
        return;
      }

      // Add this player to database if he never set a default name.
      if (Database.GetString("SELECT fingerprint FROM users WHERE fingerprint='" + bc.From.FingerPrint + "'", null) == null) {
        Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", bc.From.Rsn);
      }

      int speedValue = speed.ToInt32();
      Database.SetStringParam("users", "speeds", "fingerprint='" + bc.From.FingerPrint + "'", skill, speedValue.ToStringI());

      if (speedValue > 0) {
        bc.SendReply(@"Your speed for \b{0}\b is now set to \u{1} average exp. per hour\u.".FormatWith(skill, speedValue.ToShortString(1)));
      } else {
        bc.SendReply(@"Your speed for \b{0}\b is now deleted.".FormatWith(skill));
      }
    }

    private static void SetSkill(CommandContext bc) {
      if (bc.MessageTokens.Length < 3) {
        bc.SendReply("Syntax: !set skill <skill>");
        return;
      }

      string skill = Skill.OVER;
      if (!Skill.TryParse(bc.MessageTokens[2], ref skill)) {
        bc.SendReply("Error: Invalid skill name.");
        return;
      }

      // Add this player to database if he never set a default name.
      if (Database.GetString("SELECT fingerprint FROM users WHERE fingerprint='" + bc.From.FingerPrint + "'", null) == null) {
        Database.Insert("users", "fingerprint", bc.From.FingerPrint, "rsn", bc.From.Rsn);
      }

      Database.Update("users", "fingerprint='" + bc.From.FingerPrint + "'", "skill", skill);
      bc.SendReply(@"Your default skill is now set to \b{0}\b.".FormatWith(skill));
    }

  } //class Command
} //namespace BigSister