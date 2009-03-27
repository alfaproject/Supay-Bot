namespace BigSister {
  partial class Command {

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
          break;
        case "ITEM":
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

  } //class Command
} //namespace BigSister