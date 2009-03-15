namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode is used to give and take voice privilege to/from a channel member. 
  ///   Users with this privilege can talk on moderated channels. (<see cref="ModeratedMode"/>) </summary>
  class VoiceMode : MemberStatusMode {

    /// <summary>
    /// Creates a new instance of the <see cref="VoiceMode"/> class.
    /// </summary>
    public VoiceMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="VoiceMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public VoiceMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="VoiceMode"/> class 
    /// with the given <see cref="ModeAction"/> and member's nick.
    /// </summary>
    public VoiceMode(ModeAction action, string nick) {
      this.Action = action;
      this.Nick = nick;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "v";
      }
    }

  }
}