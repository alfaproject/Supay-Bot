using System;

namespace Supay.Bot {
  static partial class Command {

    public static void LastUpdate(CommandContext bc) {
      DateTime lastUpdate = Database.GetString("SELECT lastUpdate FROM prices ORDER BY lastUpdate DESC LIMIT 1;", DateTime.UtcNow.ToStringI("yyyyMMddHHmm")).ToDateTime();
      bc.SendReply("The GE was last updated \\c07{0}\\c ago. ({1:R})".FormatWith((DateTime.UtcNow - lastUpdate).ToLongString(), lastUpdate));
    }

  } //class Command
} ////namespace Supay.Bot