using System;
using System.Collections.Specialized;
using Supay.Bot.Irc.Messages;

namespace Supay.Bot.Irc.Contacts {
  internal class ContactsIsOnTracker : ContactsTracker, IDisposable {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public ContactsIsOnTracker(ContactList contacts)
      : base(contacts) {
    }

    public override void Initialize() {
      this.Contacts.Client.Messages.IsOnReply += new EventHandler<Supay.Bot.Irc.Messages.IrcMessageEventArgs<Supay.Bot.Irc.Messages.IsOnReplyMessage>>(client_IsOnReply);
      base.Initialize();
      if (this.timer != null) {
        this.timer.Dispose();
      }
      this.timer = new System.Timers.Timer();
      this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      this.timer.Start();
    }

    void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      if (this.Contacts.Client.Connection.Status == Supay.Bot.Irc.Network.ConnectionStatus.Connected) {
        IsOnMessage ison = new IsOnMessage();
        foreach (string nick in this.trackedNicks) {
          ison.Nicks.Add(nick);
          if (!waitingOnNicks.Contains(nick)) {
            waitingOnNicks.Add(nick);
          }
        }
        this.Contacts.Client.Send(ison);
      }
    }

    protected override void AddNicks(StringCollection nicks) {
      foreach (string nick in nicks) {
        AddNick(nick);
      }
    }

    protected override void AddNick(string nick) {
      if (!trackedNicks.Contains(nick)) {
        trackedNicks.Add(nick);
      }
    }

    protected override void RemoveNick(string nick) {
      if (trackedNicks.Contains(nick)) {
        trackedNicks.Remove(nick);
      }
    }

    private StringCollection trackedNicks = new StringCollection();
    private StringCollection waitingOnNicks = new StringCollection();
    private System.Timers.Timer timer;

    #region Reply Handlers

    void client_IsOnReply(object sender, Supay.Bot.Irc.Messages.IrcMessageEventArgs<Supay.Bot.Irc.Messages.IsOnReplyMessage> e) {
      foreach (string onlineNick in e.Message.Nicks) {
        if (waitingOnNicks.Contains(onlineNick)) {
          waitingOnNicks.Remove(onlineNick);
        }
        User knownUser = this.Contacts.Users.Find(onlineNick);
        if (knownUser != null && knownUser.OnlineStatus == UserOnlineStatus.Offline) {
          knownUser.OnlineStatus = UserOnlineStatus.Online;
        }
        if (knownUser == null && this.trackedNicks.Contains(onlineNick)) {
          this.trackedNicks.Remove(onlineNick);
        }
      }
      foreach (string nick in this.waitingOnNicks) {
        User offlineUser = this.Contacts.Users.Find(nick);
        offlineUser.OnlineStatus = UserOnlineStatus.Offline;
        waitingOnNicks.Remove(nick);
      }
    }

    #endregion


    #region IDisposable Members

    private bool disposed;

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
      if (!this.disposed) {
        if (disposing) {
          this.timer.Dispose();

        }
        disposed = true;
      }
    }

    ~ContactsIsOnTracker() {
      Dispose(false);
    }

    #endregion
  }
}