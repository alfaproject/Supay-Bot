using System;
using System.Linq;

namespace Supay.Bot
{
    internal static class Utils
    {
        public static int CalculateCombat(int attack, int strength, int defence, int ranged, int magic)
        {
            return 2 + defence + new[] {attack, strength, ranged, magic}.Max();
        }

        public static int CalculateCombat(SkillDictionary skills, bool @virtual)
        {
            return @virtual
                ? CalculateCombat(skills[Skill.ATTA].VLevel, skills[Skill.STRE].VLevel, skills[Skill.DEFE].VLevel, skills[Skill.RANG].VLevel, skills[Skill.MAGI].VLevel)
                : CalculateCombat(skills[Skill.ATTA].Level, skills[Skill.STRE].Level, skills[Skill.DEFE].Level, skills[Skill.RANG].Level, skills[Skill.MAGI].Level);
        }

        public static string CombatClass(int attack, int strength, int ranged, int magic)
        {
            var melee = Math.Max(attack, strength);

            if (melee > magic && melee > ranged)
            {
                return "Warrior";
            }
            if (magic > melee && magic > ranged)
            {
                return "Mage";
            }
            if (ranged > melee && ranged > magic)
            {
                return "Ranger";
            }
            return "Hybrid";
        }

        public static string CombatClass(SkillDictionary skills, bool @virtual)
        {
            return @virtual
                ? CombatClass(skills[Skill.ATTA].VLevel, skills[Skill.STRE].VLevel, skills[Skill.RANG].VLevel, skills[Skill.MAGI].VLevel)
                : CombatClass(skills[Skill.ATTA].Level, skills[Skill.STRE].Level, skills[Skill.RANG].Level, skills[Skill.MAGI].Level);
        }

        public static int NextCombatAttack(int att, int str, int def, int ran, int mag)
        {
            int initialAtt = att;
            int initialCombat = CalculateCombat(att, str, def, ran, mag);
            while (CalculateCombat(++att, str, def, ran, mag) <= initialCombat)
            {
            }
            return att - initialAtt;
        }

        public static int NextCombatStrength(int att, int str, int def, int ran, int mag)
        {
            int initialStr = str;
            int initialCombat = CalculateCombat(att, str, def, ran, mag);
            while (CalculateCombat(att, ++str, def, ran, mag) <= initialCombat)
            {
            }
            return str - initialStr;
        }

        public static int NextCombatDefence(int att, int str, int def, int ran, int mag)
        {
            int initialDef = def;
            int initialCombat = CalculateCombat(att, str, def, ran, mag);
            while (CalculateCombat(att, str, ++def, ran, mag) <= initialCombat)
            {
            }
            return def - initialDef;
        }

        public static int NextCombatMagic(int att, int str, int def, int ran, int mag)
        {
            int initialMag = mag;
            int initialCombat = CalculateCombat(att, str, def, ran, mag);
            while (CalculateCombat(att, str, def, ran, ++mag) <= initialCombat)
            {
            }
            return mag - initialMag;
        }

        public static int NextCombatRanged(int att, int str, int def, int ran, int mag)
        {
            int initialRan = ran;
            int initialCombat = CalculateCombat(att, str, def, ran, mag);
            while (CalculateCombat(att, str, def, ++ran, mag) <= initialCombat)
            {
            }
            return ran - initialRan;
        }

        public static int SoulWarsExpPerZeal(string skill, int level)
        {
            if (level > 99)
            {
                level = 99;
            }
            switch (skill)
            {
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
                    if (level < 30)
                    {
                        return (int) Math.Round(6.7 * Math.Pow(1.1052, level));
                    }
                    return (int) Math.Round(0.002848 * (level * level) + 0.14 * Math.Log(level)) * 45;
                default:
                    throw new ArgumentOutOfRangeException("skill");
            }
        }

        public static int SoulWarsZealToExp(string skill, long startExp, long targetExp, bool bonus)
        {
            int zeal = 0;
            while (startExp < targetExp)
            {
                int expPerZeal = SoulWarsExpPerZeal(skill, startExp.ToLevel());
                if (bonus)
                {
                    startExp += (int) ((100 * expPerZeal) * 1.1);
                    zeal += 100;
                }
                else
                {
                    startExp += expPerZeal;
                    zeal++;
                }
            }
            return zeal;
        }

        public static int PestControlExpPerPoint(string skill, int level)
        {
            if (level > 99)
            {
                level = 99;
            }
            int modifier;
            switch (skill)
            {
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
            return (int) Math.Ceiling(((level + 25) * (level - 24)) / 606.0) * modifier;
        }

        public static int PestControlPointsToExp(string skill, long startExp, long targetExp, int bonus)
        {
            int points = 0;
            while (startExp < targetExp)
            {
                int expPerPoint = PestControlExpPerPoint(skill, startExp.ToLevel());
                switch (bonus)
                {
                    case 10:
                        startExp += (int) ((10 * expPerPoint) * 1.01);
                        points += 10;
                        break;
                    case 100:
                        startExp += (int) ((100 * expPerPoint) * 1.1);
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

        public static int LampsToExp(long startExp, long targetExp)
        {
            int lamps = 0;
            while (startExp < targetExp)
            {
                startExp += 10 * Math.Min(startExp.ToLevel(), 99);
                lamps++;
            }
            return lamps;
        }

        public static int BooksToExp(long startExp, long targetExp)
        {
            int books = 0;
            while (startExp < targetExp)
            {
                startExp += 15 * Math.Min(startExp.ToLevel(), 99);
                books++;
            }
            return books;
        }

        public static int EffigyToExp(Skill skill, long targetExp)
        {
            long startExp = skill.Exp;
            int effigies = 0;
            while (startExp < targetExp)
            {
                int skillLvl = Math.Min(startExp.ToLevel(), skill.MaxLevel);
                startExp += (long) ((Math.Pow(skillLvl, 3.0) - 2 * Math.Pow(skillLvl, 2.0) + 100 * skillLvl) / 20);
                effigies++;
            }
            return effigies;
        }

        public static int Reqs(int combat)
        {
            if (combat >= 131)
            {
                return 2200;
            }
            if (combat >= 121)
            {
                return 2150;
            }
            if (combat >= 101)
            {
                return 2100;
            }
            return 2050;
        }
    }
}
