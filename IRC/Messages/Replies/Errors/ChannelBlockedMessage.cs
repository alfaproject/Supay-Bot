using System;
using System.Collections.Specialized;
using System.Globalization;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage received when attempting to join a channel which invite-only. </summary>
  /// <remarks>
  ///   A channel can be set invite-only with a ChannelModeMessage containing a InviteOnlyMode. </remarks>
  [Serializable]
  class ChannelBlockedMessage : ErrorMessage, IChannelTargetedMessage {
    //:irc.dkom.at 485 _aLfa_ #chaos :Cannot join channel (Q-Line Blocked)

    /// <summary>
    ///  Creates a new instance of the <see cref="ChannelBlockedMessage"/> class. </summary>
    public ChannelBlockedMessage()
      : base(485) {
    }

    /// <summary>
    /// Gets or sets the channel which is blocked
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

    /// <summary>
    /// Gets or sets the reason the channel is blocked
    /// </summary>
    public string Reason {
      get {
        return reason;
      }
      set {
        reason = value;
      }
    }
    private string reason;


    /// <exclude />
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Channel);
      writer.AddParameter("Cannot join channel ({0})".FormatWith(this.Reason));
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);

      this.Channel = string.Empty;
      this.Reason = string.Empty;

      if (parameters.Count > 2) {
        this.Channel = parameters[1];
        this.Reason = MessageUtil.StringBetweenStrings(parameters[2], "Cannot join channel (", ")");
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnChannelBlocked(new IrcMessageEventArgs<ChannelBlockedMessage>(this));
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