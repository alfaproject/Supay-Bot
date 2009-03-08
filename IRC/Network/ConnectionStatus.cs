using System;

namespace BigSister.Irc.Network {

  /// <summary>
  ///   Specifies the status of a <see cref="ClientConnection"/>. </summary>
  public enum ConnectionStatus {
    /// <summary>
    /// The connection is broken and not able to transmit.
    /// </summary>
    Disconnected,
    /// <summary>
    /// The connection is being made.
    /// </summary>
    Connecting,
    /// <summary>
    /// The connection is complete and ready to transmit.
    /// </summary>
    Connected,
  }

}