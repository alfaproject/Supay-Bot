using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage received when a user tries to perform a channel-specific operation on a user, 
  ///   and the user isn't in the channel. </summary>
  /// <remarks>
  ///   Although all networks have a limit on the total number of bans allowed, 
  ///   not all networks will tell you when the list is full. 
  ///   (they will simply ignore extra bans.) </remarks>
  [Serializable]
  class BanListFullMessage : ErrorMessage, IChannelTargetedMessage {
    //:irc.dkom.at 478 _aLfa_ #aLfBot *!*@aol.com :Channel ban/ignore list is full

    /// <summary>
    ///   Creates a new instance of the <see cref="BanListFullMessage"/> class. </summary>
    public BanListFullMessage()
      : base(478) {
    }

    /// <summary>
    /// Gets or sets the mask of the user being banned
    /// </summary>
    public User BanMask {
      get {
        return banMask;
      }
      set {
        banMask = value;
      }
    }
    private User banMask;

    /// <summary>
    /// Gets or sets the channel being targeted
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
      writer.AddParameter(this.BanMask.ToString());
      writer.AddParameter("Channel ban/ignore list is full");
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Channel = string.Empty;
      this.BanMask = new User();
      if (parameters.Count > 2) {
        this.Channel = parameters[1];
        this.BanMask.Parse(parameters[2]);
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnBanListFull(new IrcMessageEventArgs<BanListFullMessage>(this));
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