using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A Watch system notification that a user is offline. </summary>
  [Serializable]
  class WatchedUserIsOfflineMessage : WatchedUserOfflineMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="WatchedUserIsOfflineMessage"/>.
    /// </summary>
    public WatchedUserIsOfflineMessage()
      : base() {
      this.InternalNumeric = 605;
    }

    /// <exclude />
    protected override string ChangeMessage {
      get {
        return "is offline";
      }
    }

  }
}