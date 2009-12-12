using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   Sends a request for user info from the target client. </summary>
  [Serializable]
  class UserInfoRequestMessage : CtcpRequestMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="UserInfoRequestMessage"/> class.
    /// </summary>
    public UserInfoRequestMessage()
      : base() {
      this.InternalCommand = "USERINFO";
    }


    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnUserInfoRequest(new IrcMessageEventArgs<UserInfoRequestMessage>(this));
    }

  }
}