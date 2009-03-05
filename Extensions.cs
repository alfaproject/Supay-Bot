using System;

namespace BigSister {
  public static class Extensions {

    public static int ToExp(this int level) {
      int exp = 0;
      while (level > 1)
        exp += --level + (int)(300.0 * Math.Pow(2.0, (double)level / 7.0));
      return exp / 4;
    }

    public static int ToLevel(this int exp) {
      int level = 0;
      int levelExp = 0;
      while (levelExp / 4 <= exp)
        levelExp += ++level + (int)(300.0 * Math.Pow(2.0, (double)level / 7.0));
      return level;
    }

  } //class Extensions
} //namespace BigSister