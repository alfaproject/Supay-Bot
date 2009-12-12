using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The reply to a <see cref="VersionRequestMessage"/>. </summary>
  [Serializable]
  class VersionReplyMessage : CtcpReplyMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="VersionReplyMessage"/> class.
    /// </summary>
    public VersionReplyMessage()
      : base() {
      this.InternalCommand = "VERSION";
    }

    /// <summary>
    /// Gets or sets the version of the client.
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
      conduit.OnVersionReply(new IrcMessageEventArgs<VersionReplyMessage>(this));
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