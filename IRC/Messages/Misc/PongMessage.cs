using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   PongMessage is a reply to ping message. </summary>
  [Serializable]
  public class PongMessage : CommandMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "PONG";
      }
    }

    /// <summary>
    /// Gets or sets the target of the pong.
    /// </summary>
    public virtual string Target {
      get {
        return this.target;
      }
      set {
        this.target = value;
      }
    }
    private string target = string.Empty;

    /// <summary>
    /// Gets or sets the server that the ping should be forwarded to.
    /// </summary>
    public virtual string ForwardServer {
      get {
        return this.forwardServer;
      }
      set {
        this.forwardServer = value;
      }
    }
    private string forwardServer = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Target);
      if (this.ForwardServer.Length != 0) {
        writer.AddParameter(this.ForwardServer);
      }
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count >= 2) {
        Target = parameters[0];
        ForwardServer = parameters[1];
      } else {
        ForwardServer = string.Empty;
        Target = string.Empty;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnPong(new IrcMessageEventArgs<PongMessage>(this));
    }

  }
}