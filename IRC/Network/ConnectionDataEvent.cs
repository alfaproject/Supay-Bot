using System;

namespace BigSister.Irc.Network {
  /// <summary>
  ///   Provides data for connection events that carry a data payload. </summary>
  public class ConnectionDataEventArgs : EventArgs {

    /// <summary>
    /// Initializes a new instance of the ConnectionDataEventArgs class with the given data.
    /// </summary>
    /// <param name="data">The <see cref="ConnectionDataEventArgs.Data"/> <see cref="string"/> received by the <see cref="ClientConnection"/>.</param>
    public ConnectionDataEventArgs(string data) {
      this.data = data;
    }

    /// <summary>
    /// Gets the <see cref="ConnectionDataEventArgs.Data"/> received by the the <see cref="ClientConnection"/>.
    /// </summary>
    public string Data {
      get {
        return data;
      }
    }

    private string data;

  }
}