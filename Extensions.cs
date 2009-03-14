using System;
using System.Globalization;

namespace BigSister {
  public static partial class Extensions {

    /// <summary>
    ///   Concatenates a specified separator string between each element of a specified string array, yielding a single concatenated string. </summary>
    /// <param name="startIndex">
    ///   The first array element in value to use. </param>
    /// <param name="separator">
    ///   A System.String. </param>
    public static string Join(this string[] value, int startIndex, string separator) {
      if (value.Length == startIndex + 1) {
        return value[startIndex];
      } else {
        return string.Join(separator, value, startIndex, value.Length - startIndex);
      }
    }

    /// <summary>
    ///   Concatenates a space between each element of a specified string array, yielding a single concatenated string. </summary>
    /// <param name="startIndex">
    ///   The first array element in value to use. </param>
    public static string Join(this string[] value, int startIndex) {
      return value.Join(startIndex, " ");
    }

    /// <summary>
    ///   Concatenates a space between each element of a specified string array, yielding a single concatenated string. </summary>
    public static string Join(this string[] value) {
      return value.Join(0);
    }

    /// <summary>
    ///   Converts the specified string representation of a date (and time) to its DateTime equivalent. </summary>
    public static DateTime ToDateTime(this string s) {
      switch (s.Length) {
        case 8:
          return DateTime.ParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture);
        case 12:
          return DateTime.ParseExact(s, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
        case 14:
          return DateTime.ParseExact(s, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        default:
          return DateTime.MinValue;
      }
    }

    /// <summary>
    ///   Converts the numeric value of this instance to its equivalent short string (k/m/b) representation up to a maximum number of decimals. </summary>
    /// <param name="decimals">
    ///   Max number of decimals allowed. </param>
    public static string ToShortString(this double value, int decimals) {
      string format = "#,##0." + new string('#', decimals);
      if (value >= 1000000000 || value <= -1000000000) {
        return Math.Round(value / 1000000000, decimals).ToString(format, CultureInfo.InvariantCulture) + "b";
      } else if (value >= 1000000 || value <= -1000000) {
        return Math.Round(value / 1000000, decimals).ToString(format, CultureInfo.InvariantCulture) + "m";
      } else if (value >= 1000 || value <= -1000) {
        return Math.Round(value / 1000, decimals).ToString(format, CultureInfo.InvariantCulture) + "k";
      } else {
        return Math.Round(value, decimals).ToString(format, CultureInfo.InvariantCulture);
      }
    }

    /// <summary>
    ///   Converts the numeric value of this instance to its equivalent short string (k/m/b) representation up to a maximum number of decimals. </summary>
    /// <param name="decimals">
    ///   Max number of decimals allowed. </param>
    public static string ToShortString(this int value, int decimals) {
      return ((double)value).ToShortString(decimals);
    }

  } //class Extensions
} //namespace BigSister