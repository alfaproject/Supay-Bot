using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The base class for all ctcp request messages. </summary>
  [Serializable]
  abstract class CtcpRequestMessage : CtcpMessage {

    /// <summary>
    ///   Gets the irc command used to send the ctcp command to another user. </summary>
    /// <remarks>
    ///   A request message uses the PRIVMSG command for transport. </remarks>
    protected override string TransportCommand {
      get {
        return "PRIVMSG";
      }
    }

    /// <summary>
    /// Gets the data payload of the Ctcp request.
    /// </summary>
    protected override string ExtendedData {
      get {
        return string.Empty;
      }
    }

  }
}