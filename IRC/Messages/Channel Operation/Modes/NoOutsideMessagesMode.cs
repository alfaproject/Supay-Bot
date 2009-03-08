namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   When this mode is set, only channel members can send messages to the channel. </summary>
  public class NoOutsideMessagesMode : FlagMode {

    /// <summary>
    /// Creates a new instance of the <see cref="NoOutsideMessagesMode"/> class.
    /// </summary>
    public NoOutsideMessagesMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="NoOutsideMessagesMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public NoOutsideMessagesMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "n";
      }
    }

  }
}