using System;
using System.Globalization;

namespace Supay.Bot {
  abstract class Hiscore : IFormattable {

    public Hiscore() {
    }

    public Hiscore(int rank) {
      Rank = rank;
    }

    public Hiscore(string name, int rank)
      : this(rank) {
      Name = name;
    }

    public string RSN {
      get;
      set;
    }

    public string Name {
      get;
      protected set;
    }

    public int Rank {
      get;
      set;
    }

    #region IFormattable

    public abstract string ToString(string format, IFormatProvider formatProvider);

    public string ToString(string format) {
      return ToString(format, CultureInfo.InvariantCulture);
    }

    public override string ToString() {
      return ToString("G", CultureInfo.InvariantCulture);
    }

    #endregion

  } // class Hiscore
} // //namespace Supay.Bot