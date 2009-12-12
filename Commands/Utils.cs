using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Supay.Bot.Irc;

namespace Supay.Bot {
  static partial class Utils {

    public static string UserToRsn(User user) {
      if (string.IsNullOrEmpty(user.FingerPrint)) {
        return user.Nick.ToRsn();
      }

      string rsn = (string)Database.GetValue("users", "rsn", "fingerprint='" + user.FingerPrint + "'");
      if (rsn == null) {
        return user.Nick.ToRsn();
      } else {
        return rsn;
      }
    }

    public static bool UserIsAdmin(User user) {
      if (user.Nick.EqualsI("_aLfa_") || user.Nick.EqualsI("_aLfa_|work") || user.Nick.EqualsI("P_Gertrude")) {
        return true;
      }

      foreach (string admin in Properties.Settings.Default.Administrators.Split(';')) {
        if (user.Nick.EqualsI(admin)) {
          return true;
        }
      }

      return false;
    }

  }
} //namespace Supay.Bot