using System;
using System.Collections.ObjectModel;

namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   A collection that stores <see cref='BigSister.Irc.Messages.Modes.UserMode'/> objects. </summary>
  [Serializable()]
  public class UserModeCollection : ObservableCollection<UserMode> {

  }
}