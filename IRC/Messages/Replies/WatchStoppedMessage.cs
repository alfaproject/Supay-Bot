using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A Watch system notification that a user has been removed from your watch list. </summary>
  [Serializable]
  class WatchStoppedMessage : WatchedUserChangedMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="WatchStoppedMessage"/>.
    /// </summary>
    public WatchStoppedMessage()
      : base() {
      this.InternalNumeric = 602;
    }

    /// <exclude />
    protected override string ChangeMessage {
      get {
        return "stopped watching";
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnWatchStopped(new IrcMessageEventArgs<WatchStoppedMessage>(this));
    }

  }
}