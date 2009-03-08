using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   A client session is ended with a QuitMessage. </summary>
  /// <remarks>
  ///   The server must close the connection to a client which sends a QuitMessage.
  ///   If a <see cref="QuitMessage.Reason"/> is given, this will be sent instead of the default message, the nickname. </remarks>
  [Serializable]
  public class QuitMessage : CommandMessage {

    /// <summary>
    /// Creates a new instance of the QuitMessage class.
    /// </summary>
    public QuitMessage()
      : base() {
    }

    /// <summary>
    /// Creates a new instance of the QuitMessage class with the given reason.
    /// </summary>
    /// <param name="reason"></param>
    public QuitMessage(string reason)
      : base() {
      this.reason = reason;
    }

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "QUIT";
      }
    }

    /// <summary>
    /// Gets or sets the reason for quiting.
    /// </summary>
    public virtual string Reason {
      get {
        return this.reason;
      }
      set {
        this.reason = value;
      }
    }
    private string reason = string.Empty;


    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      if (this.Reason.Length != 0) {
        writer.AddParameter(this.Reason);
      }
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count >= 1) {
        this.Reason = parameters[0];
      } else {
        this.Reason = string.Empty;
      }
    }


    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnQuit(new IrcMessageEventArgs<QuitMessage>(this));
    }

  }
}