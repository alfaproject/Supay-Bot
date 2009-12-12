namespace Supay.Bot.Irc.Messages.Modes {
  /// <summary>
  ///   This mode conceals the existence of the channel from other users. </summary>
  class SecretMode : FlagMode {

    /// <summary>
    /// Creates a new instance of the <see cref="SecretMode"/> class.
    /// </summary>
    public SecretMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SecretMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public SecretMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "s";
      }
    }

  }
}