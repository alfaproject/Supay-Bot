using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Used to indicate the given channel name is invalid. </summary>
  [Serializable]
  public class NoSuchChannelMessage : ErrorMessage, IChannelTargetedMessage {
    // :irc.easynews.com 403 aLfBot #qwe9r8wjfq98wer :No such channel

    /// <summary>
    ///   Creates a new instance of the <see cref="NoSuchChannelMessage"/> class. </summary>
    public NoSuchChannelMessage()
      : base(403) {
    }

    /// <summary>
    /// Gets or sets the Channel which was empty or didn't exist.
    /// </summary>
    public virtual string Channel {
      get {
        return _channel;
      }
      set {
        _channel = value;
      }
    }

    private string _channel = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Channel);
      writer.AddParameter("No such channel");
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count > 1) {
        this.Channel = parameters[1];
      } else {
        this.Channel = string.Empty;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnNoSuchChannel(new IrcMessageEventArgs<NoSuchChannelMessage>(this));
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