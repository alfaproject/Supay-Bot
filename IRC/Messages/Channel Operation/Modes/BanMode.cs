namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode defines a mask for users not allowed to join a channel. </summary>
  public class BanMode : AccessControlMode {

    /// <summary>
    /// Creates a new instance of the <see cref="BanMode"/> class.
    /// </summary>
    public BanMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BanMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public BanMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BanMode"/> class 
    /// with the given <see cref="ModeAction"/> and <see cref="User"/>.
    /// </summary>
    public BanMode(ModeAction action, User mask) {
      this.Action = action;
      this.Mask = mask;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "b";
      }
    }

  }
}