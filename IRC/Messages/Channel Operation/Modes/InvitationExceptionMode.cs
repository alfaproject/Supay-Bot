namespace Supay.Bot.Irc.Messages.Modes {
  /// <summary>
  ///   For channels which have the invite-only flag set (<see cref="InviteOnlyMode"/>), 
  ///   users whose address matches an invitation mask set for the channel are allowed 
  ///   to join the channel without any invitation. </summary>
  class InvitationExceptionMode : AccessControlMode {

    /// <summary>
    /// Creates a new instance of the <see cref="InvitationExceptionMode"/> class.
    /// </summary>
    public InvitationExceptionMode() {
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="InvitationExceptionMode"/> class with the given <see cref="ModeAction"/>. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public InvitationExceptionMode(ModeAction action) {
      base.Action = action;
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="InvitationExceptionMode"/> class 
    ///   with the given <see cref="ModeAction"/> and <see cref="User"/>. </summary>
    public InvitationExceptionMode(ModeAction action, User mask) {
      base.Action = action;
      base.Mask = mask;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "I";
      }
    }

  }
}