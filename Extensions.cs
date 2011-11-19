using System;
using System.Globalization;
using System.Text;

namespace Supay.Bot
{
  internal static partial class Extensions
  {
    /// <summary>
    ///   Concatenates a specified separator string between each element of a specified string array, yielding a single concatenated string. </summary>
    /// <param name="startIndex">
    ///   The first array element in value to use. </param>
    /// <param name="separator">
    ///   A System.String. </param>
    public static string Join(this string[] self, int startIndex, string separator)
    {
      if (self.Length == startIndex + 1)
      {
        return self[startIndex];
      }
      return string.Join(separator, self, startIndex, self.Length - startIndex);
    }

    /// <summary>
    ///   Concatenates a space between each element of a specified string array, yielding a single concatenated string. </summary>
    /// <param name="startIndex">
    ///   The first array element in value to use. </param>
    public static string Join(this string[] self, int startIndex)
    {
      return self.Join(startIndex, " ");
    }

    /// <summary>
    ///   Concatenates a space between each element of a specified string array, yielding a single concatenated string. </summary>
    public static string Join(this string[] self)
    {
      return self.Join(0);
    }

    /// <summary>
    ///   Converts the specified string representation of a date (and time) to its DateTime equivalent. </summary>
    public static DateTime ToDateTime(this string self)
    {
      if (!string.IsNullOrEmpty(self))
      {
        switch (self.Length)
        {
          case 8:
            return DateTime.ParseExact(self, "yyyyMMdd", CultureInfo.InvariantCulture);
          case 12:
            return DateTime.ParseExact(self, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
          case 14:
            return DateTime.ParseExact(self, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }
      }
      return DateTime.MinValue;
    }

    /// <summary>
    ///   Converts the numeric value of this instance to its equivalent short string (k/m/b) representation up to a maximum number of decimals. </summary>
    /// <param name="decimals">
    ///   Max number of decimals allowed. </param>
    public static string ToShortString(this double self, int decimals)
    {
      string format = "#,##0." + new string('#', decimals);
      if (self >= 1000000000 || self <= -1000000000)
      {
        return Math.Round(self / 1000000000, decimals).ToStringI(format) + "b";
      }
      if (self >= 1000000 || self <= -1000000)
      {
        return Math.Round(self / 1000000, decimals).ToStringI(format) + "m";
      }
      if (self >= 1000 || self <= -1000)
      {
        return Math.Round(self / 1000, decimals).ToStringI(format) + "k";
      }
      return Math.Round(self, decimals).ToStringI(format);
    }

    /// <summary>
    ///   Converts the numeric value of this instance to its equivalent short string (k/m/b) representation up to a maximum number of decimals. </summary>
    /// <param name="decimals">
    ///   Max number of decimals allowed. </param>
    public static string ToShortString(this long self, int decimals)
    {
      return ToShortString((double) self, decimals);
    }

    /// <summary>
    ///   Converts the numeric value of this instance to its equivalent short string (k/m/b) representation up to a maximum number of decimals. </summary>
    /// <param name="decimals">
    ///   Max number of decimals allowed. </param>
    public static string ToShortString(this int self, int decimals)
    {
      return ToShortString((double) self, decimals);
    }

    /// <summary>
    ///   Returns the string representation of the value of this instance in the format: ##days ##hours ##mins ##secs. </summary>
    public static string ToLongString(this TimeSpan self)
    {
      var result = new StringBuilder(30);
      if (self.Days > 0)
      {
        result.Append(self.Days + "day" + (self.Days == 1 ? " " : "s "));
      }
      if (self.Hours > 0)
      {
        result.Append(self.Hours + "hour" + (self.Hours == 1 ? " " : "s "));
      }
      if (self.Minutes > 0)
      {
        result.Append(self.Minutes + "min" + (self.Minutes == 1 ? " " : "s "));
      }
      if (self.Seconds > 0 || (self.Days == 0 && self.Hours == 0 && self.Minutes == 0))
      {
        result.Append(self.Seconds + "sec" + (self.Seconds == 1 ? string.Empty : "s"));
      }
      return result.ToString();
    }

    /// <summary>
    ///   Returns a value indicating whether the specified System.String object occurs within this string. (case insensitive) </summary>
    /// <param name="value">
    ///   The System.String object to seek. </param>
    public static bool ContainsI(this string self, string value)
    {
      return self.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
    }

    /// <summary>
    ///   Determines whether the beginning of this instance matches the specified string. (case insensitive) </summary>
    /// <param name="value">
    ///   The System.String object to seek. </param>
    public static bool StartsWithI(this string self, string value)
    {
      return self.StartsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///   Determines whether the ending of this instance matches the specified string. (case insensitive) </summary>
    /// <param name="value">
    ///   The System.String object to seek. </param>
    public static bool EndsWithI(this string self, string value)
    {
      return self.EndsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///   Determines whether this instance and another specified String object have the same value. (case insensitive)</summary>
    public static bool EqualsI(this string self, string value)
    {
      return self.Equals(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///   Replaces the format items of this instance with the text equivalent of the value of a corresponding Object instance in a specified array. </summary>
    /// <param name="args">
    ///   An Object array containing zero or more objects to format. </param>
    public static string FormatWith(this string self, params object[] args)
    {
      return string.Format(CultureInfo.InvariantCulture, self, args);
    }

    /// <summary>
    ///   Converts the value of this instance to its equivalent string representation. </summary>
    public static string ToStringI(this char self)
    {
      return self.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///   Converts the value of this instance to its equivalent string representation. </summary>
    public static string ToStringI<T>(this T self) where T : IFormattable
    {
      return self.ToString(null, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///   Formats the value of the current instance using the specified format. </summary>
    /// <param name="format">
    ///   The System.String specifying the format to use. </param>
    public static string ToStringI<T>(this T self, string format) where T : IFormattable
    {
      return self.ToString(format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///   Converts the string representation of a number to it's 32-bit signed integer equivalent. </summary>
    public static int ToInt32(this string self)
    {
      string number = self.TrimEnd();

      int multiplier = 1;
      switch (number[number.Length - 1])
      {
        case 'm':
        case 'M':
          number = number.Substring(0, number.Length - 1);
          multiplier = 1000000;
          break;
        case 'k':
        case 'K':
          number = number.Substring(0, number.Length - 1);
          multiplier = 1000;
          break;
      }

      return (int) (multiplier * double.Parse(number, NumberStyles.AllowLeadingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///   Returns true if a string can be converted to a 64-bit signed integer. </summary>
    public static bool TryInt64(this string self, out long value)
    {
      string number = self.TrimEnd();

      int multiplier = 1;
      switch (number[number.Length - 1])
      {
        case 'm':
        case 'M':
          number = number.Substring(0, number.Length - 1);
          multiplier = 1000000;
          break;
        case 'k':
        case 'K':
          number = number.Substring(0, number.Length - 1);
          multiplier = 1000;
          break;
      }

      double result;
      if (double.TryParse(number, NumberStyles.AllowLeadingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
      {
        value = (long) (result * multiplier);
        return true;
      }

      value = 0;
      return false;
    }

    /// <summary>
    ///   Returns true if a string can be converted to a 32-bit signed integer. </summary>
    public static bool TryInt32(this string self, out int value)
    {
      long resultValue;
      bool result = TryInt64(self, out resultValue);
      value = (int) resultValue;
      return result;
    }
  }
}
