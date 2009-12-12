using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage received when a UserModeMessage was sent with a UserMode which the server didn't recognize. </summary>
  [Serializable]
  class UnknownUserModeMessage : ErrorMessage {
    //:irc.dkom.at 501 _aLfa_ :Unknown MODE flag

    /// <summary>
    ///   Creates a new instance of the <see cref="UnknownUserModeMessage"/> class. </summary>
    public UnknownUserModeMessage()
      : base(501) {
    }

    /// <exclude />
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("Unknown MODE flag");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnUnknownUserMode(new IrcMessageEventArgs<UnknownUserModeMessage>(this));
    }

  }
}