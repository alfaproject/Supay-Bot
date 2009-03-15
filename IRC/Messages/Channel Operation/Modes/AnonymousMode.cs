namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode defines an anonymous channel.</summary>
  /// <remarks>
  ///   <p>
  ///   This means that when a message sent to the channel is sent by the server to users, 
  ///   and the origin is a user, then it MUST be masked. 
  ///   To mask the message, the origin is changed to "anonymous!anonymous@anonymous."
  ///   </p> </remarks>
  class AnonymousMode : FlagMode {

    /// <summary>
    /// Creates a new instance of the <see cref="AnonymousMode"/> class.
    /// </summary>
    public AnonymousMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="AnonymousMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public AnonymousMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "a";
      }
    }

  }
}