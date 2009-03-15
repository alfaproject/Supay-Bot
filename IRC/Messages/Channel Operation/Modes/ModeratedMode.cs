namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode is used to control who may speak on a channel. 
  ///   When it is set, only channel operators, 
  ///   and members who have been given the voice privilege may send messages to the channel. </summary>
  class ModeratedMode : FlagMode {

    /// <summary>
    /// Creates a new instance of the <see cref="ModeratedMode"/> class.
    /// </summary>
    public ModeratedMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ModeratedMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public ModeratedMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "m";
      }
    }

  }
}