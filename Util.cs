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

  } //class Util
} //namespace BigSister