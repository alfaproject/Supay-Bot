using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Sends a request for the script version of the target's client. </summary>
  [Serializable]
  public class ScriptRequestMessage : CtcpRequestMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="ScriptRequestMessage"/> class. </summary>
    public ScriptRequestMessage()
      : base() {
      this.InternalCommand = "SCRIPT";
    }

    /// <summary>
    ///   Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass. </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnScriptRequest(new IrcMessageEventArgs<ScriptRequestMessage>(this));
    }

  }
}