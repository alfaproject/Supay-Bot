using System;
using System.Collections.ObjectModel;

namespace BigSister.Irc {
  /// <summary>
  ///   A collection that stores <see cref='BigSister.Irc.Client'/> objects. </summary>
  [Serializable()]
  public class ClientCollection : ObservableCollection<Client> {

  }
}