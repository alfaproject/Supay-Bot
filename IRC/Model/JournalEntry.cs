using System;
using System.ComponentModel;

namespace BigSister.Irc {
  /// <summary>
  ///   A single entry in the journal of messages and related information related to an irc channel or query. </summary>
  class JournalEntry : INotifyPropertyChanged {

    private DateTime _time;
    private Object _item;

    /// <summary>
    ///   Creates a new instance of the <see href="JournalEntry" /> class. </summary>
    public JournalEntry() {
    }

    /// <summary>
    ///   Creates a new instance of the <see href="JournalEntry"/> class, populated with the given item. </summary>
    public JournalEntry(Object item) {
      this.Item = item;
    }

    #region Properties

    /// <summary>
    /// The time at which the entry was added to the journal.
    /// </summary>
    public DateTime Time {
      get {
        return _time;
      }
      set {
        _time = value;
        NotifyPropertyChanged("Time");
      }
    }

    /// <summary>
    /// The entry data, usually an IrcMessage, but can be any object.
    /// </summary>
    public Object Item {
      get {
        return _item;
      }
      set {
        _item = value;
        NotifyPropertyChanged("Item");
      }
    }

    #endregion

    #region INotifyPropertyChanged

    /// <summary>
    /// Raised when a property on the instance has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName) {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

  }
}