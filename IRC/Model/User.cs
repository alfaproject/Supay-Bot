using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Supay.Bot.Irc {
  /// <summary>
  ///   Represents a User on an irc server. </summary>
  [Serializable]
  sealed class User : INotifyPropertyChanged {

    private string _nick = string.Empty;
    private string _userName = string.Empty;
    private string _hostName = string.Empty;

    private string _fingerPrint;

    #region CTor

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    public User() {
      this.modes.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e) {
        this.PropChanged("Modes");
      };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class with the given mask string
    /// </summary>
    /// <param name="mask">The mask string to parse.</param>
    public User(string mask)
      : this() {
      Parse(mask);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the nickname of the User
    /// </summary>
    public string Nick {
      get {
        return _nick;
      }
      set {
        if (_nick != value) {
          _nick = value;
          PropChanged("Nick");
        }
      }
    }

    /// <summary>
    /// Gets or sets the supposed real name of the User
    /// </summary>
    public string RealName {
      get {
        return realName;
      }
      set {
        if (realName != value) {
          realName = value;
          PropChanged("RealName");
        }
      }
    }
    private string realName = string.Empty;

    /// <summary>
    /// Gets or sets the Password the User will use on the server
    /// </summary>
    public string Password {
      get {
        return password;
      }
      set {
        if (password != value) {
          password = value;
          PropChanged("Password");
        }
      }
    }
    private string password = string.Empty;

    /// <summary>
    /// Gets or sets the username of the User on her local server.
    /// </summary>
    public string UserName {
      get {
        return _userName;
      }
      set {
        if (_userName != value) {
          _userName = value;
          PropChanged("UserName");
        }
      }
    }

    /// <summary>
    /// Gets or sets the _hostName of the local machine of this User
    /// </summary>
    public string HostName {
      get {
        return _hostName;
      }
      set {
        if (_hostName != value) {
          _hostName = value;
          PropChanged("HostName");
        }
      }
    }

    public string FingerPrint {
      get {
        if (string.IsNullOrEmpty(_hostName) || string.IsNullOrEmpty(_userName))
          return string.Empty;

        if (_fingerPrint == null) {
          int indexofpoint = _hostName.IndexOf('.');
          if (indexofpoint > 0)
            _fingerPrint = _userName.TrimStart('~') + "@*" + _hostName.Substring(indexofpoint);
          else
            _fingerPrint = _userName.TrimStart('~') + "@" + _hostName;
        }
        return _fingerPrint;
      }
    }

    /// <summary>
    /// Gets or sets the online status of this User
    /// </summary>
    public UserOnlineStatus OnlineStatus {
      get {
        return onlineStatus;
      }
      set {
        if (onlineStatus != value) {
          onlineStatus = value;
          PropChanged("OnlineStatus");
        }
      }
    }
    private UserOnlineStatus onlineStatus;

    /// <summary>
    /// Gets or sets the away message of this User
    /// </summary>
    public string AwayMessage {
      get {
        return awayMessage;
      }
      set {
        if (awayMessage != value) {
          awayMessage = value;
          PropChanged("AwayMessage");
        }
      }
    }
    private string awayMessage;

    /// <summary>
    /// Gets or sets the name of the server which the User is connected to.
    /// </summary>
    public string ServerName {
      get {
        return serverName;
      }
      set {
        if (serverName != value) {
          serverName = value;
          PropChanged("ServerName");
        }
      }
    }
    private string serverName;

    /// <summary>
    ///   Gets or sets if the User is an IRC Operator </summary>
    public bool IrcOperator {
      get {
        return ircOperator;
      }
      set {
        if (ircOperator != value) {
          ircOperator = value;
          PropChanged("IrcOperator");
        }
      }
    }
    private bool ircOperator;

    /// <summary>
    /// Gets the modes which apply to the user.
    /// </summary>
    public Supay.Bot.Irc.Messages.Modes.UserModeCollection Modes {
      get {
        return modes;
      }
    }
    private Supay.Bot.Irc.Messages.Modes.UserModeCollection modes = new Supay.Bot.Irc.Messages.Modes.UserModeCollection();

    #endregion

    #region Methods

    /// <summary>
    /// Represents this User's information as an irc mask
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      StringBuilder result = new StringBuilder();
      result.Append(Nick);
      if (!string.IsNullOrEmpty(this.UserName)) {
        result.Append("!");
        result.Append(this.UserName);
      }
      if (!string.IsNullOrEmpty(this.HostName)) {
        result.Append("@");
        result.Append(this.HostName);
      }

      return result.ToString();
    }

    /// <summary>
    /// Represents this User's information with a guarenteed nick!user@host format.
    /// </summary>
    public string ToNickUserHostString() {
      string finalNick = (string.IsNullOrEmpty(this.Nick)) ? "*" : this.Nick;
      string user = (string.IsNullOrEmpty(this.UserName)) ? "*" : this.UserName;
      string host = (string.IsNullOrEmpty(this.HostName)) ? "*" : this.HostName;

      return finalNick + "!" + user + "@" + host;
    }

    /// <summary>
    ///   Determines wether the current user mask matches the given user mask. </summary>
    /// <param name="wildcardMask">
    ///   The wild-card filled mask to compare with the current. </param>
    /// <returns>
    ///   True if this mask is described by the given wildcard Mask. False if not. </returns>
    public bool IsMatch(User wildcardMask) {
      if (wildcardMask == null) {
        return false;
      }

      //Fist we'll return quickly if they are exact matches
      if (this.Nick == wildcardMask.Nick && this.UserName == wildcardMask.UserName && this.HostName == wildcardMask.HostName) {
        return true;
      }

      return (true
        && Regex.IsMatch(this.Nick, makeRegexPattern(wildcardMask.Nick), RegexOptions.IgnoreCase)
        && Regex.IsMatch(this.UserName, makeRegexPattern(wildcardMask.UserName), RegexOptions.IgnoreCase)
        && Regex.IsMatch(this.HostName, makeRegexPattern(wildcardMask.HostName), RegexOptions.IgnoreCase)
        );
    }

    /// <summary>
    ///   Decides if the given user address matches the given address mask. </summary>
    /// <param name="actualMask">
    ///   The user address mask to compare match. </param>
    /// <param name="wildcardMask">
    ///   The address mask containing wildcards to match with. </param>
    /// <returns>
    ///   True if <parmref>actualMask</parmref> is contained within (or described with) the <paramref>wildcardMask</paramref>. False if not. </returns>
    public static bool IsMatch(string actualMask, string wildcardMask) {
      return new User(actualMask).IsMatch(new User(wildcardMask));
    }

    /// <summary>
    ///   Parses the given string as a mask to populate this user object. </summary>
    /// <param name="rawMask">
    ///   The mask to parse. </param>
    public void Parse(string rawMask) {
      this.Reset();

      string mask = rawMask;
      int indexOfBang = mask.IndexOf('!');
      int indexOfAt = mask.LastIndexOf('@');

      if (indexOfAt > 1) {
        this.HostName = mask.Substring(indexOfAt + 1);
        mask = mask.Substring(0, indexOfAt);
      }

      if (indexOfBang != -1) {
        this.UserName = mask.Substring(indexOfBang + 1);
        mask = mask.Substring(0, indexOfBang);
      }

      if (!string.IsNullOrEmpty(mask)) {
        string newNick = mask;
        string firstLetter = newNick.Substring(0, 1);
        if (ChannelStatus.Exists(firstLetter)) {
          newNick = newNick.Substring(1);
        }
        this.Nick = newNick;
      }
    }

    /// <summary>
    /// Resets the User properties to the default values
    /// </summary>
    public void Reset() {
      this.Nick = string.Empty;
      this.UserName = string.Empty;
      this.HostName = string.Empty;
      this.OnlineStatus = UserOnlineStatus.Online;
      this.AwayMessage = string.Empty;
      this.IrcOperator = false;
      this.Modes.Clear();
      this.Password = string.Empty;
      this.RealName = string.Empty;
      this.ServerName = string.Empty;
      this.UserName = string.Empty;

      this.dirtyProperties.Clear();
    }

    /// <summary>
    /// Merges the properties of the given User onto this User.
    /// </summary>
    public void MergeWith(User user) {
      if (user == null) {
        return;
      }
      if (user.IsDirty("OnlineStatus") && !this.IsDirty("OnlineStatus")) {
        this.OnlineStatus = user.OnlineStatus;
      }
      if (user.IsDirty("AwayMessage") && !this.IsDirty("AwayMessage")) {
        this.AwayMessage = user.AwayMessage;
      }
      if (user.IsDirty("HostName") && !this.IsDirty("HostName")) {
        this.HostName = user.HostName;
      }
      if (user.IsDirty("Nick") && !this.IsDirty("Nick")) {
        this.Nick = user.Nick;
      }
      if (user.IsDirty("Password") && !this.IsDirty("Password")) {
        this.Password = user.Password;
      }
      if (user.IsDirty("RealName") && !this.IsDirty("RealName")) {
        this.RealName = user.RealName;
      }
      if (user.IsDirty("UserName") && !this.IsDirty("UserName")) {
        this.UserName = user.UserName;
      }
      if (user.IsDirty("ServerName") && !this.IsDirty("ServerName")) {
        this.ServerName = user.ServerName;
      }
      if (user.IsDirty("IrcOperator") && !this.IsDirty("IrcOperator")) {
        this.IrcOperator = user.IrcOperator;
      }
    }

    /// <summary>
    /// Copies the properties of the given User onto this User.
    /// </summary>
    public void CopyFrom(User user) {
      if (user.IsDirty("OnlineStatus")) {
        this.OnlineStatus = user.OnlineStatus;
      }
      if (user.IsDirty("AwayMessage")) {
        this.AwayMessage = user.AwayMessage;
      }
      if (user.IsDirty("HostName")) {
        this.HostName = user.HostName;
      }
      if (user.IsDirty("Nick")) {
        this.Nick = user.Nick;
      }
      if (user.IsDirty("Password")) {
        this.Password = user.Password;
      }
      if (user.IsDirty("RealName")) {
        this.RealName = user.RealName;
      }
      if (user.IsDirty("UserName")) {
        this.UserName = user.UserName;
      }
      if (user.IsDirty("ServerName")) {
        this.ServerName = user.ServerName;
      }
      if (user.IsDirty("IrcOperator")) {
        this.IrcOperator = user.IrcOperator;
      }

      _fingerPrint = null;
    }

    private static string makeRegexPattern(string wildcardString) {
      return Regex.Escape(wildcardString).Replace(@"\*", @".*").Replace(@"\?", @".");
    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property on the instance has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(PropertyChangedEventArgs e) {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, e);
      }
    }

    #endregion

    private void PropChanged(string propertyName) {
      if (!dirtyProperties.Contains(propertyName)) {
        dirtyProperties.Add(propertyName);
      }
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    private bool IsDirty(string propertyName) {
      return dirtyProperties.Contains(propertyName);
    }

    private List<string> dirtyProperties = new List<string>();

  } //class User
} ////namespace Supay.Bot.Irc