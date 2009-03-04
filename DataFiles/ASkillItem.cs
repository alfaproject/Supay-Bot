using System.Globalization;

namespace BigSister {

  public abstract class ASkillItem {

    public ASkillItem(string[] tokens) {
      this.Skill = tokens[0];
      this.Level = int.Parse(tokens[1], CultureInfo.InvariantCulture);
      this.Exp = float.Parse(tokens[2], CultureInfo.InvariantCulture);
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

    public float Exp {
      get;
      set;
    }

    public string Name {
      get;
      set;
    }

    public abstract string IrcColour {
      get;
    }

  } //abstract class ASkillItem

} //namespace BigSister