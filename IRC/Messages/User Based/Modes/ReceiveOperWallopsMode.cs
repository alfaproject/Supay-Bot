namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode signifies that the user will receive debug messages. </summary>
  public class ReceiveOperWallopsMode : UserMode {

    /// <summary>
    /// Creates a new instance of the <see cref="ReceiveOperWallopsMode"/> class.
    /// </summary>
    public ReceiveOperWallopsMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ReceiveOperWallopsMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public ReceiveOperWallopsMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "z";
      }
    }

  }
}