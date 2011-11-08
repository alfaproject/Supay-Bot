using System.Globalization;

namespace Supay.Bot {
  internal class SkillItem {
    public SkillItem(string[] tokens) {
      Skill = tokens[0];
      Level = int.Parse(tokens[1], CultureInfo.InvariantCulture);
      Exp = double.Parse(tokens[2], CultureInfo.InvariantCulture);
      Name = tokens[3];
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
  }
}
