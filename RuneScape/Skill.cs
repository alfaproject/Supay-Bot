using System;
using System.Globalization;

namespace BigSister {
  class Skill : Hiscore, IComparable<Skill> {

    public const string OVER = "Overall";
    public const string ATTA = "Attack";
    public const string DEFE = "Defence";
    public const string STRE = "Strength";
    public const string HITP = "Hitpoints";
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
    public const string COMB = "Combat";

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
          default:
            return Name;
        }
      }
    }

    public static bool TryParse(string s, ref string result) {
      try {
        result = Parse(s);
        return true;
      } catch {
        return false;
      }
    }

    public static string Parse(string s) {
      if (s == null)
        throw new ArgumentNullException("s");

      switch (s.ToUpperInvariant()) {
        case "OA":
        case "OV":
        case "OVERALL":
          return OVER;
        case "ATT":
        case "ATTACK":
          return ATTA;
        case "DEF":
        case "DEFENCE":
          return DEFE;
        case "STR":
        case "STRENGTH":
          return STRE;
        case "HP":
        case "HIT":
        case "HITS":
        case "HITPOINTS":
          return HITP;
        case "RA":
        case "RAN":
        case "RANGE":
        case "RANGED":
        case "RANGING":
          return RANG;
        case "PR":
        case "PRAY":
        case "PRAYER":
          return PRAY;
        case "MA":
        case "MAG":
        case "MAGE":
        case "MAGIC":
          return MAGI;
        case "CK":
        case "COOK":
        case "COOKING":
          return COOK;
        case "WC":
        case "WOODCUT":
        case "WOODCUTTING":
          return WOOD;
        case "FL":
        case "FLETCH":
        case "FLETCHING":
          return FLET;
        case "FI":
        case "FISH":
        case "FISHING":
          return FISH;
        case "FM":
        case "FIRE":
        case "FIREMAKE":
        case "FIREMAKING":
          return FIRE;
        case "CR":
        case "CRAFT":
        case "CRAFTING":
          return CRAF;
        case "SM":
        case "SMITH":
        case "SMITHING":
          return SMIT;
        case "MI":
        case "MINE":
        case "MINING":
          return MINI;
        case "HE":
        case "HERB":
        case "HERBLORE":
          return HERB;
        case "AG":
        case "AGI":
        case "AGIL":
        case "AGILITY":
          return AGIL;
        case "TH":
        case "THIEF":
        case "THIEV":
        case "THIEVING":
          return THIE;
        case "SL":
        case "SLAY":
        case "SLAYER":
          return SLAY;
        case "FA":
        case "FARM":
        case "FARMING":
          return FARM;
        case "RC":
        case "RUNECRAFT":
        case "RUNECRAFTING":
          return RUNE;
        case "HU":
        case "HUNT":
        case "HUNTER":
        case "HUNTING":
          return HUNT;
        case "CO":
        case "CS":
        case "CON":
        case "CONS":
        case "CONST":
        case "CONSTRUCT":
        case "CONSTRUCTION":
          return CONS;
        case "SU":
        case "SUM":
        case "SUMMON":
        case "SUMMONING":
          return SUMM;
        case "CB":
        case "CMB":
        case "COMB":
        case "COMBAT":
          return COMB;
        default:
          throw new ArgumentException("Input skill alias '" + s + "' is invalid.", "s");
      }
    }

    public static int NameToId(string name) {
      switch (name) {
        case ATTA:
          return 1;
        case DEFE:
          return 2;
        case STRE:
          return 3;
        case HITP:
          return 4;
        case RANG:
          return 5;
        case PRAY:
          return 6;
        case MAGI:
          return 7;
        case COOK:
          return 8;
        case WOOD:
          return 9;
        case FLET:
          return 10;
        case FISH:
          return 11;
        case FIRE:
          return 12;
        case CRAF:
          return 13;
        case SMIT:
          return 14;
        case MINI:
          return 15;
        case HERB:
          return 16;
        case AGIL:
          return 17;
        case THIE:
          return 18;
        case SLAY:
          return 19;
        case FARM:
          return 20;
        case RUNE:
          return 21;
        case HUNT:
          return 22;
        case CONS:
          return 23;
        case SUMM:
          return 24;
        default:
          return 0;
      }
    }

    public static string IdToName(int id) {
      switch (id) {
        case 0:
          return OVER;
        case 1:
          return ATTA;
        case 2:
          return DEFE;
        case 3:
          return STRE;
        case 4:
          return HITP;
        case 5:
          return RANG;
        case 6:
          return PRAY;
        case 7:
          return MAGI;
        case 8:
          return COOK;
        case 9:
          return WOOD;
        case 10:
          return FLET;
        case 11:
          return FISH;
        case 12:
          return FIRE;
        case 13:
          return CRAF;
        case 14:
          return SMIT;
        case 15:
          return MINI;
        case 16:
          return HERB;
        case 17:
          return AGIL;
        case 18:
          return THIE;
        case 19:
          return SLAY;
        case 20:
          return FARM;
        case 21:
          return RUNE;
        case 22:
          return HUNT;
        case 23:
          return CONS;
        case 24:
          return SUMM;
        default:
          return "Skill" + id;
      }
    }

    // newSkill - oldSkill
    public static Skill operator -(Skill newSkill, Skill oldSkill) {
      if (oldSkill.Rank == -1)
        return new Skill(newSkill.Name, 0, newSkill.Level - oldSkill.Level, newSkill.Exp - oldSkill.Exp);
      else
        return new Skill(newSkill.Name, oldSkill.Rank - newSkill.Rank, newSkill.Level - oldSkill.Level, newSkill.Exp - oldSkill.Exp);
    }

    // IFormattable {ToString}
    public override string ToString(string format, IFormatProvider provider) {
      if (format == null)
        format = "N";

      if (provider != null) {
        ICustomFormatter formatter = provider.GetFormat(this.GetType()) as ICustomFormatter;
        if (formatter != null)
          return formatter.Format(format, this, provider);
      }

      switch (format) {
        case "N":
          return this.Name;
        case "n":
          return this.Name.ToLowerInvariant();
        case "R":
          return (this.Rank == -1 || this.Rank == int.MaxValue ? "Not ranked" : this.Rank.ToString("N0", CultureInfo.InvariantCulture));
        case "r":
          return (this.Rank == -1 || this.Rank == int.MaxValue ? "NR" : this.Rank.ToString("N0", CultureInfo.InvariantCulture));
        case "e":
          return _exp.ToString("N0", CultureInfo.InvariantCulture);
        case "l":
          return _level.ToString("N0", CultureInfo.InvariantCulture);
        case "v":
          return this.VLevel.ToString("N0", CultureInfo.InvariantCulture);
        case "re":
          return (this.Rank == -1 ? "~" : string.Empty) + _exp.ToString("N0", CultureInfo.InvariantCulture);
        case "rl":
          return (this.Rank == -1 ? "~" : string.Empty) + _level.ToString("N0", CultureInfo.InvariantCulture);
        case "rv":
          return (this.Rank == -1 ? "~" : string.Empty) + this.VLevel.ToString("N0", CultureInfo.InvariantCulture);
        default:
          return this.Name;
      }
    }

    #region IComparable<Skill>

    // {CompareTo < 0 => this < other} {CompareTo > 0 => this > other} {CompareTo == 0 => this == other}
    public int CompareTo(Skill other) {
      if (Object.ReferenceEquals(this, other))
        return 0; // same object reference

      if (this.VLevel == other.VLevel)
        if (_exp == other.Exp)
          return 0;
        else if (_exp > other.Exp)
          return -1;
        else
          return 1;
      else if (this.VLevel > other.VLevel)
        return -1;
      else
        return 1;
    }

    #endregion

  } // class Skill
} // namespace BigSister