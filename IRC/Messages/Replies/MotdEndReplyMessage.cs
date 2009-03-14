using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Signifies the end of the motd sent by the server. </summary>
  [Serializable]
  public class MotdEndReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="MotdEndReplyMessage"/> class. </summary>
    public MotdEndReplyMessage()
      : base(376) {
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("End of /MOTD command");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnMotdEndReply(new IrcMessageEventArgs<MotdEndReplyMessage>(this));
    }

  }
}