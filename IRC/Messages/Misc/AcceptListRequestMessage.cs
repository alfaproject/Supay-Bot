using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A CallerId/Accept system message that requests the nicks of the users on your accept list. </summary>
  [Serializable]
  class AcceptListRequestMessage : CommandMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "ACCEPT";
      }
    }

    /// <summary>
    /// Validates this message against the given server support
    /// </summary>
    public override void Validate(ServerSupport serverSupport) {
      base.Validate(serverSupport);
      if (serverSupport != null && !serverSupport.CallerId) {
        throw new InvalidMessageException("Server does not support accept.");
      }

    }

    /// <summary>
    /// Determines if the message can be parsed by this type. </summary>
    public override bool CanParse(string unparsedMessage) {
      if (!base.CanParse(unparsedMessage)) {
        return false;
      }
      string firstParam = MessageUtil.GetParameter(unparsedMessage, 0);
      return (firstParam == "*");
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("*");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(MessageConduit conduit) {
      conduit.OnAcceptListRequest(new IrcMessageEventArgs<AcceptListRequestMessage>(this));
    }

  }
}