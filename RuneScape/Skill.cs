using System;
using System.Globalization;
using System.Linq;

namespace Supay.Bot {
  class Skill : Hiscore, IComparable<Skill> {

    public const string OVER = "Overall";
    public const string ATTA = "Attack";
    public const string DEFE = "Defence";
    public const string STRE = "Strength";
    public const string HITP = "Constitution";
    public const string RANG = "Ranged";
    public const string PRAY = "Prayer";
    public const string MAGI = "Magic";
    public const string COOK = "Cooking";
    public const string WOOD = "Woodcutting";
    public const string FLET = "Fletching";
    public const string FISH = "Fishing";
    public const string FIRE = "Firemaking";
    public const string CRAF = "Crafting";
    public const string SMIT = "Smithing";
    public const string MINI = "Mining";
    public const string HERB = "Herblore";
    public const string AGIL = "Agility";
    public const string THIE = "Thieving";
    public const string SLAY = "Slayer";
    public const string FARM = "Farming";
    public const string RUNE = "Runecraft";
    public const string HUNT = "Hunter";
    public const string CONS = "Construction";
    public const string SUMM = "Summoning";
    public const string DUNG = "Dungeoneering";
    public const string COMB = "Combat";

    private static readonly string[][] _aliases = {
      new[] { OVER, "OA", "OVE", "OVER", "OV", "TOT", "TOTAL" },
      new[] { ATTA, "AT", "ATT", "ATTA" },
      new[] { DEFE, "DE", "DEF", "DEFE", "DEFENSE" },
      new[] { STRE, "ST", "STR", "STRE" },
      new[] { HITP, "CT", "HIT", "HITP", "CONSTITUT", "CONSTITUTE", "HP", "HITS", "HITPOINT", "HITPOINTS", "LP", "LIFE", "LIFEPOINT", "LIFEPOINTS" },
      new[] { RANG, "RA", "RAN", "RANG", "RANGE", "RANGING" },
      new[] { PRAY, "PR", "PRA", "PRAY" },
      new[] { MAGI, "MA", "MAG", "MAGE", "MAGI" },
      new[] { COOK, "CK", "COO", "COOK" },
      new[] { WOOD, "WC", "WOO", "WOOD", "WOODCUT" },
      new[] { FLET, "FL", "FLE", "FLET", "FLETCH" },
      new[] { FISH, "FI", "FIS", "FISH" },
      new[] { FIRE, "FM", "FIR", "FIRE", "FIREMAKE" },
      new[] { CRAF, "CR", "CRA", "CRAF", "CRAFT" },
      new[] { SMIT, "SM", "SMI", "SMIT", "SMITH" },
      new[] { MINI, "MI", "MIN", "MINE" },
      new[] { HERB, "HE", "HER", "HERB", "HERBLAW" },
      new[] { AGIL, "AG", "AGI", "AGIL" },
      new[] { THIE, "TH", "THI", "THIE", "THIEF", "THIEVE" },
      new[] { SLAY, "SL", "SLA", "SLAY" },
      new[] { FARM, "FA", "FAR", "FARM" },
      new[] { RUNE, "RC", "RUN", "RUNE", "RUNECRAFTING" },
      new[] { HUNT, "HU", "HUN", "HUNT", "HUNTING" },
      new[] { CONS, "CO", "CON", "CONS", "CONST", "CONSTRUCT" },
      new[] { SUMM, "SU", "SUM", "SUMM", "SUMMON" },
      new[] { DUNG, "DU", "DUN", "DUNG", "DUNGEON", "DUNGEONERING" },
      new[] { COMB, "CB", "CMB", "COMB" }
    };

    public Skill(string name, int rank, int level, int exp)
      : base(name, rank) {
      _level = level;
      _exp = exp;

      if (name == OVER || name == COMB)
        VLevel = level;
      else
        VLevel = _exp.ToLevel();
    }

    public Skill(string name, int rank, int exp)
      : base(name, rank) {
      Exp = exp;
    }

    public Skill(string name, int rank)
      : base(name, rank) {
    }

    /// <summary>
    /// Parse a hiscore line from 'light' Hiscores.
    /// http://www.runescape.com/kbase/viewarticle.ws?article_id=201
    /// </summary>
    /// <param name="id">Skill ID</param>
    /// <param name="hiscoreLine">Tokens of the hiscore line</param>
    public Skill(int id, string[] hiscoreLine) {
      Name = IdToName(id);
      Rank = int.Parse(hiscoreLine[0], CultureInfo.InvariantCulture);
      if (Rank == -1) {
        _level = -1;
        _exp = -1;
      } else {
        _level = int.Parse(hiscoreLine[1], CultureInfo.InvariantCulture);
        _exp = int.Parse(hiscoreLine[2], CultureInfo.InvariantCulture);
      }
    }

    private int _exp;
    public int Exp {
      get {
        return _exp;
      }
      set {
        if (_exp != value) {
          _exp = value;
          if (Name != OVER && Name != COMB) {
            VLevel = _exp.ToLevel();
            _level = Math.Min(99, VLevel);
          }
        }
      }
    }

    private int _level;
    public int Level {
      get {
        return _level;
      }
      set {
        _level = value;
        VLevel = value;
      }
    }

    public int VLevel {
      get;
      private set;
    }

    public int ExpToLevel {
      get {
        if (_level < 99)
          return (_level + 1).ToExp() - _exp;
        return 0;
      }
    }

    public int ExpToVLevel {
      get {
        if (VLevel < 126)
          return (this.VLevel + 1).ToExp() - _exp;
        return 200000000 - _exp;
      }
    }

    public string ShortName {
      get {
        switch (Name) {
          case RANG:
            return "Range";
          case COOK:
            return "Cook";
          case WOOD:
            return "Woodcut";
          case FLET:
            return "Fletch";
          case FISH:
            return "Fish";
          case FIRE:
            return "Firemake";
          case CRAF:
            return "Craft";
          case SMIT:
            return "Smith";
          case MINI:
            return "Mine";
          case THIE:
            return "Thief";
          case FARM:
            return "Farm";
          case CONS:
            return "Construct";
          case SUMM:
            return "Summon";
          case DUNG:
            return "Dungeon";
          default:
            return Name;
        }
      }
    }

    public static bool TryParse(string s, ref string result) {
      if (s == null) {
        return false;
      }

      foreach (string[] aliases in _aliases.Where(aliases => aliases.Any(alias => alias.EqualsI(s)))) {
        result = aliases[0];
        return true;
      }

      return false;
    }

    public static string Parse(string s) {
      if (s == null) {
        throw new ArgumentNullException("s");
      }

      foreach (string[] aliases in _aliases.Where(aliases => aliases.Any(alias => alias.EqualsI(s)))) {
        return aliases[0];
      }

      throw new ArgumentException(@"Input skill alias is invalid.", "s");
    }

    public static string IdToName(int id) {
      if (id < _aliases.Length - 1) {
        return _aliases[id][0];
      }
      return "Skill" + id;
    }

    public static int NameToId(string name) {
      for (int i = 0; i < _aliases.Length; i++) {
        if (name.EqualsI(_aliases[i][0])) {
          return i;
        }
      }
      return -1;
    }

    #region Operators

    public static Skill operator -(Skill left, Skill right) {
      if (right.Rank == -1) {
        return new Skill(left.Name, 0, left.Level - right.Level, left.Exp - right.Exp);
      }
      return new Skill(left.Name, right.Rank - left.Rank, left.Level - right.Level, left.Exp - right.Exp);
    }

    #endregion

    #region IFormattable

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
    public override string ToString(string format, IFormatProvider provider) {
      if (string.IsNullOrEmpty(format)) {
        format = "G";
      }

      if (provider != null) {
        ICustomFormatter formatter = provider.GetFormat(this.GetType()) as ICustomFormatter;
        if (formatter != null) {
          return formatter.Format(format, this, provider);
        }
      }

      switch (format) {
        case "G":
          return string.Format(provider, "{{ Skill, Name = {0}, Rank = {1}, Level = {2}, Exp = {3} }}", Name, Rank, Level, Exp);
        case "N":
          return Name;
        case "n":
          return Name.ToLowerInvariant();
        case "R":
          return (Rank == -1 || Rank == int.MaxValue ? "Not ranked" : Rank.ToString("N0", provider));
        case "r":
          return (Rank == -1 || Rank == int.MaxValue ? "NR" : Rank.ToString("N0", provider));
        case "e":
          return Exp.ToString("N0", provider);
        case "l":
          return Level.ToString("N0", provider);
        case "v":
          return VLevel.ToString("N0", provider);
        case "re":
          return (Rank == -1 ? "~" : string.Empty) + Exp.ToString("N0", provider);
        case "rl":
          return (Rank == -1 ? "~" : string.Empty) + Level.ToString("N0", provider);
        case "rv":
          return (Rank == -1 ? "~" : string.Empty) + VLevel.ToString("N0", provider);
        default:
          throw new FormatException(string.Format(provider, "The {0} format string is not supported.", format));
      }
    }

    #endregion

    #region IComparable<Skill>

    public int CompareTo(Skill other) {
      if (ReferenceEquals(this, other)) {
        return 0;
      }

      // compare by experience if levels are the same or levels otherwise
      return (Level == other.Level ? other._exp.CompareTo(_exp) : other.Level.CompareTo(Level));
    }

    #endregion

  } //class Skill
} //namespace Supay.Bot