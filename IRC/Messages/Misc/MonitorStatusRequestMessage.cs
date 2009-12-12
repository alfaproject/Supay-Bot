using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A Monitor system message that requests that status of the users on your monitor list. </summary>
  [Serializable]
  class MonitorStatusRequestMessage : MonitorMessage {

    /// <summary>
    ///   Determines if the message can be parsed by this type. </summary>
    public override bool CanParse(string unparsedMessage) {
      if (!base.CanParse(unparsedMessage)) {
        return false;
      }
      StringCollection param = MessageUtil.GetParameters(unparsedMessage);
      return (param.Count == 1 && MessageUtil.IsIgnoreCaseMatch(param[0], "S"));
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter("S");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(MessageConduit conduit) {
      conduit.OnMonitorStatusRequest(new IrcMessageEventArgs<MonitorStatusRequestMessage>(this));
    }

  }
}