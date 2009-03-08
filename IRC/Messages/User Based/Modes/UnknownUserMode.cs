namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   A user mode sent in a <see cref="UserModeMessage"/> which is not known. </summary>
  public class UnknownUserMode : UserMode {

    /// <summary>
    /// Creates a new instance of the <see cref="UnknownUserMode"/> class with the given <see cref="ModeAction"/> and value.
    /// </summary>
    public UnknownUserMode(ModeAction action, string symbol) {
      this.Action = action;
      this.symbol = symbol;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return symbol;
      }
    }

    private string symbol;

  }
}