using System;

namespace BigSister {
  static class Util {

    private static int CalculateCombat(int neutralBonus, int meleeBonus, int magicBonus, int rangeBonus) {
      return (int)Math.Floor((double)(neutralBonus * 100 + Math.Max(meleeBonus, Math.Max(magicBonus, rangeBonus)) * 130) / 400.0);
    }

    public static int CalculateCombat(int att, int str, int def, int hp, int ran, int pr, int mag, int sum) {
      return CalculateCombat(def + hp + pr / 2 + sum / 2, att + str, mag + mag / 2, ran + ran / 2);
    }

    public static int CalculateCombat(int att, int str, int def, int hp, int ran, int pr, int mag) {
      return CalculateCombat(def + hp + pr / 2, att + str, mag + mag / 2, ran + ran / 2);
    }

    public static int CalculateCombat(Skills skills, bool @virtual, bool f2p) {
      if (@virtual) {
        if (f2p) {
          return CalculateCombat(skills[Skill.ATTA].VLevel, skills[Skill.STRE].VLevel, skills[Skill.DEFE].VLevel, skills[Skill.HITP].VLevel, skills[Skill.RANG].VLevel, skills[Skill.PRAY].VLevel, skills[Skill.MAGI].VLevel);
        } else {
          return CalculateCombat(skills[Skill.ATTA].VLevel, skills[Skill.STRE].VLevel, skills[Skill.DEFE].VLevel, skills[Skill.HITP].VLevel, skills[Skill.RANG].VLevel, skills[Skill.PRAY].VLevel, skills[Skill.MAGI].VLevel, skills[Skill.SUMM].VLevel);
        }
      } else {
        if (f2p) {
          return CalculateCombat(skills[Skill.ATTA].Level, skills[Skill.STRE].Level, skills[Skill.DEFE].Level, skills[Skill.HITP].Level, skills[Skill.RANG].Level, skills[Skill.PRAY].Level, skills[Skill.MAGI].Level);
        } else {
          return CalculateCombat(skills[Skill.ATTA].Level, skills[Skill.STRE].Level, skills[Skill.DEFE].Level, skills[Skill.HITP].Level, skills[Skill.RANG].Level, skills[Skill.PRAY].Level, skills[Skill.MAGI].Level, skills[Skill.SUMM].Level);
        }
      }
    }

    public static string CombatClass(int att, int str, int ran, int mag) {
      int meleeBonus = att + str;
      int magicBonus = mag + mag / 2;
      int rangeBonus = ran + ran / 2;

      if (meleeBonus > magicBonus && meleeBonus > rangeBonus)
        return "Warrior";
      else if (magicBonus > meleeBonus && magicBonus > rangeBonus)
        return "Mage";
      else if (rangeBonus > meleeBonus && rangeBonus > magicBonus)
        return "Ranger";
      else
        return "Hybrid";
    }

    public static string CombatClass(Skills skills, bool @virtual) {
      if (@virtual) {
        return CombatClass(skills[Skill.ATTA].VLevel, skills[Skill.STRE].VLevel, skills[Skill.RANG].VLevel, skills[Skill.MAGI].VLevel);
      } else {
        return CombatClass(skills[Skill.ATTA].Level, skills[Skill.STRE].Level, skills[Skill.RANG].Level, skills[Skill.MAGI].Level);
      }
    }

    public static int NextCombatAttStr(int att, int str, int def, int hp, int ran, int pr, int mag, int sum) {
      int initialAtt = att;
      int initialCombat = CalculateCombat(att, str, def, hp, ran, pr, mag, sum);
      while (CalculateCombat(++att, str, def, hp, ran, pr, mag, sum) <= initialCombat)
        ;
      return att - initialAtt;
    }

    public static int NextCombatDefHp(int att, int str, int def, int hp, int ran, int pr, int mag, int sum) {
      int initialDef = def;
      int initialCombat = CalculateCombat(att, str, def, hp, ran, pr, mag, sum);
      while (CalculateCombat(att, str, ++def, hp, ran, pr, mag, sum) <= initialCombat)
        ;
      return def - initialDef;
    }

    public static int NextCombatMag(int att, int str, int def, int hp, int ran, int pr, int mag, int sum) {
      int initialMag = mag;
      int initialCombat = CalculateCombat(att, str, def, hp, ran, pr, mag, sum);
      while (CalculateCombat(att, str, def, hp, ran, pr, ++mag, sum) <= initialCombat)
        ;
      return mag - initialMag;
    }

    public static int NextCombatRan(int att, int str, int def, int hp, int ran, int pr, int mag, int sum) {
      int initialRan = ran;
      int initialCombat = CalculateCombat(att, str, def, hp, ran, pr, mag, sum);
      while (CalculateCombat(att, str, def, hp, ++ran, pr, mag, sum) <= initialCombat)
        ;
      return ran - initialRan;
    }

    public static int NextCombatPray(int att, int str, int def, int hp, int ran, int pr, int mag, int sum) {
      int initialPray = pr;
      int initialCombat = CalculateCombat(att, str, def, hp, ran, pr, mag, sum);
      while (CalculateCombat(att, str, def, hp, ran, ++pr, mag, sum) <= initialCombat)
        ;
      return pr - initialPray;
    }

    public static int NextCombatSum(int att, int str, int def, int hp, int ran, int pr, int mag, int sum) {
      int initialSum = sum;
      int initialCombat = CalculateCombat(att, str, def, hp, ran, pr, mag, sum);
      while (CalculateCombat(att, str, def, hp, ran, pr, mag, ++sum) <= initialCombat)
        ;
      return sum - initialSum;
    }

  } //class RSUtil
} //namespace BigSister