using System;
using System.Collections.ObjectModel;

namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   A collection that stores <see cref='BigSister.Irc.Messages.Modes.ChannelMode'/> objects. </summary>
  [Serializable()]
  class ChannelModeCollection : ObservableCollection<ChannelMode> {

    /// <summary>
    ///   Clears the current collection and adds the given modes. </summary>
    public void ResetWith(ChannelModeCollection newModes) {
      this.Clear();
      foreach (ChannelMode mode in newModes) {
        this.Add(mode);
      }
    }

  }
}