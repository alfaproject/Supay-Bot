using System;
using System.Collections.ObjectModel;
using BigSister.Irc.Messages;

namespace BigSister.Irc {
  /// <summary>
  ///   A collection that stores <see cref='BigSister.Irc.Channel'/> objects. </summary>
  [Serializable()]
  class ChannelCollection : ObservableCollection<Channel> {

    /// <summary>
    ///   Finds the <see href="Channel" /> in the collection with the given name. </summary>
    /// <returns>
    ///   The so-named channel, or null. </returns>
    public Channel Find(string channelName) {
      foreach (Channel channel in this)
        if (MessageUtil.IsIgnoreCaseMatch(channel.Name, channelName))
          return channel;
      return null;
    }

    /// <summary>
    ///   Either finds or creates the channel by the given name. </summary>
    public Channel EnsureChannel(string name, Client client) {
      Channel c = this.Find(name);
      if (c == null || c.Client != client) {
        c = new Channel(client, name);
        this.Add(c);
      }
      return c;
    }

  }
}