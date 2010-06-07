using System.Collections.Generic;

namespace Supay.Bot {
  class SkillDictionary : OrderedDictionary<string, Skill> {

    public SkillDictionary()
      : base(26) {
    }

    public int F2pExp {
      get {
        return this[Skill.ATTA].Exp + this[Skill.DEFE].Exp + this[Skill.STRE].Exp + this[Skill.HITP].Exp + this[Skill.RANG].Exp + this[Skill.PRAY].Exp + this[Skill.MAGI].Exp + this[Skill.COOK].Exp + this[Skill.WOOD].Exp + this[Skill.FISH].Exp + this[Skill.FIRE].Exp + this[Skill.CRAF].Exp + this[Skill.SMIT].Exp + this[Skill.MINI].Exp + this[Skill.RUNE].Exp + this[Skill.DUNG].Exp;
      }
    }

    public List<Skill> Lowest {
      get {
        List<Skill> lowest = new List<Skill>(this.Values);
        lowest.RemoveAt(lowest.Count - 1);
        lowest.RemoveAt(0);
        lowest.Sort((s1, s2) => s1.VLevel.CompareTo(s2.VLevel));
        return lowest;
      }
    }

    public List<Skill> Highest {
      get {
        List<Skill> highest = new List<Skill>(this.Values);
        highest.RemoveAt(highest.Count - 1);
        highest.RemoveAt(0);
        highest.Sort((s1, s2) => -s1.VLevel.CompareTo(s2.VLevel));
        return highest;
      }
    }

    public List<Skill> LowestRanked {
      get {
        List<Skill> lowest = new List<Skill>(this.Values);
        lowest.RemoveAll(s => s.Name == Skill.OVER || s.Name == Skill.COMB);
        foreach (Skill s in lowest.FindAll(s => s.Rank == -1))
          s.Rank = int.MaxValue;
        lowest.Sort((s1, s2) => -s1.Rank.CompareTo(s2.Rank));
        return lowest;
      }
    }

    public List<Skill> HighestRanked {
      get {
        List<Skill> highest = new List<Skill>(this.Values);
        highest.RemoveAll(s => s.Name == Skill.OVER || s.Name == Skill.COMB);
        foreach (Skill s in highest.FindAll(s => s.Rank == -1))
          s.Rank = int.MaxValue;
        highest.Sort((s1, s2) => s1.Rank.CompareTo(s2.Rank));
        return highest;
      }
    }

    public List<Skill> SortedByExpToNextVLevel {
      get {
        List<Skill> result = new List<Skill>(this.Values);
        result.RemoveAll(s => s.ExpToVLevel == 0 || s.Name == Skill.OVER || s.Name == Skill.COMB);
        result.Sort((s1, s2) => s1.ExpToVLevel.CompareTo(s2.ExpToVLevel));
        return result;
      }
    }

  } //class SkillDictionary
} //namespace Supay.Bot