using System;

namespace System {
  public static class Math2 {

    public static double Factorial(double number) {
      double ret = 1.0;
      for (int i = 2; i <= Math.Floor(number); i++)
        ret *= (double)i;
      return ret;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "n"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r")]
    public static double Permutation(double n, double r) {
      return Factorial(n) / Factorial(n - r);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "n")]
    public static double Combination(double n, double r) {
      return Factorial(n) / (Factorial(r) * Factorial(n - r));
    }

    public static double CelsiusToFahrenheit(double celsius) {
      return (celsius * 9.0 / 5.0) + 32.0;
    }

    public static double FahrenheitToCelsius(double fahrenheit) {
      return (fahrenheit - 32.0) * 5.0 / 9.0;
    }

    public static long GreatestCommonDivisor(long number1, long number2) {
      long divisor = number2;
      while (number1 > 0) {
        divisor = number1;
        number1 = number2 % number1;
        number2 = divisor;
      }
      return divisor;
    }

  }
}