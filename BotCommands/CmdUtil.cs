using System.Globalization;

namespace BigSister {
  static class CmdUtil {

    //setname <rsn>
    public static void SetName(CommandContext bc) {
      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("Syntax: !setname <rsn>");
        return;
      }

      string rsn = bc.MessageTokens.Join(1).ToRsn();

      // add/update to database
      if (Database.GetValue("users", "rsn", "fingerprint='" + bc.From.FingerPrint + "'") == null) {
        Database.Insert("users", "fingerprint", bc.From.FingerPrint,
                                 "rsn", rsn);
      } else {
        Database.Update("users", "fingerprint='" + bc.From.FingerPrint + "'",
                                 "rsn", rsn);
      }
      bc.From.RSN = rsn;

      bc.SendReply("Your default RuneScape name is now \\b{0}\\b. This RSN is associated with the address \\u*!*{1}\\u.".FormatWith(rsn, bc.From.FingerPrint));
    }

    //whois <nick>
    public static void Whois(CommandContext bc) {
      if (bc.MessageTokens.Length <= 1) {
        bc.SendReply("{0}'s RSN is \\b{1}\\b.".FormatWith(bc.From.Nick, bc.From.RSN));
        return;
      }

      string nick = bc.MessageTokens.Join(1, "_");
      BigSister.Irc.User u = bc.Users.Find(nick);
      if (u != null)
        bc.SendReply("{0}'s RSN is \\b{1}\\b.".FormatWith(u.Nick, u.RSN));
      else
        bc.SendReply("\\c07{0}\\c must be in a channel monitored by the bot for you to look up their RSN.".FormatWith(nick));
    }

    //calc <expression>
    public static void Calc(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !calc <expression>");
        return;
      }

      MathParser mp = new MathParser();
      mp.Evaluate(bc.MessageTokens.Join(1));
      bc.SendReply("\\c07" + mp.Expression + "\\c => \\c07" + mp.ValueAsString + "\\c");
    }

  } //class CmdUtils
} //namespace BigSister