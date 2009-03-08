namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode signifies that the user will receive 'I-line is full' messages. </summary>
  public class ReceiveILineFullNoticesMode : UserMode {

    /// <summary>
    /// Creates a new instance of the <see cref="ReceiveILineFullNoticesMode"/> class.
    /// </summary>
    public ReceiveILineFullNoticesMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ReceiveILineFullNoticesMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public ReceiveILineFullNoticesMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "f";
      }
    }

  }
}