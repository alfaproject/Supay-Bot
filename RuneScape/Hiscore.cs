using System;
using System.Globalization;

namespace Supay.Bot
{
  internal abstract class Hiscore : IFormattable
  {
    protected Hiscore()
    {
    }

    protected Hiscore(int rank)
    {
      this.Rank = rank;
    }

    protected Hiscore(string name, int rank)
      : this(rank)
    {
      this.Name = name;
    }

    public string RSN
    {
      get;
      set;
    }

    public string Name
    {
      get;
      protected set;
    }

    public int Rank
    {
      get;
      set;
    }

    #region IFormattable

    public abstract string ToString(string format, IFormatProvider formatProvider);

    public string ToString(string format)
    {
      return this.ToString(format, CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
      return this.ToString("G", CultureInfo.InvariantCulture);
    }

    #endregion
  }
}
