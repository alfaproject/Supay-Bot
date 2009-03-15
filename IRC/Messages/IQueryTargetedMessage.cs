using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   An interface implemented by messages which are, or can be, within the context of a query with a user. </summary>
  interface IQueryTargetedMessage {

    /// <summary>
    ///   Determines if the the current message is targeted at a query with the given user. </summary>
    bool IsQueryToUser(User user);

  }
}