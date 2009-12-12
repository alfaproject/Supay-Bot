using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Supay.Bot.Irc.Messages.Modes;
using Supay.Bot.Irc.Network;

namespace Supay.Bot.Irc {
  /// <summary>
  ///   Represents a single irc channel, with it's users. </summary>
  class Channel : INotifyPropertyChanged {

    private Client _client;
    private UserCollection _users = new UserCollection();
    private ChannelModeCollection _modes = new ChannelModeCollection();
    private Journal _journal = new Journal();
    private UserStatusMap _userModes = new UserStatusMap();
    private NameValueCollection _properties = new NameValueCollection();
    private bool _open;
    private User _topicSetter;
    private DateTime _topicSetTime;

    #region ctor

    /// <summary>
    ///   Creates a new instance of the <see cref="Channel"/> class on the given client. </summary>
    public Channel(Client client) {
      _client = client;
      _users.CollectionChanged += new NotifyCollectionChangedEventHandler(users_CollectionChanged);
      _modes.CollectionChanged += new NotifyCollectionChangedEventHandler(modes_CollectionChanged);
      _journal.CollectionChanged += new NotifyCollectionChangedEventHandler(journal_CollectionChanged);
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="Channel"/> class on the given client with the given name. </summary>
    public Channel(Client client, string name)
      : this(client) {
      this.Name = name;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the collection of general properties assigned to this channel
    /// </summary>
    public NameValueCollection Properties {
      get {
        return _properties;
      }
    }

    /// <summary>
    /// Gets the client which the channel is on.
    /// </summary>
    public Client Client {
      get {
        return _client;
      }
    }


    /// <summary>
    ///   Gets or sets whether the channel is currently open. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public bool Open {
      get {
        return _open && (_client != null && _client.Connection.Status == ConnectionStatus.Connected);
      }
      internal set {
        if (_open != value) {
          _open = value;
          OnPropertyChanged(new PropertyChangedEventArgs("Open"));
        }
      }
    }

    /// <summary>
    ///   Gets or sets the name of the channel. </summary>
    public string Name {
      get {
        return _properties["NAME"] ?? string.Empty;
      }
      set {
        if (this.Name != value) {
          _properties["NAME"] = value;
          this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
        }
      }
    }

    /// <summary>
    /// Gets or sets the topic of the channel
    /// </summary>
    public string Topic {
      get {
        return _properties["TOPIC"] ?? string.Empty;
      }
      set {
        if (this.Topic != value) {
          _properties["TOPIC"] = value;
          OnPropertyChanged(new PropertyChangedEventArgs("Topic"));
        }
      }
    }

    /// <summary>
    ///   Gets or sets the user which set the current topic. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public User TopicSetter {
      get {
        return _topicSetter;
      }
      set {
        if (_topicSetter != value) {
          _topicSetter = value;
          OnPropertyChanged(new PropertyChangedEventArgs("TopicSetter"));
        }
      }
    }

    /// <summary>
    ///   Gets or sets the time which topic was set. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public DateTime TopicSetTime {
      get {
        return _topicSetTime;
      }
      set {
        if (_topicSetTime != value) {
          _topicSetTime = value;
          OnPropertyChanged(new PropertyChangedEventArgs("TopicSetTime"));
        }
      }
    }

    /// <summary>
    /// Gets the users in the channel.
    /// </summary>
    public UserCollection Users {
      get {
        return _users;
      }
    }

    /// <summary>
    /// Gets the modes in the channel.
    /// </summary>
    public ChannelModeCollection Modes {
      get {
        return _modes;
      }
    }

    /// <summary>
    /// Gets the journal of messages on the channel
    /// </summary>
    public Journal Journal {
      get {
        return _journal;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Gets the status for the given <see cref="T:User"/> in the channel. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public ChannelStatus GetStatusForUser(User channelUser) {
      VerifyUserInChannel(channelUser);
      if (_userModes.ContainsKey(channelUser))
        return _userModes[channelUser];
      return ChannelStatus.None;
    }

    /// <summary>
    /// Applies the given <see cref="T:ChannelStatus"/> to the given <see cref="T:User"/> in the channel.
    /// </summary>
    public void SetStatusForUser(User channelUser, ChannelStatus status) {
      if (status == ChannelStatus.None && _userModes.ContainsKey(channelUser)) {
        _userModes.Remove(channelUser);
      } else {
        VerifyUserInChannel(channelUser);
        _userModes[channelUser] = status;
      }
    }

    private void VerifyUserInChannel(User channelUser) {
      if (channelUser == null)
        throw new ArgumentNullException("channelUser");
      if (!Users.Contains(channelUser))
        throw new ArgumentException("User '{0}' is not in channel '{1}'.".FormatWith(channelUser.Nick, this.Name), "channelUser");
    }

    #endregion

    #region Private

    private void users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      if (e.Action == NotifyCollectionChangedAction.Remove)
        _userModes.RemoveAll(keyValue => !_users.Contains(keyValue.Key));
      OnPropertyChanged(new PropertyChangedEventArgs("Users"));
    }

    private void modes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      OnPropertyChanged(new PropertyChangedEventArgs("Modes"));
    }

    private void journal_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      OnPropertyChanged(new PropertyChangedEventArgs("Journal"));
    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    ///   The event raised when a property on the object changes. </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///   Raises the PropertyChanged event. </summary>
    protected void OnPropertyChanged(PropertyChangedEventArgs e) {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, e);
    }

    #endregion

    #region UserStatusMap

    private class UserStatusMap : Dictionary<User, ChannelStatus> {
      /// <summary>
      /// Removes all of the items from the dictionary which the given predictate matches.
      /// </summary>
      /// <returns>The number of items removed from the dictionary.</returns>
      public int RemoveAll(Predicate<KeyValuePair<User, ChannelStatus>> match) {
        if (match == null)
          throw new ArgumentNullException("match");

        int countOfItemsRemoved = 0;

        User[] users = new User[this.Keys.Count];
        this.Keys.CopyTo(users, 0);
        foreach (User u in users) {
          if (this.ContainsKey(u) && match(new KeyValuePair<User, ChannelStatus>(u, this[u]))) {
            this.Remove(u);
            countOfItemsRemoved++;
          }
        }

        return countOfItemsRemoved;
      }
    }

    #endregion

  }
}