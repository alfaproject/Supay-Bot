using System;
using System.Collections.ObjectModel;
using BigSister.Irc.Messages;

namespace BigSister.Irc {
  /// <summary>
  ///   A collection that stores <see cref='BigSister.Irc.Messages.IrcMessage'/> objects. </summary>
  [Serializable()]
  public class MessageCollection : ObservableCollection<IrcMessage> {

  }
}