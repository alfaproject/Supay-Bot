namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This category of modes is used to control access to a channel. </summary>
  abstract class AccessControlMode : ChannelMode {

    /// <summary>
    /// Gets or sets the mask applied to this mode.
    /// </summary>
    public virtual User Mask {
      get {
        return mask;
      }
      set {
        mask = value;
      }
    }
    private User mask = new User();

    /// <summary>
    /// Applies this mode to the ModeArguments property of the given <see cref="ChannelModeMessage" />.
    /// </summary>
    /// <param name="msg">The message which will be modified to include this mode.</param>
    protected override void AddParameter(BigSister.Irc.Messages.ChannelModeMessage msg) {
      msg.ModeArguments.Add(this.Mask.ToString());
    }

    /// <summary>
    /// A string representation of the mode.
    /// </summary>
    public override string ToString() {
      return base.ToString() + " " + this.Mask.ToString();
    }

  }
}