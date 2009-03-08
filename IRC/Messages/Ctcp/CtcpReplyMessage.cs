using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The base class for all ctcp reply messages. </summary>
  [Serializable]
  public abstract class CtcpReplyMessage : CtcpMessage {

    /// <summary>
    ///   Gets the irc command used to send the ctcp command to another user. </summary>
    /// <remarks>
    ///   A reply message uses the NOTICE command for transport. </remarks>
    protected override string TransportCommand {
      get {
        return "NOTICE";
      }
    }

  }
}