namespace BigSister {
  class SkillItem : ASkillItem {

    public SkillItem(string[] tokens)
      : base (tokens) {
    }

    public override string IrcColour {
      get {
        return "07";
      }
    }

  } //class SkillItem
} //namespace BigSister