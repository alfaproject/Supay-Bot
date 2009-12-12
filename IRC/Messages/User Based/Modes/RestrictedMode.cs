namespace Supay.Bot.Irc.Messages.Modes {
  /// <summary>
  ///   This mode signifies that the user has a restricted connection. </summary>
  class RestrictedMode : UserMode {

    /// <summary>
    /// Creates a new instance of the <see cref="RestrictedMode"/> class.
    /// </summary>
    public RestrictedMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RestrictedMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public RestrictedMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "r";
      }
    }

  }
}