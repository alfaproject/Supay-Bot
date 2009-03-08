namespace BigSister {
  class CmdUtil {

    //setname <rsn>
    public static void SetName(CommandContext bc) {
      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !setname <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ToRSN();

      // add/update to database
      if (DataBase.GetValue("users", "rsn", "fingerprint='" + bc.From.FingerPrint + "'") == null) {
        DataBase.Insert("users", "fingerprint", bc.From.FingerPrint,
                                 "rsn", rsn);
      } else {
        DataBase.Update("users", "fingerprint='" + bc.From.FingerPrint + "'",
                                 "rsn", rsn);
      }
      bc.From.RSN = rsn;

      bc.SendReply(string.Format("Your default RuneScape name is now \\b{0}\\b. This RSN is associated with the address \\u*!*{1}\\u.", rsn, bc.From.FingerPrint));
    }

    //whois <nick>
    public static void Whois(CommandContext bc) {
      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply(string.Format("{0}'s RSN is \\b{1}\\b.", bc.From.Nick, bc.From.RSN));
        return;
      }

      string nick = bc.MessageTokens.Join(1, "_");
      BigSister.Irc.User u = bc.Users.Find(nick);
      if (u != null)
        bc.SendReply(string.Format("{0}'s RSN is \\b{1}\\b.", u.Nick, u.RSN));
      else
        bc.SendReply(string.Format("\\c07{0}\\c must be in a channel monitored by the bot for you to look up their RSN.", nick));
    }

    //calc <expression>
    public static void Calc(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !calc <expression>");
        return;
      }

      string input = bc.MessageTokens.Join(1).Replace(",", string.Empty).Replace(" ", string.Empty);
      MathParser mp = new MathParser();
      mp.Evaluate(input);
      bc.SendReply("\\c07" + mp.Expression + "\\c => \\c07" + mp.ValueAsString + "\\c");
    }

  } //class CmdUtils
} //namespace BigSister