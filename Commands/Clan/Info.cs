using System;

namespace BigSister {
  static partial class Command {

    public static void ClanInfo(CommandContext bc) {
      string skill = Skill.OVER;
      if (bc.MessageTokens.Length > 1)
        Skill.TryParse(bc.MessageTokens[1], ref skill);

      int totallevel = 0;
      long totalexp = 0;
      Players ssplayers = new Players("SS");
      foreach (Player p in ssplayers) {
        if (p.Ranked) {
          totallevel += p.Skills[skill].Level;
          totalexp += p.Skills[skill].Exp;
        }
      }

      bc.SendReply("\\bSupreme Skillers\\b | Forum: \\c12www.supremeskillers.com\\c | \\u{0}\\u average level: \\c07{1}\\c (\\c07{2:N0}\\c average exp.) | Members (\\c07{3}\\c): \\c12http://runehead.com/clans/ml.php?clan=lovvel\\c".FormatWith(skill, totallevel / ssplayers.Count, totalexp / ssplayers.Count, ssplayers.Count));
    }

  } //class Command
} //namespace BigSister