using System.Globalization;

namespace Supay.Bot {
  class SkillItem {

    public SkillItem(string[] tokens) {
      this.Skill = tokens[0];
      this.Level = int.Parse(tokens[1], CultureInfo.InvariantCulture);
      this.Exp = double.Parse(tokens[2], CultureInfo.InvariantCulture);
      this.Name = tokens[3];
    }

    public string Skill {
      get;
      set;
    }

    public int Level {
      get;
      set;
    }

    public double Exp {
      get;
      set;
    }

    public string Name {
      get;
      set;
    }

    public virtual string IrcColour {
      get {
        return "07";
      }
    }

  } //class SkillItem
} //namespace Supay.Bot