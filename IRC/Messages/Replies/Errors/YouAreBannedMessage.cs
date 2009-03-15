using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Returned after an attempt to connect and register yourself with a server which has been setup to explicitly deny connections to you. </summary>
  [Serializable]
  class YouAreBannedMessage : ErrorMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="YouAreBannedMessage"/> class. </summary>
    public YouAreBannedMessage()
      : base(465) {
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("You are banned from this server");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnYouAreBanned(new IrcMessageEventArgs<YouAreBannedMessage>(this));
    }

  }
}