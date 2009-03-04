using System;
using System.Collections.Generic;
using System.Text;

namespace BigSister {
  
  public abstract class Hiscore : IFormattable {

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

    #endregion

  } // class Hiscore

} // namespace BigSister