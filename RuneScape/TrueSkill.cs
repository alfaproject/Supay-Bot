namespace Supay.Bot
{
  internal class TrueSkill : Skill
  {
    private const int MAX_LEVEL = 120;

    public TrueSkill(string name, int rank, int level, int exp)
      : base(name, rank, level, exp)
    {
    }

    public TrueSkill(string name, int rank, long exp)
      : base(name, rank)
    {
      this.Exp = exp;

      this.Level = exp.ToLevel();
      if (this.Level > MAX_LEVEL)
      {
        this.Level = MAX_LEVEL;
      }
    }

    public override int MaxLevel
    {
      get
      {
        return MAX_LEVEL;
      }
    }
  }
}
