using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The JoinMessage is used by client to start listening a specific channel. </summary>
  /// <remarks>
  ///   Whether or not a client is allowed to join a channel is checked only by the server the client is connected to;
  ///   all other servers automatically add the user to the channel when it is received from other servers.
  ///   This message wraps the JOIN command. </remarks>
  [Serializable]
  class JoinMessage : CommandMessage, IChannelTargetedMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="JoinMessage"/> class.
    /// </summary>
    public JoinMessage()
      : base() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="JoinMessage"/> class with the given channel.
    /// </summary>
    /// <param name="channel">The name of the channel to join.</param>
    public JoinMessage(string channel)
      : base() {
      this.channels.Add(channel);
    }

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "JOIN";
      }
    }

    /// <summary>
    /// Gets the channel names joined
    /// </summary>
    public virtual StringCollection Channels {
      get {
        return this.channels;
      }
    }

    /// <summary>
    /// Gets the key (password) of the channels
    /// </summary>
    /// <remarks>Only relevant for channels that have a key</remarks>
    public virtual StringCollection Keys {
      get {
        return this.keys;
      }
    }

    private StringCollection channels = new StringCollection();
    private StringCollection keys = new StringCollection();

    /// <summary>
    /// Validates this message against the given server support
    /// </summary>
    public override void Validate(ServerSupport serverSupport) {
      base.Validate(serverSupport);
      for (int i = 0; i < Channels.Count; i++) {
        Channels[i] = MessageUtil.EnsureValidChannelName(Channels[i], serverSupport);
      }
    }


    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddList(this.Channels, ",", true);
      if (this.Keys.Count != 0) {
        writer.AddList(this.Keys, ",", true);
      }
    }


    /// <summary>
    /// Parses the parameters portion of the message
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Channels.Clear();
      this.Keys.Clear();
      if (parameters.Count > 0) {
        this.Channels.AddRange(parameters[0].Split(','));
        if (parameters.Count > 1) {
          this.Keys.AddRange(parameters[1].Split(','));
        }
      }
    }


    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnJoin(new IrcMessageEventArgs<JoinMessage>(this));
    }

    #region IChannelTargetedMessage Members

    bool IChannelTargetedMessage.IsTargetedAtChannel(string channelName) {
      return IsTargetedAtChannel(channelName);
    }

    /// <summary>
    /// Determines if the the current message is targeted at the given channel.
    /// </summary>
    protected virtual bool IsTargetedAtChannel(string channelName) {
      return MessageUtil.ContainsIgnoreCaseMatch(this.Channels, channelName);
    }

    #endregion

  }
}