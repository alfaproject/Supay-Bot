using System;
using Supay.Bot.Irc.Messages;

namespace Supay.Bot.Irc.Contacts {
  internal class ContactsWatchTracker : ContactsTracker {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public ContactsWatchTracker(ContactList contacts)
      : base(contacts) {
    }

    public override void Initialize() {
      this.Contacts.Client.Messages.WatchedUserOffline += new EventHandler<Supay.Bot.Irc.Messages.IrcMessageEventArgs<Supay.Bot.Irc.Messages.WatchedUserOfflineMessage>>(client_WatchedUserOffline);
      this.Contacts.Client.Messages.WatchedUserOnline += new EventHandler<Supay.Bot.Irc.Messages.IrcMessageEventArgs<Supay.Bot.Irc.Messages.WatchedUserOnlineMessage>>(client_WatchedUserOnline);
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

    void client_WatchedUserOnline(object sender, Supay.Bot.Irc.Messages.IrcMessageEventArgs<Supay.Bot.Irc.Messages.WatchedUserOnlineMessage> e) {
      User knownUser = this.Contacts.Users.Find(e.Message.WatchedUser.Nick);
      if (knownUser != null && knownUser.OnlineStatus == UserOnlineStatus.Offline) {
        knownUser.OnlineStatus = UserOnlineStatus.Online;
      }
    }

    void client_WatchedUserOffline(object sender, Supay.Bot.Irc.Messages.IrcMessageEventArgs<Supay.Bot.Irc.Messages.WatchedUserOfflineMessage> e) {
      User knownUser = this.Contacts.Users.Find(e.Message.WatchedUser.Nick);
      if (knownUser != null) {
        knownUser.OnlineStatus = UserOnlineStatus.Offline;
      }
    }

    #endregion

  }
}