namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   This mode is used to toggle the half-operator status of a channel member. </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Op")]
  class HalfOpMode : MemberStatusMode {

    /// <summary>
    /// Creates a new instance of the <see cref="HalfOpMode"/> class.
    /// </summary>
    public HalfOpMode() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="HalfOpMode"/> class with the given <see cref="ModeAction"/>.
    /// </summary>
    public HalfOpMode(ModeAction action) {
      this.Action = action;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="HalfOpMode"/> class 
    /// with the given <see cref="ModeAction"/> and member's nick.
    /// </summary>
    public HalfOpMode(ModeAction action, string nick) {
      this.Action = action;
      this.Nick = nick;
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return "h";
      }
    }

  }
}