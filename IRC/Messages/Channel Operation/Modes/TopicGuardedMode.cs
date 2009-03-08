namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode is used to restrict the usage of the <see cref="BigSister.Irc.Messages.TopicMessage"/> to channel operators. </summary>
  public class TopicGuardedMode : FlagMode {

    /// <summary>
    /// Creates a new instance of the <see cref="TopicGuardedMode"/> class.
    /// </summary>
    public TopicGuardedMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="TopicGuardedMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public TopicGuardedMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "t";
      }
    }

  }
}