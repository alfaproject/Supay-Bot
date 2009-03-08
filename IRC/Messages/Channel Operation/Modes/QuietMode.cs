namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode restricts the type of data sent to users about the channel operations: 
  ///   other user joins, parts and nick changes are not sent. </summary>
  public class QuietMode : FlagMode {

    /// <summary>
    /// Creates a new instance of the <see cref="QuietMode"/> class.
    /// </summary>
    public QuietMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="QuietMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public QuietMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "q";
      }
    }

  }
}