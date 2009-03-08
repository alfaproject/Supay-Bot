using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   This message is sent when the client has been elevated to network operator status. </summary>
  [Serializable]
  public class OperReplyMessage : NumericMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="OperReplyMessage"/> class
    /// </summary>
    public OperReplyMessage()
      : base() {
      this.InternalNumeric = 381;
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("You are now an IRC operator");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnOperReply(new IrcMessageEventArgs<OperReplyMessage>(this));
    }

  }
}