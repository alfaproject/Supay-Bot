using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Queries the server to see if it supports the Ircx extension, and sets the client into ircx mode if it does. </summary>
  [Serializable]
  public class IrcxMessage : CommandMessage {

    /// <summary>
    /// Creates a new instance of the IrcxMessage class.
    /// </summary>
    public IrcxMessage()
      : base() {
    }

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "IRCX";
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnIrcx(new IrcMessageEventArgs<IrcxMessage>(this));
    }

  }
}