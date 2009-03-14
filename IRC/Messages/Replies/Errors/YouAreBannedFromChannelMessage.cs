using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage received when attempting to join a channel on which the user is banned. </summary>
  [Serializable]
  public class YouAreBannedFromChannelMessage : ErrorMessage, IChannelTargetedMessage {
    //:irc.dkom.at 474 _aLfa_ #chaos :Cannot join channel (+b)

    /// <summary>
    ///   Creates a new instance of the <see cref="YouAreBannedFromChannelMessage"/> class. </summary>
    public YouAreBannedFromChannelMessage()
      : base(474) {
    }

    /// <summary>
    /// Gets or sets the channel on which the user is banned
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
      writer.AddParameter("Cannot join channel (+b)");
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
      conduit.OnYouAreBannedFromChannel(new IrcMessageEventArgs<YouAreBannedFromChannelMessage>(this));
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