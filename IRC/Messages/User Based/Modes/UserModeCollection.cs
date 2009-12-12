using System;
using System.Collections.ObjectModel;

namespace Supay.Bot.Irc.Messages.Modes {
  /// <summary>
  ///   A collection that stores <see cref='UserMode'/> objects. </summary>
  [Serializable()]
  class UserModeCollection : ObservableCollection<UserMode> {

  }
}