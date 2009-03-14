using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BigSister {
  public class Util {

    public static bool TryCalc(string s, out double result_value) {
      result_value = 0;

      if (s == null)
        return false;

      s = s.Trim();
      if (s.Length == 0)
        return false;

      // evaluate and output the result
      MathParser c = new MathParser();
      c.Evaluate(s);
      if (c.Value != null) {
        result_value = (double)c.Value;
        return true;
      }

      return false;
    }

    public static string FormatTimeSpan(TimeSpan TimeSpan) {
      string ret = string.Format("{0}sec" + (TimeSpan.Seconds == 1 ? string.Empty : "s"), TimeSpan.Seconds);
      if (TimeSpan.Minutes > 0 || TimeSpan.Hours > 0 || TimeSpan.Days > 0)
        ret = string.Format("{0}min", TimeSpan.Minutes) + (TimeSpan.Minutes == 1 ? string.Empty : "s") + " " + ret;
      if (TimeSpan.Hours > 0 || TimeSpan.Days > 0)
        ret = string.Format("{0}hour", TimeSpan.Hours) + (TimeSpan.Hours == 1 ? string.Empty : "s") + " " + ret;
      if (TimeSpan.Days > 0)
        ret = string.Format("{0}day", TimeSpan.Days) + (TimeSpan.Days == 1 ? string.Empty : "s") + " " + ret;

      if ((TimeSpan.Minutes > 0 || TimeSpan.Hours > 0 || TimeSpan.Days > 0) && TimeSpan.Seconds == 0)
        ret = ret.Substring(0, ret.Length - 6);

      return ret;
    }

    public static int ParseNumber(string number) {
      switch (number.Substring(number.Length - 1).ToUpperInvariant()) {
        case "M":
          return (int)(1000000 * double.Parse(number.Substring(0, number.Length - 1), NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));
        case "K":
          return (int)(1000 * double.Parse(number.Substring(0, number.Length - 1), NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));
        default:
          return int.Parse(number, NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
      }
    }

  } //class Util
} //namespace BigSister