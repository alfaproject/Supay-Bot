using System;

namespace BigSister.Irc.Contacts {
  /// <summary>
  ///   A contact list which tracks the online and offline status of the users within the Users collection property. </summary>
  /// <remarks>
  ///   The ContactList will use Watch, Monitor, or IsOn, depending on server support. User status changes 
  ///   will be updated via the User.OnlineStatus property. </remarks>
  class ContactList : IDisposable {

    /// <summary>
    /// Gets the collection of users being tracked as a contact list.
    /// </summary>
    public UserCollection Users {
      get;
      private set;
    }

    /// <summary>
    /// The client on which the list is tracked.
    /// </summary>
    public Client Client {
      get;
      private set;
    }

    /// <summary>
    /// Initializes the ContactList on the given client.
    /// </summary>
    /// <remarks>
    /// This method should not be called until the Client receives the ServerSupport is populated.
    /// An easy way to make sure is to wait until the Ready event of the Client.
    /// </remarks>
    public void Initialize(Client client) {
      if (client == null) {
        throw new ArgumentNullException("client");
      }
      ServerSupport support = client.ServerSupports;
      this.Client = client;
      this.Users = new UserCollection();

      if (support.MaxWatches > 0) {
        this.tracker = new ContactsWatchTracker(this);
      } else if (support.MaxMonitors > 0) {
        this.tracker = new ContactsMonitorTracker(this);
      } else {
        this.tracker = new ContactsIsOnTracker(this);
      }

      this.tracker.Initialize();
    }

    private ContactsTracker tracker;


    #region IDisposable Members

    private bool disposed = false;

    /// <summary>
    /// Implements IDisposable.Dispose
    /// </summary>
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Implements IDisposable.Dispose </summary>
    private void Dispose(bool disposing) {
      if (!this.disposed) {
        if (disposing) {
          IDisposable disposableTracker = this.tracker as IDisposable;
          if (disposableTracker != null) {
            disposableTracker.Dispose();
          }

        }
        disposed = true;
      }
    }

    /// <exclude />
    ~ContactList() {
      Dispose(false);
    }

    #endregion

  }
}