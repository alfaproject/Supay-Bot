using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   I see no real need for this message... but it should generate an <see cref="ErrorReplyMessage"/> from the target. </summary>
  [Serializable]
  class ErrorRequestMessage : CtcpRequestMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="ErrorRequestMessage"/> class.
    /// </summary>
    public ErrorRequestMessage()
      : base() {
      this.InternalCommand = "ERRMSG";
    }

    /// <summary>
    /// Gets or sets the string to be parroted back to you, with an indication that no error occured.
    /// </summary>
    public virtual string Query {
      get {
        return this.query;
      }
      set {
        this.query = value;
      }
    }
    private string query = string.Empty;

    /// <summary>
    /// Gets the data payload of the Ctcp request.
    /// </summary>
    protected override string ExtendedData {
      get {
        return this.query;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnErrorRequest(new IrcMessageEventArgs<ErrorRequestMessage>(this));
    }

    /// <summary>
    /// Parses the given string to populate this <see cref="IrcMessage"/>.
    /// </summary>
    public override void Parse(string unparsedMessage) {
      base.Parse(unparsedMessage);
      this.Query = CtcpUtil.GetExtendedData(unparsedMessage);
    }

  }
}