using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage sent when a command is sent which doesn't contain all the required parameters. </summary>
  [Serializable]
  class NotEnoughParametersMessage : ErrorMessage {
    //:irc.dkom.at 443 _aLfa_ KICK :not enough parameters

    /// <summary>
    ///   Creates a new instance of the <see cref="NotEnoughParametersMessage"/> class. </summary>
    public NotEnoughParametersMessage()
      : base(461) {
    }

    /// <summary>
    /// Gets or sets the command sent
    /// </summary>
    public string Command {
      get {
        return command;
      }
      set {
        command = value;
      }
    }
    private string command;

    /// <exclude />
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Command);
      writer.AddParameter("Not enough parameters");
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Command = string.Empty;
      if (parameters.Count > 2) {
        this.Command = parameters[1];
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnNotEnoughParameters(new IrcMessageEventArgs<NotEnoughParametersMessage>(this));
    }

  }
}