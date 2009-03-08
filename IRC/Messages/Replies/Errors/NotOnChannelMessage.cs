using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage received when a user tries to perform a channel-specific operation on a user, 
  ///   and the user isn't in the channel. </summary>
  /// <remarks>
  ///   You will often get this if you attempt to kick a user but someone else kicks them before you do. 
  ///   If the user does not actually exist at all, 401 will be returned instead. </remarks>
  [Serializable]
  public class NotOnChannelMessage : ErrorMessage, IChannelTargetedMessage {
    //:irc.dkom.at 441 _aLfa_ bob #aLfBot :They aren't on that channel

    /// <summary>
    /// Creates a new instances of the <see cref="NotOnChannelMessage"/> class.
    /// </summary>
    public NotOnChannelMessage()
      : base() {
      this.InternalNumeric = 441;
    }

    /// <summary>
    /// Gets or sets the nick of the user being targeted
    /// </summary>
    public string Nick {
      get {
        return nick;
      }
      set {
        nick = value;
      }
    }
    private string nick;

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
      writer.AddParameter(this.Nick);
      writer.AddParameter(this.Channel);
      writer.AddParameter("They aren't on that channel");
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Nick = string.Empty;
      this.Channel = string.Empty;
      if (parameters.Count > 2) {
        this.Nick = parameters[1];
        this.Channel = parameters[2];
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnNotOnChannel(new IrcMessageEventArgs<NotOnChannelMessage>(this));
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