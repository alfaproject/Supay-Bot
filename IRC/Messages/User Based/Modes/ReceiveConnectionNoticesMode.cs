namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode signifies that the user will receive client connection notices. </summary>
  class ReceiveConnectionNoticesMode : UserMode {

    /// <summary>
    /// Creates a new instance of the <see cref="ReceiveConnectionNoticesMode"/> class.
    /// </summary>
    public ReceiveConnectionNoticesMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ReceiveConnectionNoticesMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public ReceiveConnectionNoticesMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "c";
      }
    }

  }
}