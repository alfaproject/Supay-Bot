using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage sent when a command is sent to a server which didn't recognize it. </summary>
  [Serializable]
  class UnknownCommandMessage : ErrorMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="TooManyLinesMessage"/> class. </summary>
    public UnknownCommandMessage()
      : base(421) {
    }

    /// <summary>
    /// Gets or sets the command which caused the error.
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
      writer.AddParameter("Unknown command");
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Command = string.Empty;
      if (parameters.Count > 1) {
        this.Command = parameters[1];
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnUnknownCommand(new IrcMessageEventArgs<UnknownCommandMessage>(this));
    }

  }
}