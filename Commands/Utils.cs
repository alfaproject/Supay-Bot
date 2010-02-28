using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Supay.Irc;

namespace Supay.Bot {
  static partial class Utils {

    public static string UserToRsn(User user) {
      if (string.IsNullOrEmpty(user.FingerPrint)) {
        return user.Nickname.ToRsn();
      }

      string rsn = (string)Database.GetValue("users", "rsn", "fingerprint='" + user.FingerPrint + "'");
      if (rsn == null) {
        return user.Nickname.ToRsn();
      } else {
        return rsn;
      }
    }

    public static bool UserIsAdmin(User user) {
      if (user.Nickname.EqualsI("_aLfa_") || user.Nickname.EqualsI("_aLfa_|work") || user.Nickname.EqualsI("P_Gertrude")) {
        return true;
      }

      foreach (string admin in Properties.Settings.Default.Administrators.Split(';')) {
        if (user.Nickname.EqualsI(admin)) {
          return true;
        }
      }

      return false;
    }

  }
} //namespace Supay.Bot