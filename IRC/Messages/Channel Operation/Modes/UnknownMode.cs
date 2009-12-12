namespace Supay.Bot.Irc.Messages.Modes {
  /// <summary>
  ///   A channel mode sent in a <see cref="ChannelModeMessage"/> which is not known.</summary>
  class UnknownChannelMode : ChannelMode {

    /// <summary>
    /// Creates a new instance of the <see cref="UnknownChannelMode"/> class with the given <see cref="ModeAction"/> and value.
    /// </summary>
    public UnknownChannelMode(ModeAction action, string symbol) {
      this.Action = action;
      this.symbol = symbol;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="UnknownChannelMode"/> class with the given <see cref="ModeAction"/>, value, and parameter.
    /// </summary>
    public UnknownChannelMode(ModeAction action, string symbol, string parameter) {
      this.Action = action;
      this.symbol = symbol;
      this.parameter = parameter;
    }

    /// <summary>
    /// Gets or sets the parameter passed with this mode.
    /// </summary>
    public virtual string Parameter {
      get {
        return parameter;
      }
      set {
        parameter = value;
      }
    }

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected override string Symbol {
      get {
        return symbol;
      }
    }


    private string symbol;
    private string parameter = string.Empty;

    /// <summary>
    /// Applies this mode to the ModeArguments property of the given <see cref="ChannelModeMessage" />.
    /// </summary>
    /// <param name="msg">The message which will be modified to include this mode.</param>
    protected override void AddParameter(Supay.Bot.Irc.Messages.ChannelModeMessage msg) {
      if (this.Parameter.Length != 0) {
        msg.ModeArguments.Add(this.Parameter);
      }
    }

  }
}