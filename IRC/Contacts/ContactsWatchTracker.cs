using System;
using BigSister.Irc.Messages;

namespace BigSister.Irc.Contacts {
  internal class ContactsWatchTracker : ContactsTracker {

    public ContactsWatchTracker(ContactList contacts)
      : base(contacts) {
    }

    public override void Initialize() {
      this.Contacts.Client.Messages.WatchedUserOffline += new EventHandler<BigSister.Irc.Messages.IrcMessageEventArgs<BigSister.Irc.Messages.WatchedUserOfflineMessage>>(client_WatchedUserOffline);
      this.Contacts.Client.Messages.WatchedUserOnline += new EventHandler<BigSister.Irc.Messages.IrcMessageEventArgs<BigSister.Irc.Messages.WatchedUserOnlineMessage>>(client_WatchedUserOnline);
      base.Initialize();
    }

    protected override void AddNicks(System.Collections.Specialized.StringCollection nicks) {
      WatchListEditorMessage addMsg = new WatchListEditorMessage();
      foreach (string nick in nicks) {
        addMsg.AddedNicks.Add(nick);
      }
      this.Contacts.Client.Send(addMsg);
    }

    protected override void AddNick(string nick) {
      WatchListEditorMessage addMsg = new WatchListEditorMessage();
      addMsg.AddedNicks.Add(nick);
      this.Contacts.Client.Send(addMsg);
    }

    protected override void RemoveNick(string nick) {
      WatchListEditorMessage remMsg = new WatchListEditorMessage();
      remMsg.RemovedNicks.Add(nick);
      this.Contacts.Client.Send(remMsg);
    }

    #region Reply Handlers

    void client_WatchedUserOnline(object sender, BigSister.Irc.Messages.IrcMessageEventArgs<BigSister.Irc.Messages.WatchedUserOnlineMessage> e) {
      User knownUser = this.Contacts.Users.Find(e.Message.WatchedUser.Nick);
      if (knownUser != null && knownUser.OnlineStatus == UserOnlineStatus.Offline) {
        knownUser.OnlineStatus = UserOnlineStatus.Online;
      }
    }

    void client_WatchedUserOffline(object sender, BigSister.Irc.Messages.IrcMessageEventArgs<BigSister.Irc.Messages.WatchedUserOfflineMessage> e) {
      User knownUser = this.Contacts.Users.Find(e.Message.WatchedUser.Nick);
      if (knownUser != null) {
        knownUser.OnlineStatus = UserOnlineStatus.Offline;
      }
    }

    #endregion

  }
}