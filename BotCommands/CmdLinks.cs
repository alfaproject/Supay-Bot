using System.Text.RegularExpressions;

namespace BigSister {
  class CmdLinks {

    public static void Qfc(CommandContext bc) {
      if (bc.MessageTokens.Length == 1) {
        bc.SendReply("Syntax: !Qfc <qfc>");
        return;
      }

      Match qfc = Regex.Match(Util.JoinTokens(bc.MessageTokens, 1), "(\\d+).(\\d+).(\\d+).(\\d+)");
      if (!qfc.Success) {
        bc.SendReply("Syntax: !Qfc <qfc>");
      } else {
        bc.SendReply(string.Format("Quick find code \\c07{0}-{1}-{2}-{3}\\c: \\c12http://forum.runescape.com/forums.ws?{0},{1},{2},{3}\\c", qfc.Groups[1].Value, qfc.Groups[2].Value, qfc.Groups[3].Value, qfc.Groups[4].Value));
      }
    }

  } //class CmdLinks
} //namespace BigSister