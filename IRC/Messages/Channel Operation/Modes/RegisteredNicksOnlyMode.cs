namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   When this mode is set, 
  ///   new members are only accepted if their nick is registered. </summary>
  class RegisteredNicksOnlyMode : ChannelMode {

    /// <summary>
    /// Creates a new instance of the <see cref="RegisteredNicksOnlyMode"/> class.
    /// </summary>
    public RegisteredNicksOnlyMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RegisteredNicksOnlyMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public RegisteredNicksOnlyMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "R";
      }
    }

  }
}