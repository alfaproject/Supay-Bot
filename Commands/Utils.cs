using Supay.Irc;

namespace Supay.Bot {
  static partial class Utils {

    public static bool UserIsAdmin(User user) {
      if (user.Nickname.EqualsI("_aLfa_") || user.Nickname.EqualsI("_aLfa_|laptop") || user.Nickname.EqualsI("P_Gertrude")) {
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