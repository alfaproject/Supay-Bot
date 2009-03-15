using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   This message is received from the server when it acknowledges a client's
  ///   <see cref="BackMessage"/>. </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Un"), Serializable]
  class SelfUnAwayMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="SelfUnAwayMessage"/> class. </summary>
    public SelfUnAwayMessage()
      : base(305) {
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("You are no longer marked as being away");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnSelfUnAway(new IrcMessageEventArgs<SelfUnAwayMessage>(this));
    }

  }
}