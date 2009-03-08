using System.Collections.Specialized;

namespace BigSister.Irc.Contacts {
  internal abstract class ContactsTracker {

    public ContactsTracker(ContactList contacts) {
      this.contacts = contacts;
      this.contacts.Users.CollectionChanged += new NotifyCollectionChangedEventHandler(Users_CollectionChanged);
    }

    void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      if (e.Action == NotifyCollectionChangedAction.Add) {
        foreach (User newUser in e.NewItems) {
          this.AddNick(newUser.Nick);
        }
      }
      if (e.Action == NotifyCollectionChangedAction.Remove) {
        foreach (User oldUser in e.OldItems) {
          this.RemoveNick(oldUser.Nick);
        }
      }
    }

    ContactList contacts;

    protected ContactList Contacts {
      get {
        return contacts;
      }
    }

    public virtual void Initialize() {
      StringCollection nicks = new StringCollection();
      foreach (User u in this.Contacts.Users) {
        nicks.Add(u.Nick);
      }
      this.AddNicks(nicks);
    }

    protected abstract void AddNicks(StringCollection nicks);

    protected abstract void AddNick(string nick);

    protected abstract void RemoveNick(string nick);

  }
}