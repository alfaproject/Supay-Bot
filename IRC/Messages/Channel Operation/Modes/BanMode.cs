namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode defines a mask for users not allowed to join a channel. </summary>
  class BanMode : AccessControlMode {

    /// <summary>
    ///   Creates a new instance of the <see cref="BanMode"/> class. </summary>
    public BanMode() {
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="BanMode"/> class with the given <see cref="ModeAction"/>. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public BanMode(ModeAction action) {
      base.Action = action;
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="BanMode"/> class 
    ///   with the given <see cref="ModeAction"/> and <see cref="User"/>. </summary>
    public BanMode(ModeAction action, User mask) {
      base.Action = action;
      base.Mask = mask;
    }

    /// <summary>
    ///   Gets the IRC string representation of the mode being changed or applied. </summary>
    protected override string Symbol {
      get {
        return "b";
      }
    }

  } //class BanMode
} //namespace BigSister.Irc.Messages.Modes