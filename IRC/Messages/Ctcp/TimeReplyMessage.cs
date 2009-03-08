using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The reply to the <see cref="TimeRequestMessage"/> query. </summary>
  [Serializable]
  public class TimeReplyMessage : CtcpReplyMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="TimeReplyMessage"/> class.
    /// </summary>
    public TimeReplyMessage()
      : base() {
      this.InternalCommand = "TIME";
    }

    /// <summary>
    /// Gets or sets the time, sent in any format the client finds useful.
    /// </summary>
    public virtual string CurrentTime {
      get {
        return this.currentTime;
      }
      set {
        this.currentTime = value;
      }
    }
    private string currentTime = string.Empty;

    /// <summary>
    /// Gets the data payload of the Ctcp request.
    /// </summary>
    protected override string ExtendedData {
      get {
        return this.currentTime;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnTimeReply(new IrcMessageEventArgs<TimeReplyMessage>(this));
    }

    /// <summary>
    /// Parses the given string to populate this <see cref="IrcMessage"/>.
    /// </summary>
    public override void Parse(string unparsedMessage) {
      base.Parse(unparsedMessage);
      this.CurrentTime = CtcpUtil.GetExtendedData(unparsedMessage);
    }

  }
}