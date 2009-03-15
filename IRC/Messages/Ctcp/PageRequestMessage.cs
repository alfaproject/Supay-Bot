using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Sends a page request to the target. </summary>
  [Serializable]
  class PageRequestMessage : CtcpRequestMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="PageRequestMessage"/> class.
    /// </summary>
    public PageRequestMessage()
      : base() {
      this.InternalCommand = "PAGE";
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnPageRequest(new IrcMessageEventArgs<PageRequestMessage>(this));
    }

  }
}