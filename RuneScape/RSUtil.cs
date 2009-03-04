using System;
using System.Collections.Generic;
using System.Text;

namespace BigSister {
  public class RSUtil {
    
    public static string FixRSN(string rsn) {
      string fixedRSN = string.Empty;
      bool toUpper = true;
      foreach (char c in rsn.ToLowerInvariant()) {
        if (c >= 'a' && c <= 'z') {
          if (toUpper) {
            fixedRSN += c.ToString().ToUpperInvariant();
            toUpper = false;
          } else {
            fixedRSN += c;
          }
        } else if (c >= '0' && c <= '9') {
          fixedRSN += c;
        } else {
          fixedRSN += '_';
          toUpper = true;
        }
      }
      return fixedRSN.Substring(0, Math.Min(12, fixedRSN.Length));
    }

    public static int CalculateCombat(int Att, int Str, int Def, int Hp, int Ran, int Pr, int Mag, int Sum, out string CombatClass) {
      // Get combat bonus
      int NeutralBonus = Def + Hp + Pr / 2 + Sum / 2;
      int MeleeBonus = Att + Str;
      int MagicBonus = Mag + Mag / 2;
      int RangeBonus = Ran + Ran / 2;

      // Return combat class for this set of skills
      if (MeleeBonus > MagicBonus & MeleeBonus > RangeBonus)
        CombatClass = "Warrior";
      else if (MagicBonus > MeleeBonus & MagicBonus > RangeBonus)
        CombatClass = "Mage";
      else if (RangeBonus > MeleeBonus & RangeBonus > MagicBonus)
        CombatClass = "Ranger";
      else
        CombatClass = "Hybrid";

      // Return combat level
      return (int)Math.Floor((NeutralBonus * 100 + Math.Max(MeleeBonus, Math.Max(MagicBonus, RangeBonus)) * 130) / 400.0);
    }

    public static int CalculateCombat(int Att, int Str, int Def, int Hp, int Ran, int Pr, int Mag, int Sum) {
      // Get combat bonus 
      int NeutralBonus = Def + Hp + Pr / 2 + Sum / 2;
      int MeleeBonus = Att + Str;
      int MagicBonus = Mag + Mag / 2;
      int RangeBonus = Ran + Ran / 2;

      // Return combat level 
      return (int)Math.Floor((NeutralBonus * 100 + Math.Max(MeleeBonus, Math.Max(MagicBonus, RangeBonus)) * 130) / 400.0);
    }

    public static int CalculateF2pCombat(int Att, int Str, int Def, int Hp, int Ran, int Pr, int Mag) {
      // Get combat bonus
      int NeutralBonus = Def + Hp + Pr / 2;
      int MeleeBonus = Att + Str;
      int MagicBonus = Mag + Mag / 2;
      int RangeBonus = Ran + Ran / 2;

      // Return combat level
      return (int)Math.Floor((NeutralBonus * 100 + Math.Max(MeleeBonus, Math.Max(MagicBonus, RangeBonus)) * 130) / 400.0);
    }

    public static int NextCombatAttStr(int A, int S, int D, int H, int R, int P, int M, int Sum) {
      int initial_att = A;
      int initial_combat = CalculateCombat(A, S, D, H, R, P, M, Sum);
      while (CalculateCombat(++A, S, D, H, R, P, M, Sum) <= initial_combat)
        ;
      return A - initial_att;
    }

    public static int NextCombatMag(int A, int S, int D, int H, int R, int P, int M, int Sum) {
      int initial_mag = M;
      int initial_combat = CalculateCombat(A, S, D, H, R, P, M, Sum);
      while (CalculateCombat(A, S, D, H, R, P, ++M, Sum) <= initial_combat)
        ;
      return M - initial_mag;
    }

    public static int NextCombatRan(int A, int S, int D, int H, int R, int P, int M, int Sum) {
      int initial_ran = R;
      int initial_combat = CalculateCombat(A, S, D, H, R, P, M, Sum);
      while (CalculateCombat(A, S, D, H, ++R, P, M, Sum) <= initial_combat)
        ;
      return R - initial_ran;
    }

    public static int NextCombatDefHp(int A, int S, int D, int H, int R, int P, int M, int Sum) {
      int initial_def = D;
      int initial_combat = CalculateCombat(A, S, D, H, R, P, M, Sum);
      while (CalculateCombat(A, S, ++D, H, R, P, M, Sum) <= initial_combat)
        ;
      return D - initial_def;
    }

    public static int NextCombatPray(int A, int S, int D, int H, int R, int P, int M, int Sum) {
      int initial_pray = P;
      int initial_combat = CalculateCombat(A, S, D, H, R, P, M, Sum);
      while (CalculateCombat(A, S, D, H, R, ++P, M, Sum) <= initial_combat)
        ;
      return P - initial_pray;
    }

    public static int NextCombatSum(int A, int S, int D, int H, int R, int P, int M, int Sum) {
      int initial_sum = Sum;
      int initial_combat = CalculateCombat(A, S, D, H, R, P, M, Sum);
      while (CalculateCombat(A, S, D, H, R, P, M, ++Sum) <= initial_combat)
        ;
      return Sum - initial_sum;
    }

    public static int Lvl2Exp(int level) {
      int exp = 0;
      while (level > 1)
        exp += --level + (int)(300.0 * Math.Pow(2, level / 7.0));
      return exp / 4;
    }

    public static int Exp2Lvl(int exp) {
      int level = 0;
      int levelExp = 0;
      while (levelExp / 4 <= exp)
        levelExp += ++level + (int)(300.0 * Math.Pow(2, level / 7.0));
      return level;
    }

  } //class RSUtil
} //namespace BigSister