using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage received when a user adds too many users to his Accept list. </summary>
  [Serializable]
  public class AcceptListFullMessage : ErrorMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="AcceptListFullMessage"/> class. </summary>
    public AcceptListFullMessage()
      : base(456) {
    }

    /// <exclude />
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      // :irc.server 456 client :Accept list is full

      base.AddParametersToFormat(writer);
      writer.AddParameter("Accept list is full");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnAcceptListFull(new IrcMessageEventArgs<AcceptListFullMessage>(this));
    }

  }
}