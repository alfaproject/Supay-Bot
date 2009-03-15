using System.Globalization;

namespace BigSister {
  class Util {

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