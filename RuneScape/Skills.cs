using System.Collections.Generic;

namespace BigSister {

  public class Skills : OrderedDictionary<string, Skill> {

    public Skills()
      : base(26) {
    }

    public int F2pExp() {
      return this[Skill.ATTA].Exp + this[Skill.DEFE].Exp + this[Skill.STRE].Exp + this[Skill.HITP].Exp + this[Skill.RANG].Exp + this[Skill.PRAY].Exp + this[Skill.MAGI].Exp + this[Skill.COOK].Exp + this[Skill.WOOD].Exp + this[Skill.FISH].Exp + this[Skill.FIRE].Exp + this[Skill.CRAF].Exp + this[Skill.SMIT].Exp + this[Skill.MINI].Exp + this[Skill.RUNE].Exp;
    }

    public List<Skill> Lowest {
      get {
        List<Skill> lowest = new List<Skill>(this.Values);
        lowest.RemoveAt(lowest.Count - 1);
        lowest.Sort((s1, s2) => -s1.CompareTo(s2));
        return lowest;
      }
    }

    public List<Skill> Highest {
      get {
        List<Skill> highest = new List<Skill>(this.Values);
        highest.RemoveAt(highest.Count - 1);
        highest.RemoveAt(0);
        highest.Sort();
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

    private Skill _closestToLevel;
    public Skill ClosestToLevel {
      get {
        if (_closestToLevel == null)
          foreach (Skill skill in this.Values)
            if (skill.Name != "Overall" && skill.Name != "Combat" && skill.ExpToVLevel > 0)
              if (_closestToLevel == null || _closestToLevel.ExpToVLevel > skill.ExpToVLevel)
                _closestToLevel = skill;
        return _closestToLevel;
      }
    }

    public int NextCombatAttStr() {
      return RSUtil.NextCombatAttStr(this[Skill.ATTA].Level, this[Skill.STRE].Level, this[Skill.DEFE].Level, this[Skill.HITP].Level, this[Skill.RANG].Level, this[Skill.PRAY].Level, this[Skill.MAGI].Level, this[Skill.SUMM].Level);
    }

    public int NextCombatMag() {
      return RSUtil.NextCombatMag(this[Skill.ATTA].Level, this[Skill.STRE].Level, this[Skill.DEFE].Level, this[Skill.HITP].Level, this[Skill.RANG].Level, this[Skill.PRAY].Level, this[Skill.MAGI].Level, this[Skill.SUMM].Level);
    }

    public int NextCombatRan() {
      return RSUtil.NextCombatRan(this[Skill.ATTA].Level, this[Skill.STRE].Level, this[Skill.DEFE].Level, this[Skill.HITP].Level, this[Skill.RANG].Level, this[Skill.PRAY].Level, this[Skill.MAGI].Level, this[Skill.SUMM].Level);
    }

    public int NextCombatDefHp() {
      return RSUtil.NextCombatDefHp(this[Skill.ATTA].Level, this[Skill.STRE].Level, this[Skill.DEFE].Level, this[Skill.HITP].Level, this[Skill.RANG].Level, this[Skill.PRAY].Level, this[Skill.MAGI].Level, this[Skill.SUMM].Level);
    }

    public int NextCombatPray() {
      return RSUtil.NextCombatPray(this[Skill.ATTA].Level, this[Skill.STRE].Level, this[Skill.DEFE].Level, this[Skill.HITP].Level, this[Skill.RANG].Level, this[Skill.PRAY].Level, this[Skill.MAGI].Level, this[Skill.SUMM].Level);
    }

    public int NextCombatSum() {
      return RSUtil.NextCombatSum(this[Skill.ATTA].Level, this[Skill.STRE].Level, this[Skill.DEFE].Level, this[Skill.HITP].Level, this[Skill.RANG].Level, this[Skill.PRAY].Level, this[Skill.MAGI].Level, this[Skill.SUMM].Level);
    }

    public List<Skill> SortedByExpToNextVLevel() {
      List<Skill> result = new List<Skill>(this.Count);
      int i;
      foreach (Skill skill in this.Values) {
        if (skill.Name == Skill.OVER || skill.Name == Skill.COMB || skill.ExpToVLevel == 0)
          continue;

        if (result.Count == 0 || result[result.Count - 1].ExpToVLevel < skill.ExpToVLevel) {
          result.Add(skill);
          continue;
        }

        for (i = 0; i < result.Count; i++) {
          if (result[i].ExpToVLevel > skill.ExpToVLevel) {
            result.Insert(i, skill);
            break;
          }
        }
      }
      return result;
    }

  } //class Skills
} //namespace BigSister