using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BigSister {
  public class Util {

    public static string FormatShort(double value, int decimals) {
      string format = "#,##0." + new string('#', decimals);
      if (value >= 1000000 || value <= -1000000) {
        return Math.Round(value / 1000000, decimals).ToString(format, CultureInfo.InvariantCulture) + "m";
      } else if (value >= 1000 || value <= -1000) {
        return Math.Round(value / 1000, decimals).ToString(format, CultureInfo.InvariantCulture) + "k";
      } else {
        return Math.Round(value, decimals).ToString(format, CultureInfo.InvariantCulture);
      }
    }

    public static string FormatShort(double value) {
      return FormatShort(value, 0);
    }

    public static DateTime StrToDateTime(string s) {
      switch (s.Length) {
        case 8: //yyyyMMdd
          return new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2)));
        case 12: //yyyyMMddHHmm
          return new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2)), int.Parse(s.Substring(8, 2)), int.Parse(s.Substring(10, 2)), 0);
        case 14: //yyyyMMddHHmmss
          return new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2)), int.Parse(s.Substring(8, 2)), int.Parse(s.Substring(10, 2)), int.Parse(s.Substring(12, 2)));
        default:
          return DateTime.MinValue;
      }
    }

    public static bool TryCalc(string s, out double result_value) {
      result_value = 0;

      if (s == null)
        return false;

      s = s.Trim();
      if (s.Length == 0)
        return false;

      s = s.Replace(",", string.Empty).Replace(" ", string.Empty);

      // evaluate and output the result
      MathParser c = new MathParser();
      c.Evaluate(s);
      if (c.Value != null) {
        result_value = (double)c.Value;
        return true;
      }

      return false;
    }

    public static string ClearHTML(string html) {
      return Regex.Replace(html, "<[^>]+>", string.Empty, RegexOptions.Singleline);
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