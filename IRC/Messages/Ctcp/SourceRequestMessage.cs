using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   A request to know where the target's client be downloaded from. </summary>
  [Serializable]
  public class SourceRequestMessage : CtcpRequestMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="SourceRequestMessage"/> class. </summary>
    public SourceRequestMessage()
      : base() {
      this.InternalCommand = "SOURCE";
    }

    /// <summary>
    ///   Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass. </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnSourceRequest(new IrcMessageEventArgs<SourceRequestMessage>(this));
    }

  }
}