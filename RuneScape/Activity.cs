using System;
using System.Linq;

namespace Supay.Bot
{
  internal class Activity : Hiscore, IEquatable<Activity>, IComparable<Activity>
  {
    public const string DUEL = "Duel Tournament";
    public const string BOUN = "Bounty Hunters";
    public const string ROGU = "Bounty Hunter Rogues";
    public const string FIST = "Fist of Guthix";
    public const string MOBI = "Mobilising Armies";
    public const string BAAT = "BA Attacker";
    public const string BADE = "BA Defender";
    public const string BACO = "BA Collector";
    public const string BAHE = "BA Healer";
    public const string CWAR = "Castle Wars";
    public const string CONQ = "Conquest";
    public const string DOMI = "Dominion Tower";

    private static readonly string[][] _aliases = new[] {
      new[] { DUEL, "DT", "DUEL", "DUELING", "DUELTOURNAMENT" },
      new[] { BOUN, "BH", "BOUNTY", "BOUNTYHUNT", "BOUNTYHUNTER", "BOUNTYHUNTERS" },
      new[] { ROGU, "BR", "BHR", "ROGUE", "ROGUES", "BOUNTYROGUE", "BOUNTYROGUES", "HUNTERROGUE", "HUNTERROGUES", "BOUNTYHUNTERROGUE", "BOUNTYHUNTERROGUES" },
      new[] { FIST, "FG", "FOG", "FIST", "FISTING", "FISTOFGUTHIX" },
      new[] { MOBI, "MO", "AR", "MOB", "MOBIL", "MOBILISING", "ARMY", "ARMYS", "ARMIES", "MOA", "MOBA", "MOBILISINGARMY", "MOBILISINGARMIES" },
      new[] { BAAT, "BAAT", "BAATT", "BAATTACK", "BAATTACKER" },
      new[] { BADE, "BADE", "BADEF", "BADEFEND", "BADEFENDER" },
      new[] { BACO, "BACO", "BACOL", "BACOLL", "BACOLLECT", "BACOLLECTOR" },
      new[] { BAHE, "BAHE", "BAHEAL", "BAHEALER" },
      new[] { CWAR, "CW", "CWAR", "CWARS", "CASTLE", "CASTLEWARS" },
      new[] { CONQ, "CQ", "CONQ", "CONQUEST" },
      new[] { DOMI, "DO", "DF", "DOT", "DOMINION", "DOMINIONTOWER", "TOWER" },
    };

    public Activity(string name, int rank, int score)
      : base(name, rank)
    {
      this.Score = score < 0 ? 0 : score;
    }

    public int Score
    {
      get;
      set;
    }

    public static bool TryParse(string s, ref string result)
    {
      if (s == null)
      {
        return false;
      }

      foreach (var aliases in _aliases.Where(aliases => aliases.Any(alias => alias.EqualsI(s))))
      {
        result = aliases[0];
        return true;
      }

      return false;
    }

    public static string Parse(string s)
    {
      if (s == null)
      {
        throw new ArgumentNullException("s");
      }

      foreach (var aliases in _aliases.Where(aliases => aliases.Any(alias => alias.EqualsI(s))))
      {
        return aliases[0];
      }

      throw new ArgumentException(@"Input activity alias is invalid.", "s");
    }

    public static string IdToName(int id)
    {
      if (id < _aliases.Length)
      {
        return _aliases[id][0];
      }
      return "Activity" + id;
    }

    public static int NameToId(string name)
    {
      for (int i = 0; i < _aliases.Length; i++)
      {
        if (name.EqualsI(_aliases[i][0]))
        {
          return i;
        }
      }
      return -1;
    }

    #region Operators

    public static Activity operator -(Activity left, Activity right)
    {
      if (right.Rank == -1 && left.Rank > 0)
      {
        return new Activity(left.Name, 0, left.Score - right.Score);
      }
      return new Activity(left.Name, right.Rank - left.Rank, left.Score - right.Score);
    }

    #endregion

    #region IFormattable

    public override string ToString(string format, IFormatProvider provider)
    {
      if (string.IsNullOrEmpty(format))
      {
        format = "G";
      }

      if (provider != null)
      {
        var formatter = provider.GetFormat(this.GetType()) as ICustomFormatter;
        if (formatter != null)
        {
          return formatter.Format(format, this, provider);
        }
      }

      switch (format)
      {
        case "G":
          return string.Format(provider, "{{ Activity, Name = {0}, Rank = {1}, Score = {2} }}", this.Name, this.Rank, this.Score);
        case "N":
          return this.Name;
        case "n":
          return this.Name.ToLowerInvariant();
        case "R":
          return this.Rank == -1 ? "Not ranked" : this.Rank.ToString("N0", provider);
        case "r":
          return this.Rank == -1 ? "NR" : this.Rank.ToString("N0", provider);
        case "s":
          return this.Score.ToString("N0", provider);
        default:
          throw new FormatException(string.Format(provider, "The {0} format string is not supported.", format));
      }
    }

    #endregion

    #region IEquatable<Activity>

    public bool Equals(Activity other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      return this.Name.EqualsI(other.Name) && this.Rank.Equals(other.Rank) && this.Score.Equals(other.Score);
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as Activity);
    }

    public override int GetHashCode()
    {
      // TODO provide a value based implementation
      return base.GetHashCode();
    }

    public static bool operator ==(Activity left, Activity right)
    {
      return ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right);
    }

    public static bool operator !=(Activity left, Activity right)
    {
      return !(left == right);
    }

    #endregion

    #region IComparable<Activity>

    // {CompareTo < 0 => this < other} {CompareTo > 0 => this > other} {CompareTo == 0 => this == other}
    public int CompareTo(Activity other)
    {
      if (ReferenceEquals(null, other))
      {
        return 1;
      }
      if (ReferenceEquals(this, other))
      {
        return 0;
      }

      return other.Score - this.Score;
    }

    #endregion
  }
}
