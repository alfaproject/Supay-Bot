namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode conceals the existence of the channel from other users. </summary>
  public class PrivateMode : FlagMode {

    /// <summary>
    /// Creates a new instance of the <see cref="PrivateMode"/> class.
    /// </summary>
    public PrivateMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PrivateMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public PrivateMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "p";
      }
    }

  }
}