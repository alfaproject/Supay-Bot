using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The base for all message which send a text command. </summary>
  [Serializable]
  abstract class CommandMessage : IrcMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected abstract string Command {
      get;
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Command);
    }

    /// <summary>
    ///   Determines if the message can be parsed by this type. </summary>
    public override bool CanParse(string unparsedMessage) {
      string messageCommand = MessageUtil.GetCommand(unparsedMessage);
      return MessageUtil.IsIgnoreCaseMatch(messageCommand, this.Command);
    }

  }
}