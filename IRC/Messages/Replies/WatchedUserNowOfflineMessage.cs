using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   A Watch system notification that a user is now offline. </summary>
  [Serializable]
  class WatchedUserNowOfflineMessage : WatchedUserOfflineMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="WatchedUserNowOfflineMessage"/>.
    /// </summary>
    public WatchedUserNowOfflineMessage()
      : base() {
      this.InternalNumeric = 600;
    }

    /// <exclude />
    protected override string ChangeMessage {
      get {
        return "logged offline";
      }
    }

  }
}