using System;

namespace Supay.Bot {
  static partial class Utils {

    private static int CalculateCombat(int neutralBonus, int meleeBonus, int magicBonus, int rangeBonus) {
      return (int)Math.Floor((double)(neutralBonus * 100 + Math.Max(meleeBonus, Math.Max(magicBonus, rangeBonus)) * 130) / 400.0);
    }

    public static int CalculateCombat(int att, int str, int def, int hp, int ran, int pr, int mag, int sum) {
      return CalculateCombat(def + hp + pr / 2 + sum / 2, att + str, mag + mag / 2, ran + ran / 2);
    }

    public static int CalculateCombat(int att, int str, int def, int hp, int ran, int pr, int mag) {
      return CalculateCombat(def + hp + pr / 2, att + str, mag + mag / 2, ran + ran / 2);
    }

    public static int CalculateCombat(SkillDictionary skills, bool @virtual, bool f2p) {
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

    public static string CombatClass(SkillDictionary skills, bool @virtual) {
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

    public static int SoulWarsExpPerZeal(string skill, int level) {
      if (level > 99) {
        level = 99;
      }
      switch (skill) {
        case Skill.ATTA:
        case Skill.STRE:
        case Skill.DEFE:
        case Skill.HITP:
          return level * level / 600 * 525;
        case Skill.RANG:
        case Skill.MAGI:
          return level * level / 600 * 480;
        case Skill.PRAY:
          return level * level / 600 * 270;
        case Skill.SLAY:
          if (level < 30) {
            return (int)Math.Round(6.7 * Math.Pow(1.1052, (double)level));
          } else {
            return (int)Math.Round(0.002848 * (double)(level * level) + 0.14 * Math.Log((double)level)) * 45;
          }
        default:
          throw new ArgumentOutOfRangeException("skill");
      }
    }

    public static int SoulWarsZealToExp(string skill, long startExp, long targetExp, bool bonus) {
      int zeal = 0;
      while (startExp < targetExp) {
        int expPerZeal = Utils.SoulWarsExpPerZeal(skill, startExp.ToLevel());
        if (bonus) {
          startExp += (int)((double)(100 * expPerZeal) * 1.1);
          zeal += 100;
        } else {
          startExp += expPerZeal;
          zeal++;
        }
      }
      return zeal;
    }

    public static int PestControlExpPerPoint(string skill, int level) {
      if (level > 99) {
        level = 99;
      }
      int modifier;
      switch (skill) {
        case Skill.ATTA:
        case Skill.STRE:
        case Skill.DEFE:
        case Skill.HITP:
          modifier = 35;
          break;
        case Skill.RANG:
        case Skill.MAGI:
          modifier = 32;
          break;
        case Skill.PRAY:
          modifier = 18;
          break;
        default:
          throw new ArgumentOutOfRangeException("skill");
      }
      return (int)Math.Ceiling((double)((level + 25) * (level - 24)) / 606.0) * modifier;
    }

    public static int PestControlPointsToExp(string skill, long startExp, long targetExp, int bonus) {
      int points = 0;
      while (startExp < targetExp) {
        int expPerPoint = Utils.PestControlExpPerPoint(skill, startExp.ToLevel());
        switch (bonus) {
          case 10:
            startExp += (int)((double)(10 * expPerPoint) * 1.01);
            points += 10;
            break;
          case 100:
            startExp += (int)((double)(100 * expPerPoint) * 1.1);
            points += 100;
            break;
          default:
            startExp += expPerPoint;
            points++;
            break;
        }
      }
      return points;
    }

    public static int LampsToExp(long startExp, long targetExp) {
      int lamps = 0;
      while (startExp < targetExp) {
        startExp += 10 * Math.Min(startExp.ToLevel(), 99);
        lamps++;
      }
      return lamps;
    }

    public static int BooksToExp(long startExp, long targetExp) {
      int books = 0;
      while (startExp < targetExp) {
        startExp += 15 * Math.Min(startExp.ToLevel(), 99);
        books++;
      }
      return books;
    }

    public static int EffigyToExp(Skill skill, long targetExp) {
      long startExp = skill.Exp;
      int effigies = 0;
      while (startExp < targetExp) {
        startExp += (long)(Math.Pow((double)Math.Min(startExp.ToLevel(), skill.MaxLevel), 3.0) / 20.2);
        effigies++;
      }
      return effigies;
    }

  } //class Utils
} //namespace Supay.Bot