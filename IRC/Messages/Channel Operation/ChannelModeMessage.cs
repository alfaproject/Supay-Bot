using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The ChannelModeMessage allows channels to have their mode changed. </summary>
  /// <remarks>
  ///   Modes include such things as channel user limits and passwords, as well as the bans list and settings ops.
  ///   This message wraps the MODE command. </remarks>
  [Serializable]
  class ChannelModeMessage : CommandMessage, IChannelTargetedMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "MODE";
      }
    }

    /// <summary>
    /// Creates a new instance of the ChannelModeMessage class.
    /// </summary>
    public ChannelModeMessage() {
    }

    /// <summary>
    ///   Creates a new instance of the ChannelModeMessage class and applies the given parameters. </summary>
    /// <param name="channel">
    ///   The name of the channel being affected. </param>
    /// <param name="modeChanges">
    ///   The mode changes being applied. </param>
    /// <param name="modeArguments">
    ///   The arguments (parameters) for the <see cref="ChannelModeMessage.ModeChanges"/> property. </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public ChannelModeMessage(string channel, string modeChanges, params string[] modeArguments) {
      this.channel = channel;
      this.modeChanges = modeChanges;
      this.modeArguments.AddRange(modeArguments);
    }

    /// <summary>
    /// Gets or sets the name of the channel being affected.
    /// </summary>
    public virtual string Channel {
      get {
        return this.channel;
      }
      set {
        this.channel = value;
      }
    }
    private string channel = string.Empty;

    /// <summary>
    /// Gets or sets the mode changes being applied.
    /// </summary>
    /// <remarks>
    /// An example ModeChanges might look like "+ool".
    /// This means adding the cannel op mode for two users, and setting a limit on the user count.
    /// </remarks>
    public virtual string ModeChanges {
      get {
        return this.modeChanges;
      }
      set {
        this.modeChanges = value;
      }
    }
    private string modeChanges = string.Empty;

    /// <summary>
    /// Gets the collection of arguments ( parameters ) for the <see cref="ChannelModeMessage.ModeChanges"/> property.
    /// </summary>
    /// <remarks>
    /// Some modes require a parameter, such as +o requires the mask of the person to be given ops.
    /// </remarks>
    public virtual StringCollection ModeArguments {
      get {
        return this.modeArguments;
      }
    }
    private StringCollection modeArguments = new StringCollection();

    /// <summary>
    ///   Determines if the message can be parsed by this type. </summary>
    public override bool CanParse(string unparsedMessage) {
      if (!base.CanParse(unparsedMessage)) {
        return false;
      }
      StringCollection p = MessageUtil.GetParameters(unparsedMessage);
      if (p.Count >= 1) {
        return MessageUtil.HasValidChannelPrefix(p[0]);
      } else {
        return false;
      }
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Channel = parameters[0];

      if (parameters.Count > 1) {
        this.ModeChanges = parameters[1];
      } else {
        this.ModeChanges = string.Empty;
      }

      this.ModeArguments.Clear();
      if (parameters.Count > 2) {
        for (int i = 2; i < parameters.Count; i++) {
          this.ModeArguments.Add(parameters[i]);
        }
      }
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Channel);
      if (!string.IsNullOrEmpty(this.ModeChanges)) {
        writer.AddParameter(this.ModeChanges);
        foreach (string modeArg in this.ModeArguments) {
          writer.AddParameter(modeArg);
        }
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnChannelMode(new IrcMessageEventArgs<ChannelModeMessage>(this));
    }

    #region IChannelTargetedMessage Members

    bool IChannelTargetedMessage.IsTargetedAtChannel(string channelName) {
      return IsTargetedAtChannel(channelName);
    }

    /// <summary>
    /// Determines if the the current message is targeted at the given channel.
    /// </summary>
    protected virtual bool IsTargetedAtChannel(string channelName) {
      return MessageUtil.IsIgnoreCaseMatch(this.Channel, channelName);
    }

    #endregion

  }
}