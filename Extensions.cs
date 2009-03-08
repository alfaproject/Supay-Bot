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

  } //class Extensions
} //namespace BigSister