using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Returned when a nickname parameter expected for a command and isn't found. </summary>
  [Serializable]
  public class NoNickGivenMessage : ErrorMessage {

    /// <summary>
    /// Creates a new instances of the <see cref="NoNickGivenMessage"/> class.
    /// </summary>
    public NoNickGivenMessage()
      : base() {
      this.InternalNumeric = 431;
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("No nickname given");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnNoNickGiven(new IrcMessageEventArgs<NoNickGivenMessage>(this));
    }

  }
}