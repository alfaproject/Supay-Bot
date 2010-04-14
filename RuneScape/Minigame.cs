using System;
using System.Globalization;

namespace Supay.Bot {
  class Minigame : Hiscore, IComparable<Minigame> {

    public const string DUEL = "Duel Tournament";
    public const string BOUN = "Bounty Hunters";
    public const string ROGU = "Bounty Hunter Rogues";
    public const string FIST = "Fist of Guthix";
    public const string MOBI = "Mobilising Armies";
    public const string BAAT = "BA Attacker";
    public const string BADE = "BA Defender";
    public const string BACO = "BA Collector";
    public const string BAHE = "BA Healer";


    public Minigame(string name, int rank, int score)
      : base(rank) {
      Name = name;

      if (score < 0)
        Score = 0;
      else
        Score = score;
    }

    public Minigame(int rank)
      : base(rank) {
    }

    public int Score {
      get;
      set;
    }

    public static bool TryParse(string s, ref string result) {
      try {
        result = Parse(s);
        return true;
      }
      catch {
        return false;
      }
    }

    public static string Parse(string s) {
      if (s == null)
        throw new ArgumentNullException("s");

      switch (s.ToUpperInvariant()) {
        case "DT":
        case "DUEL":
        case "DUELING":
        case "DUELTOURNAMENT":
          return DUEL;
        case "BH":
        case "BOUNTY":
        case "BOUNTYHUNT":
        case "BOUNTYHUNTER":
        case "BOUNTYHUNTERS":
          return BOUN;
        case "BHR":
        case "ROGUE":
        case "ROGUES":
        case "BOUNTYROGUE":
        case "BOUNTYROGUES":
        case "HUNTERROGUE":
        case "HUNTERROGUES":
        case "BOUNTYHUNTERROGUE":
        case "BOUNTYHUNTERROGUES":
          return ROGU;
        case "FOG":
        case "FIST":
        case "FISTING":
        case "FISTOFGUTHIX":
          return FIST;
        case "MO":
        case "AR":
        case "MOB":
        case "MOBIL":
        case "MOBILISING":
        case "ARMY":
        case "ARMYS":
        case "ARMIES":
        case "MOA":
        case "MOBA":
        case "MOBILISINGARMIES":
          return MOBI;
        case "BAAT":
        case "BAATT":
        case "BAATTACK":
        case "BAATTACKER":
          return BAAT;
        case "BADE":
        case "BADEF":
        case "BADEFENDER":
          return BADE;
        case "BACO":
        case "BACOL":
        case "BACOLL":
        case "BACOLLECTOR":
          return BACO;
        case "BAHE":
        case "BAHEAL":
        case "BAHEALER":
          return BAHE;
        default:
          throw new ArgumentException("Input minigame alias is invalid.", "s");
      }
    }

    public static string IdToName(int id) {
      switch (id) {
        case 0: return DUEL;
        case 1: return BOUN;
        case 2: return ROGU;
        case 3: return FIST;
        case 4: return MOBI;
        case 5: return BAAT;
        case 6: return BADE;
        case 7: return BACO;
        case 8: return BAHE;
        default:
          return "Minigame" + id;
      }
    }

    public static int NameToId(string name) {
      switch (name) {
        case DUEL: return 0;
        case BOUN: return 1;
        case ROGU: return 2;
        case FIST: return 3;
        case MOBI: return 4;
        case BAAT: return 5;
        case BADE: return 6;
        case BACO: return 7;
        case BAHE: return 8;
        default:
          return -1;
      }
    }

    // newMinigame - oldMinigame
    public static Minigame operator -(Minigame newMinigame, Minigame oldMinigame) {
      if (oldMinigame.Rank == -1 && newMinigame.Rank > 0)
        return new Minigame(newMinigame.Name, 0, newMinigame.Score - oldMinigame.Score);
      else
        return new Minigame(newMinigame.Name, oldMinigame.Rank - newMinigame.Rank, newMinigame.Score - oldMinigame.Score);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
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
          return (this.Rank == -1 ? "Not ranked" : this.Rank.ToString("N0", provider));
        case "r":
          return (this.Rank == -1 ? "NR" : this.Rank.ToString("N0", provider));
        case "s":
          return this.Score.ToString("N0", provider);
        default:
          return Name;
      }
    }

    #region IComparable<Minigame> Members

    // {CompareTo < 0 => this < other} {CompareTo > 0 => this > other} {CompareTo == 0 => this == other}
    public int CompareTo(Minigame other) {
      if (Object.ReferenceEquals(this, other))
        return 0; // same object reference

      if (Score == other.Score)
        return 0;
      else if (Score > other.Score)
        return -1;
      else
        return 1;
    }

    #endregion

  } // class Minigame
} // //namespace Supay.Bot