using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   A reply to a <see cref="WhoMessage"/> query. </summary>
  [Serializable]
  public class WhoReplyMessage : NumericMessage, IChannelTargetedMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="WhoReplyMessage"/> class.
    /// </summary>
    public WhoReplyMessage()
      : base() {
      this.InternalNumeric = 352;
    }

    /// <summary>
    /// Gets or sets the channel associated with the user.
    /// </summary>
    /// <remarks>
    /// In the case of a non-channel based WhoMessage, 
    /// Channel will contain the most recent channel which the user joined and is still on.
    /// </remarks>
    public virtual string Channel {
      get {
        return channel;
      }
      set {
        channel = value;
      }
    }

    /// <summary>
    /// Gets or sets the user being examined.
    /// </summary>
    public virtual BigSister.Irc.User User {
      get {
        return user;
      }
      set {
        user = value;
      }
    }

    /// <summary>
    /// Gets or sets the status of the user on the associated channel.
    /// </summary>
    public virtual ChannelStatus Status {
      get {
        return status;
      }
      set {
        Status = value;
      }
    }

    /// <summary>
    /// Gets or sets the server the user is on.
    /// </summary>
    public virtual string Server {
      get {
        return server;
      }
      set {
        server = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of hops to the server the user is on.
    /// </summary>
    public virtual int HopCount {
      get {
        return hopCount;
      }
      set {
        hopCount = value;
      }
    }

    /// <summary>
    ///   Gets or sets if the user is an irc operator. </summary>
    public virtual bool IsOper {
      get {
        return isOper;
      }
      set {
        isOper = value;
      }
    }

    private string channel = string.Empty;
    private User user = new User();
    private string server = string.Empty;
    private int hopCount = -1;
    private bool isOper = false;
    private ChannelStatus status = ChannelStatus.None;


    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Channel);
      writer.AddParameter(this.User.UserName);
      writer.AddParameter(this.User.HostName);
      writer.AddParameter(this.Server);
      writer.AddParameter(this.User.Nick);
      //HACK it could also be a G, but I was unable to determine what it meant.
      if (this.IsOper) {
        writer.AddParameter("H*");
      } else {
        writer.AddParameter("H");
      }
      writer.AddParameter(this.Status.ToString());
      writer.AddParameter(this.HopCount.ToString(CultureInfo.InvariantCulture) + " " + this.User.RealName);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.User = new User();
      if (parameters.Count == 8) {

        this.Channel = parameters[1];
        this.User.UserName = parameters[2];
        this.User.HostName = parameters[3];
        this.Server = parameters[4];
        this.User.Nick = parameters[5];

        string levels = parameters[6];
        // TODO I'm going to ignore the H/G issue until i know what it means.
        this.IsOper = (levels.IndexOf("*", StringComparison.Ordinal) != -1);
        string lastLetter = levels.Substring(levels.Length - 1);
        if (ChannelStatus.Exists(lastLetter)) {
          //HACK this.Status = ChannelStatus.GetInstance(lastLetter);
        } else {
          //HACK this.Status = ChannelStatus.None;
        }

        string trailing = parameters[7];
        this.HopCount = Convert.ToInt32(trailing.Substring(0, trailing.IndexOf(" ", StringComparison.Ordinal)), CultureInfo.InvariantCulture);
        this.User.RealName = trailing.Substring(trailing.IndexOf(" ", StringComparison.Ordinal));
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnWhoReply(new IrcMessageEventArgs<WhoReplyMessage>(this));
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