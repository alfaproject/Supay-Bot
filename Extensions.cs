using System;
using System.Globalization;
using System.Text;

namespace BigSister {
  public static partial class Extensions {

    /// <summary>
    ///   Concatenates a specified separator string between each element of a specified string array, yielding a single concatenated string. </summary>
    /// <param name="startIndex">
    ///   The first array element in value to use. </param>
    /// <param name="separator">
    ///   A System.String. </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "startIndex+1")]
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
    public static DateTime ToDateTime(this string date) {
      switch (date.Length) {
        case 8:
          return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        case 12:
          return DateTime.ParseExact(date, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
        case 14:
          return DateTime.ParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
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

    /// <summary>
    ///   Returns the string representation of the value of this instance in the format: ##days ##hours ##mins ##secs. </summary>
    public static string ToLongString(this TimeSpan timeSpan) {
      StringBuilder result = new StringBuilder(30);
      if (timeSpan.Days > 0)
        result.Append(timeSpan.Days + "day" + (timeSpan.Days == 1 ? " " : "s "));
      if (timeSpan.Hours > 0)
        result.Append(timeSpan.Hours + "hour" + (timeSpan.Hours == 1 ? " " : "s "));
      if (timeSpan.Minutes > 0)
        result.Append(timeSpan.Minutes + "min" + (timeSpan.Minutes == 1 ? " " : "s "));
      result.Append(timeSpan.Seconds + "sec" + (timeSpan.Seconds == 1 ? string.Empty : "s"));
      return result.ToString();
    }

  } //class Extensions
} //namespace BigSister