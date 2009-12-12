using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A Watch system notification that a watched user is offline. </summary>
  /// <remarks>
  ///   This message may either be a WatchedUserIsOfflineMessage or a WatchedUserNowOfflineMessage.
  ///   Both messages have the same api and have the same impact for any watch tracking component,
  ///   but are replies sent in reponse to different commands. </remarks>
  [Serializable]
  abstract class WatchedUserOnlineMessage : WatchedUserChangedMessage {
    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnWatchedUserOnline(new IrcMessageEventArgs<WatchedUserOnlineMessage>(this));
    }

  }
}