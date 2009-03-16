using System;
using System.Globalization;
using System.Text;

namespace BigSister {
  public static partial class Extensions {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public static string ToRSN(this string rsn) {
      StringBuilder fixedRSN = new StringBuilder(rsn.Substring(0, Math.Min(12, rsn.Length)).ToLowerInvariant(), 12);
      bool toUpper = true;
      for (int i = 0; i < fixedRSN.Length; i++) {
        if ((fixedRSN[i] >= 'a' && fixedRSN[i] <= 'z') || (fixedRSN[i] >= '0' && fixedRSN[i] <= '9')) {
          if (toUpper) {
            fixedRSN[i] = char.ToUpperInvariant(fixedRSN[i]);
            toUpper = false;
          }
        } else {
          fixedRSN[i] = '_';
          toUpper = true;
        }
      }
      return fixedRSN.ToString();
    }

    public static int ToExp(this int level) {
      int exp = 0;
      while (level > 1)
        exp += --level + (int)(300.0 * Math.Pow(2.0, (double)level / 7.0));
      return exp / 4;
    }

    public static int ToLevel(this int exp) {
      int level = 0;
      int levelExp = 0;
      while (levelExp / 4 <= exp)
        levelExp += ++level + (int)(300.0 * Math.Pow(2.0, (double)level / 7.0));
      return level;
    }

  } //class Extensions
} //namespace BigSister