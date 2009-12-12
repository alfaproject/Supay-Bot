using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A Message that participates in the Watch framework. </summary>
  [Serializable]
  abstract class WatchMessage : CommandMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "WATCH";
      }
    }

    /// <summary>
    /// Validates this message against the given server support
    /// </summary>
    public override void Validate(ServerSupport serverSupport) {
      base.Validate(serverSupport);
      if (serverSupport != null && serverSupport.MaxWatches <= 0) {
        throw new InvalidMessageException("Server does not support watch.");
      }

    }

  }
}