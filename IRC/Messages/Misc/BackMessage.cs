using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   With the BackMessage, clients can set disable the automatic reply string set by an <see cref="AwayMessage"/>. </summary>
  [Serializable]
  class BackMessage : CommandMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "AWAY";
      }
    }

    /// <summary>
    ///   Determines if the message can be parsed by this type. </summary>
    public override bool CanParse(string unparsedMessage) {
      return (base.CanParse(unparsedMessage) && MessageUtil.GetParameters(unparsedMessage).Count == 0);
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnBack(new IrcMessageEventArgs<BackMessage>(this));
    }

  }
}