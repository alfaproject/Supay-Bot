namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   The modes in this category have a channel member nickname property, <see cref="MemberStatusMode.Nick"/> and affect the privileges given to this user. </summary>
  abstract class MemberStatusMode : ChannelMode {

    /// <summary>
    /// Gets or sets the nickname of the channel member who will be affected by this mode.
    /// </summary>
    public virtual string Nick {
      get {
        return nick;
      }
      set {
        nick = value;
      }
    }
    private string nick = string.Empty;

    /// <summary>
    /// Applies this mode to the ModeArguments property of the given <see cref="ChannelModeMessage" />.
    /// </summary>
    /// <param name="msg">The message which will be modified to include this mode.</param>
    protected override void AddParameter(BigSister.Irc.Messages.ChannelModeMessage msg) {
      msg.ModeArguments.Add(this.Nick);
    }

  }
}