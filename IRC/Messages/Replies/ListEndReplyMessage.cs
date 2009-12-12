using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   Marks the end of the replies to the <see cref="ListMessage"/> query. </summary>
  [Serializable]
  class ListEndReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="ListEndReplyMessage"/>. </summary>
    public ListEndReplyMessage()
      : base(323) {
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("End of /LIST");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnListEndReply(new IrcMessageEventArgs<ListEndReplyMessage>(this));
    }

  }
}