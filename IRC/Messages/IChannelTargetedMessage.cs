using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   An interface implemented by messages which are, or can be, within the context of a channel. </summary>
  public interface IChannelTargetedMessage {

    /// <summary>
    ///   Determines if the the current message is targeted at the given channel. </summary>
    bool IsTargetedAtChannel(string channelName);

  }
}