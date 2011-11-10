using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Supay.Bot {
    static partial class Command {

        public static void Effigies(CommandContext bc) {
            // .effigy <lvl>
            int level = 0, Exp = 0;
            if (bc.MessageTokens.Length != 2 || bc.MessageTokens[1].TryInt32(out level) == false) {
                bc.SendReply("Syntax: !effigy <level>");
                return;
            }
            Exp = (int)((Math.Pow(level, 3) - 2 * Math.Pow(level, 2) + 100 * level) / 20);
            string reply = @"A Dragonkin Lamp used on a skill of Level \c07{0}\c will give \c07{1:N0}\c Experience.".FormatWith(level, Exp);
            bc.SendReply(reply);

        }

    }
}
