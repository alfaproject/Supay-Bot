using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Marks the start of the replies to the <see cref="ListMessage"/> query. </summary>
  [Serializable]
  class ListStartReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="ListStartReplyMessage"/> class. </summary>
    public ListStartReplyMessage()
      : base(321) {
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("Channel");
      writer.AddParameter("Users Name");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnListStartReply(new IrcMessageEventArgs<ListStartReplyMessage>(this));
    }

  }
}