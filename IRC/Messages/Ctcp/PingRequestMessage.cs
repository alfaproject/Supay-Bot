using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A client-to-client ping request message. </summary>
  [Serializable]
  class PingRequestMessage : CtcpRequestMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="PingRequestMessage"/> class.
    /// </summary>
    public PingRequestMessage()
      : base() {
      this.InternalCommand = "PING";
    }

    /// <summary>
    /// The custom timestamp to send in the ping request.
    /// </summary>
    /// <remarks>
    /// The ping reply should have this same exact timestamp,
    /// so you could subtract the original timestamp with the
    /// current one to determine the lag time.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TimeStamp")]
    public virtual string TimeStamp {
      get {
        return this.timeStamp;
      }
      set {
        this.timeStamp = value;
      }
    }
    private string timeStamp = string.Empty;

    /// <summary>
    /// Gets the data payload of the Ctcp request.
    /// </summary>
    protected override string ExtendedData {
      get {
        return this.timeStamp;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnPingRequest(new IrcMessageEventArgs<PingRequestMessage>(this));
    }

    /// <summary>
    /// Parses the given string to populate this <see cref="IrcMessage"/>.
    /// </summary>
    public override void Parse(string unparsedMessage) {
      base.Parse(unparsedMessage);
      this.TimeStamp = CtcpUtil.GetExtendedData(unparsedMessage);
    }

  }
}