using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BigSister.Irc {
  /// <summary>
  ///   The journal of messages and related information related to an irc channel or query. </summary>
  class Journal : ObservableCollection<JournalEntry> {

    /// <summary>
    ///   Creates a new instance of the Journal class. </summary>
    public Journal()
      : base() {
    }

    /// <summary>
    ///   Creates a new instance of the Journal class starting with the given entry list. </summary>
    public Journal(List<JournalEntry> list)
      : base(list) {
    }

    /// <summary>
    ///   Inserts the given entry into the collection at the given index. </summary>
    protected override void InsertItem(int index, JournalEntry item) {
      this.CheckReentrancy();
      this.Items.Insert(index, item);
      if (this.Items.Count > MaxEntries) {
        this.Items.RemoveAt(index != 0 ? 0 : this.Items.Count - 1);
      }
      base.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Count"));
      base.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Item[]"));
      base.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, item, index));
    }

    /// <summary>
    ///   The maximum number of entries kept in the journal at once. </summary>
    public int MaxEntries {
      get {
        return _maxEntries;
      }
      set {
        _maxEntries = value;
        base.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("MaxEntries"));
      }
    }
    private int _maxEntries = 1000;

  }
}