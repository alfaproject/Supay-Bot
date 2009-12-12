using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The KillMessage is used to cause a client-server connection to be closed by the server which has the actual connection. </summary>
  /// <remarks>
  ///   KillMessage is used by servers when they encounter a duplicate entry in the list of valid nicknames and is used to remove both entries. 
  ///   It is also available to operators. </remarks>
  [Serializable]
  class KillMessage : CommandMessage {
    private string nick = string.Empty;
    private string reason = string.Empty;

    /// <summary>
    /// Gets or sets the nick of the user killed.
    /// </summary>
    public virtual string Nick {
      get {
        return this.nick;
      }
      set {
        this.nick = value;
      }
    }

    /// <summary>
    /// Gets or sets the reason for the kill.
    /// </summary>
    public virtual string Reason {
      get {
        return this.reason;
      }
      set {
        this.reason = value;
      }
    }

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "KILL";
      }
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Nick);
      if (this.Reason.Length != 0) {
        writer.AddParameter(this.Reason);
      } else {
        writer.AddParameter("kill");
      }
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count >= 2) {
        this.Nick = parameters[0];
        this.Reason = parameters[1];
      } else {
        this.Nick = string.Empty;
        this.Reason = string.Empty;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnKill(new IrcMessageEventArgs<KillMessage>(this));
    }

  }
}