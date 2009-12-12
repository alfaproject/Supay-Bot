namespace Supay.Bot.Irc {

  /// <summary>
  /// The enum of a user's online status.
  /// </summary>
  public enum UserOnlineStatus {
    /// <summary>
    /// The value indicating a user is online and available for chat
    /// </summary>
    Online = 0,

    /// <summary>
    /// The value indicating a user is online but away from his keyboard.
    /// </summary>
    Away = 1,

    /// <summary>
    /// The value indicatin a user is not connected to irc.
    /// </summary>
    Offline = 2
  }

}