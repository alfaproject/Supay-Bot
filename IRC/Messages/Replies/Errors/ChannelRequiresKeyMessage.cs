using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage received when attempting to join a channel which has a key set, and the user has not provided it. </summary>
  /// <remarks>
  ///   A channel can require a key with the ChannelModeMessage with a RegisteredNicksOnlyMode. </remarks>
  [Serializable]
  class ChannelRequiresKeyMessage : ErrorMessage, IChannelTargetedMessage {
    //:irc.dkom.at 475 _aLfa_ #chaos :Cannot join channel (+k)

    /// <summary>
    ///   Creates a new instance of the <see cref="ChannelRequiresKeyMessage"/> class. </summary>
    public ChannelRequiresKeyMessage()
      : base(475) {
    }

    /// <summary>
    /// Gets or sets the channel which has a key
    /// </summary>
    public string Channel {
      get {
        return channel;
      }
      set {
        channel = value;
      }
    }
    private string channel;

    /// <exclude />
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Channel);
      writer.AddParameter("Cannot join channel (+k)");
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Channel = string.Empty;
      if (parameters.Count > 2) {
        this.Channel = parameters[1];
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnChannelRequiresKey(new IrcMessageEventArgs<ChannelRequiresKeyMessage>(this));
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