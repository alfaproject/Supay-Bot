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
      return Join(value, startIndex, " ");
    }

    /// <summary>
    ///   Concatenates a space between each element of a specified string array, yielding a single concatenated string. </summary>
    public static string Join(this string[] value) {
      return Join(value, 0, " ");
    }

    /// <summary>
    ///   Converts the specified string representation of a date (and time) to its DateTime equivalent. </summary>
    /// <param name="s">
    ///   A string containing a date (and time) to convert. </param>
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

  } //class Extensions
} //namespace BigSister