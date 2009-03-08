using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Reply to a <see cref="WhoIsMessage"/>, stating the channels a user is in. </summary>
  [Serializable]
  public class WhoIsChannelsReplyMessage : NumericMessage, IChannelTargetedMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="WhoIsChannelsReplyMessage"/> class.
    /// </summary>
    public WhoIsChannelsReplyMessage()
      : base() {
      this.InternalNumeric = 319;
    }

    /// <summary>
    /// Gets or sets the Nick of the user being 
    /// </summary>
    public virtual string Nick {
      get {
        return nick;
      }
      set {
        nick = value;
      }
    }

    /// <summary>
    /// Gets the collection of channels the user is a member of.
    /// </summary>
    public virtual StringCollection Channels {
      get {
        return channels;
      }
    }

    private string nick = string.Empty;
    private StringCollection channels = new StringCollection();

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Nick);
      if (this.Channels.Count != 0) {
        writer.AddList(this.Channels, " ", false);
      }
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Nick = string.Empty;
      this.Channels.Clear();

      if (parameters.Count == 3) {
        this.Nick = parameters[1];
        this.Channels.AddRange(parameters[2].Split(' '));
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnWhoIsChannelsReply(new IrcMessageEventArgs<WhoIsChannelsReplyMessage>(this));
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