using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The reply for the <see cref="UserInfoRequestMessage"/> query. </summary>
  [Serializable]
  class UserInfoReplyMessage : CtcpReplyMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="UserInfoReplyMessage"/> class.
    /// </summary>
    public UserInfoReplyMessage()
      : base() {
      this.InternalCommand = "USERINFO";
    }

    /// <summary>
    /// The information that the client wants to return.
    /// </summary>
    public virtual string Response {
      get {
        return this.response;
      }
      set {
        this.response = value;
      }
    }
    private string response = string.Empty;

    /// <summary>
    /// Gets the data payload of the Ctcp request.
    /// </summary>
    protected override string ExtendedData {
      get {
        return this.response;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnUserInfoReply(new IrcMessageEventArgs<UserInfoReplyMessage>(this));
    }

    /// <summary>
    /// Parses the given string to populate this <see cref="IrcMessage"/>.
    /// </summary>
    public override void Parse(string unparsedMessage) {
      base.Parse(unparsedMessage);
      this.Response = CtcpUtil.GetExtendedData(unparsedMessage);
    }

  }
}