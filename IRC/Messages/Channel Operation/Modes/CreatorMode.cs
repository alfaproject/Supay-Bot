namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   Servers use this mode to give the user creating a safe channel the status of "channel creator". </summary>
  class CreatorMode : MemberStatusMode {

    /// <summary>
    /// Creates a new instance of the <see cref="CreatorMode"/> class.
    /// </summary>
    public CreatorMode() {
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="CreatorMode"/> class with the given <see cref="ModeAction"/>. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public CreatorMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="CreatorMode"/> class 
    ///   with the given <see cref="ModeAction"/> and member's nick. </summary>
    public CreatorMode(ModeAction action, string nick) {
      base.Action = action;
      base.Nick = nick;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "O";
      }
    }

  }
}