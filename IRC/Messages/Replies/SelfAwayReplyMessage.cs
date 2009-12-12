using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   This message is received from the server when it acknowledges a client's
  ///   <see cref="AwayMessage"/>. </summary>
  [Serializable]
  class SelfAwayMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="SelfAwayMessage"/> class. </summary>
    public SelfAwayMessage()
      : base(306) {
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("You have been marked as being away");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnSelfAway(new IrcMessageEventArgs<SelfAwayMessage>(this));
    }

  }
}