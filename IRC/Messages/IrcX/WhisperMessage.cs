using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   With the WhisperMessage, clients can send messages to people within the context of a channel. </summary>
  [Serializable]
  public class WhisperMessage : CommandMessage, IChannelTargetedMessage {

    /// <summary>
    /// Creates a new instance of the WhisperMessage class.
    /// </summary>
    public WhisperMessage()
      : base() {
    }

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "WHISPER";
      }
    }

    /// <summary>
    /// Gets or sets the channel being targeted.
    /// </summary>
    public virtual string Channel {
      get {
        return channel;
      }
      set {
        channel = value;
      }
    }
    private string channel = string.Empty;

    /// <summary>
    /// Gets the target of this <see cref="TextMessage"/>.
    /// </summary>
    public virtual StringCollection Targets {
      get {
        return this.targets;
      }
    }
    private StringCollection targets = new StringCollection();

    /// <summary>
    /// Gets or sets the actual text of this <see cref="TextMessage"/>.
    /// </summary>
    /// <remarks>
    /// This property holds the core purpose of irc itself... sending text communication to others.
    /// </remarks>
    public virtual string Text {
      get {
        return this.text;
      }
      set {
        this.text = value;
      }
    }
    private string text = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Channel);
      writer.AddList(this.Targets, ",", false);
      writer.AddParameter(this.Text, true);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Targets.Clear();
      if (parameters.Count > 2) {
        this.Channel = parameters[0];
        this.Targets.AddRange(parameters[1].Split(','));
        this.Text = parameters[2];
      } else {
        this.Channel = string.Empty;
        this.Text = string.Empty;
      }
    }

    /// <summary>
    /// Validates this message's properties according to the given <see cref="ServerSupport"/>.
    /// </summary>
    public override void Validate(ServerSupport serverSupport) {
      base.Validate(serverSupport);
      this.Channel = MessageUtil.EnsureValidChannelName(this.Channel, serverSupport);
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnWhisper(new IrcMessageEventArgs<WhisperMessage>(this));
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
      ;
    }

    #endregion

  }
}