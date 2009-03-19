using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;

namespace BigSister.Irc {
  /// <summary>
  /// Contains information about what irc extensions and such the server supports.
  /// </summary>
  /// <remarks>
  /// This information is sent from a <see cref="Client"/> when it receives a <see cref="BigSister.Irc.Messages.SupportMessage"/>.
  /// This most likely makes it unneccesary to catch this message's received event.
  /// </remarks>
  [Serializable]
  class ServerSupport {

    /// <summary>
    /// The extended parameters which the server can support on a List message.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"), Flags]
    public enum ExtendedListParameters {
      /// <summary>
      /// No extended parameters are supported
      /// </summary>
      None = 0,
      /// <summary>
      /// Searching by matching only the given mask is supported
      /// </summary>
      Mask = 1,
      /// <summary>
      /// Searching by not matching the given mask is supported
      /// </summary>
      NotMask = 2,
      /// <summary>
      /// Searching by number of users in the channel is supported
      /// </summary>
      UserCount = 4,
      /// <summary>
      /// Searching by the channel creation time is supported
      /// </summary>
      CreationTime = 8,
      /// <summary>
      /// Searching by the most recent change in a channel's topic is supported
      /// </summary>
      Topic = 16
    }

    #region Default Support

    /// <summary>
    /// 
    /// </summary>
    public static ServerSupport DefaultSupport {
      get {
        if (_defaultSupport == null) {
          _defaultSupport = new ServerSupport();
          //TODO Create A Good Default ServerSupport
        }
        return _defaultSupport;
      }
      set {
        _defaultSupport = value;
      }
    }
    private static ServerSupport _defaultSupport;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets if the server supports the Deaf user mode. </summary>
    public bool DeafMode {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the standard used by the server.
    /// </summary>
    public string Standard {
      get {
        return (this._standard);
      }
      set {
        this._standard = value;
      }
    }
    private string _standard = "i-d";

    /// <summary>
    /// Gets or sets a list of channel modes a person can get and the respective prefix a channel or nickname will get in case the person has it.
    /// </summary>
    /// <remarks>
    /// The order of the modes goes from most powerful to least powerful. 
    /// Those prefixes are shown in the output of the WHOIS, WHO and NAMES command.
    /// </remarks>
    public string ChannelStatuses {
      get {
        return (this._channelStatuses);
      }
      set {
        this._channelStatuses = value;
      }
    }
    private string _channelStatuses = "(ov)@+";

    /// <summary>
    /// Gets or sets the channel status prefixes supported for matched-status-only messages
    /// </summary>
    public string StatusMessages {
      get {
        return (this.statusMessages);
      }
      set {
        this.statusMessages = value;
      }
    }
    private string statusMessages = string.Empty;

    /// <summary>
    /// Gets the supported channel prefixes.
    /// </summary>
    public StringCollection ChannelTypes {
      get {
        return (this._channelTypes);
      }
    }
    private StringCollection _channelTypes = new StringCollection();

    /// <summary>
    /// Gets the modes that require parameters
    /// </summary>
    public StringCollection ModesWithParameters {
      get {
        return (this._modesWithParameters);
      }
    }
    private StringCollection _modesWithParameters = new StringCollection();

    /// <summary>
    /// Gets the modes that require parameters only when set.
    /// </summary>
    public StringCollection ModesWithParametersWhenSet {
      get {
        return (this._modesWithParametersWhenSet);
      }
    }
    private StringCollection _modesWithParametersWhenSet = new StringCollection();

    /// <summary>
    /// Gets the modes that do not require parameters.
    /// </summary>
    public StringCollection ModesWithoutParameters {
      get {
        return (this._modesWithoutParameters);
      }
    }
    private StringCollection _modesWithoutParameters = new StringCollection();

    /// <summary>
    /// Maximum number of channel modes with parameter allowed per <see cref="BigSister.Irc.Messages.ChannelModeMessage"/> command.
    /// </summary>
    public int MaxModes {
      get {
        return (this._maxModes);
      }
      set {
        this._maxModes = value;
      }
    }
    private int _maxModes = 3;

    /// <summary>
    /// Gets or sets the maximum number of channels a client can join.
    /// </summary>
    /// <remarks>
    /// This property is considered obsolete, as most servers use the ChannelLimits property instead.
    /// </remarks>
    public int MaxChannels {
      get {
        return (this._maxChannels);
      }
      set {
        this._maxChannels = value;
      }
    }
    private int _maxChannels = -1;

    /// <summary>
    /// Gets the collection of channel limits, grouped by channel type (ex, #, +).
    /// </summary>
    /// <remarks>This property has replaced MaxChannels becuase of the added flexibility.</remarks>
    public Dictionary<string, int> ChannelLimits {
      get {
        return _channelLimits;
      }
    }
    private Dictionary<string, int> _channelLimits = new Dictionary<string, int>();

    /// <summary>
    /// Gets or sets the maximum nickname length.
    /// </summary>
    public int MaxNickLength {
      get {
        return (this._maxNickLength);
      }
      set {
        this._maxNickLength = value;
      }
    }
    private int _maxNickLength = 9;

    /// <summary>
    /// Gets or sets the maximum channel topic length.
    /// </summary>
    public int MaxTopicLength {
      get {
        return (this._maxTopicLength);
      }
      set {
        this._maxTopicLength = value;
      }
    }
    private int _maxTopicLength = -1;

    /// <summary>
    /// Gets or sets the maximum length of the reason in a <see cref="BigSister.Irc.Messages.KickMessage"/>.
    /// </summary>
    public int MaxKickCommentLength {
      get {
        return (this._maxKickCommentLength);
      }
      set {
        this._maxKickCommentLength = value;
      }
    }
    private int _maxKickCommentLength = -1;

    /// <summary>
    /// Gets or sets the maximum length of a channel name.
    /// </summary>
    public int MaxChannelNameLength {
      get {
        return (this._maxChannelNameLength);
      }
      set {
        this._maxChannelNameLength = value;
      }
    }
    private int _maxChannelNameLength = 200;

    /// <summary>
    /// Gets or sets the maximum number of bans that a channel can have.
    /// </summary>
    public int MaxBans {
      get {
        return (this._maxBans);
      }
      set {
        this._maxBans = value;
      }
    }
    private int _maxBans = -1;

    /// <summary>
    /// Gets or sets the Maximum number of invitation exceptions a channel can have.
    /// </summary>
    public int MaxInvitationExceptions {
      get {
        return (this.maxInvitationsExceptions);
      }
      set {
        this.maxInvitationsExceptions = value;
      }
    }
    private int maxInvitationsExceptions = -1;

    /// <summary>
    /// Gets or sets the maximum number of ban exceptions that a channel can have.
    /// </summary>
    public int MaxBanExceptions {
      get {
        return (this.maxBanExceptions);
      }
      set {
        this.maxBanExceptions = value;
      }
    }
    private int maxBanExceptions = -1;

    /// <summary>
    /// Gets or sets the name of the network which the server is on.
    /// </summary>
    public string NetworkName {
      get {
        return (this._networkName);
      }
      set {
        this._networkName = value;
      }
    }
    private string _networkName = string.Empty;

    /// <summary>
    ///   Gets or sets if the server supports channel ban exceptions. </summary>
    public bool BanExceptions {
      get {
        return (this._banExceptions);
      }
      set {
        this._banExceptions = value;
      }
    }
    private bool _banExceptions = false;

    /// <summary>
    ///   Gets or sets if the server supports channel invitation exceptions.</summary>
    public bool InvitationExceptions {
      get {
        return (this._invitationExceptions);
      }
      set {
        this._invitationExceptions = value;
      }
    }
    private bool _invitationExceptions = false;

    /// <summary>
    /// Gets or sets the maximum number of silence ( serverside ignore ) listings a client can store.
    /// </summary>
    public int MaxSilences {
      get {
        return (this._maxSilences);
      }
      set {
        this._maxSilences = value;
      }
    }
    private int _maxSilences = 0;

    /// <summary>
    ///   Gets or sets if the server supports messages to channel operators. </summary>
    /// <remarks>
    ///   To send a message to channel operators, use a <see cref="BigSister.Irc.Messages.NoticeMessage"/>
    ///   with a target in the format "@#channel". </remarks>
    public bool MessagesToOperators {
      get {
        return (this._messagesToOperators);
      }
      set {
        this._messagesToOperators = value;
      }
    }
    private bool _messagesToOperators = false;

    /// <summary>
    /// Gets or sets the case mapping supported by the server.
    /// </summary>
    /// <remarks>"ascii", "rfc1459", and "strict-rfc1459" are the only known values.</remarks>
    public string CaseMapping {
      get {
        return (this._caseMapping);
      }
      set {
        this._caseMapping = value;
      }
    }
    private string _caseMapping = "rfc1459";

    /// <summary>
    /// Gets or sets the text encoding used by the server.
    /// </summary>
    public string CharacterSet {
      get {
        return (this._characterSet);
      }
      set {
        this._characterSet = value;
      }
    }
    private string _characterSet = string.Empty;

    /// <summary>
    ///   Gets or sets if the server supports the standards declared in rfc 2812. </summary>
    public bool Rfc2812 {
      get {
        return (this._rfc2812);
      }
      set {
        this._rfc2812 = value;
      }
    }
    private bool _rfc2812 = false;

    /// <summary>
    /// Gets or sets the length of channel ids.
    /// </summary>
    public int ChannelIdLength {
      get {
        return (this._channelIdLength);
      }
      set {
        this._channelIdLength = value;
      }
    }
    private int _channelIdLength = -1;

    /// <summary>
    ///   Gets or sets if the server has a message penalty. </summary>
    public bool Penalties {
      get {
        return (this._penalties);
      }
      set {
        this._penalties = value;
      }
    }
    private bool _penalties = false;

    /// <summary>
    ///   Gets or sets if the server will change your nick automatticly when it needs to. </summary>
    public bool ForcedNickChanges {
      get {
        return (this._forcedNickChanges);
      }
      set {
        this._forcedNickChanges = value;
      }
    }
    private bool _forcedNickChanges = false;

    /// <summary>
    ///   Gets or sets if the server supports the USERIP command. </summary>
    public bool UserIP {
      get {
        return (this._userIp);
      }
      set {
        this._userIp = value;
      }
    }
    private bool _userIp = false;

    /// <summary>
    ///   Gets or sets if the server supports the CPRIVMSG command. </summary>
    public bool ChannelMessages {
      get {
        return (this._channelMessages);
      }
      set {
        this._channelMessages = value;
      }
    }
    private bool _channelMessages = false;

    /// <summary>
    ///   Gets or sets if the server supports the CNOTICE command. </summary>
    public bool ChannelNotices {
      get {
        return (this._channelNotices);
      }
      set {
        this._channelNotices = value;
      }
    }
    private bool _channelNotices = false;

    /// <summary>
    /// Gets or sets the maximum number of targets allowed on targetted messages, grouped by message command
    /// </summary>
    public Dictionary<string, int> MaxMessageTargets {
      get {
        return _maxMessageTargets;
      }
    }
    private Dictionary<string, int> _maxMessageTargets = new Dictionary<string, int>();

    /// <summary>
    ///   Gets or sets if the server supports the <see cref="BigSister.Irc.Messages.KnockMessage"/>. </summary>
    public bool Knock {
      get {
        return (this._knock);
      }
      set {
        this._knock = value;
      }
    }
    private bool _knock = false;

    /// <summary>
    ///   Gets or sets if the server supports virtual channels. </summary>
    public bool VirtualChannels {
      get {
        return (this._virtualChannels);
      }
      set {
        this._virtualChannels = value;
      }
    }
    private bool _virtualChannels = false;

    /// <summary>
    ///   Gets or sets if the <see cref="BigSister.Irc.Messages.ListReplyMessage"/> is sent in multiple itterations. </summary>
    public bool SafeList {
      get {
        return (this._safeList);
      }
      set {
        this._safeList = value;
      }
    }
    private bool _safeList = false;

    /// <summary>
    /// Gets or sets the extended parameters the server supports for a <see cref="T:BigSister.Irc.Messages.ListMessage"/>.
    /// </summary>
    public ExtendedListParameters ExtendedList {
      get {
        return _eList;
      }
      set {
        _eList = value;
      }
    }
    private ExtendedListParameters _eList;

    /// <summary>
    /// Gets or sets the maximum number of watches a user is allowed to set.
    /// </summary>
    public int MaxWatches {
      get {
        return (this._maxWatches);
      }
      set {
        this._maxWatches = value;
      }
    }
    private int _maxWatches = -1;

    /// <summary>
    ///   Gets or sets if the <see cref="BigSister.Irc.Messages.WhoMessage"/> uses the WHOX protocol. </summary>
    public bool WhoX {
      get {
        return (this._whoX);
      }
      set {
        this._whoX = value;
      }
    }
    private bool _whoX = false;

    /// <summary>
    ///   Gets or sets if the server suports callerid-style ignore. </summary>
    public bool CallerId {
      get {
        return (this._callerId);
      }
      set {
        this._callerId = value;
      }
    }
    private bool _callerId = false;

    /// <summary>
    ///   Gets or sets if the server supports ETrace. </summary>
    public bool ETrace {
      get {
        return (this.eTrace);
      }
      set {
        this.eTrace = value;
      }
    }
    private bool eTrace = false;

    /// <summary>
    /// Gets or sets the maximum number of user monitors a user is allowed to set.
    /// </summary>
    /// <remarks>
    /// A value of 0 indicates that the server doesn't support the monitor system.
    /// A value of -1 indicates that the server has no limit to the users on the monitor system list.
    /// A value greater than 0 indicates the maximum number of users which can be added to the monitor system list.
    /// </remarks>
    public int MaxMonitors {
      get {
        return _maxMonitors;
      }
      set {
        _maxMonitors = value;
      }
    }
    private int _maxMonitors = 0;

    /// <summary>
    /// Gets the collection of safe channel prefix lengths, grouped by the channel type they apply to.
    /// </summary>
    public Dictionary<string, int> SafeChannelPrefixLengths {
      get {
        return _safeChannelPrefixLengths;
      }
    }
    private Dictionary<string, int> _safeChannelPrefixLengths = new Dictionary<string, int>();

    /// <summary>
    /// Gets or sets the maximum length of away messages.
    /// </summary>
    public int MaxAwayMessageLength {
      get {
        return _maxAwayMessageLength;
      }
      set {
        _maxAwayMessageLength = value;
      }
    }
    private int _maxAwayMessageLength = -1;

    #endregion

    /// <summary>
    /// Gets the list of unknown items supported by the server.
    /// </summary>
    public virtual NameValueCollection UnknownItems {
      get {
        return unknownItems;
      }
    }
    private NameValueCollection unknownItems = new NameValueCollection();

    /// <summary>
    ///   Loads support information from the given <see cref="BigSister.Irc.Messages.SupportMessage"/>. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    public void LoadInfo(BigSister.Irc.Messages.SupportMessage msg) {
      NameValueCollection items = msg.SupportedItems;
      foreach (string key in items.Keys) {
        string value = items[key] ?? string.Empty;
        switch (key) {
          case "DEAF":
            this.DeafMode = true;
            break;
          case "AWAYLEN":
            SetIfNumeric(this.GetType().GetProperty("MaxAwayMessageLength"), value);
            break;
          case "IDCHAN":
            foreach (InfoPair pair in CreateInfoPairs(value)) {
              int prefixLength = -1;
              if (int.TryParse(pair.Value, out prefixLength)) {
                this.SafeChannelPrefixLengths.Add(pair.Key, prefixLength);
              }
            }
            break;
          case "STD":
            this.Standard = value;
            break;
          case "PREFIX":
            this.ChannelStatuses = value;
            break;
          case "STATUSMSG":
          case "WALLVOICES":
            this.StatusMessages = value;
            break;
          case "CHANTYPES":
            AddChars(this.ChannelTypes, value);
            break;
          case "CHANMODES":
            string[] modeGroups = value.Split(',');
            if (modeGroups.Length >= 4) {
              AddChars(this.ModesWithParameters, modeGroups[0]);
              AddChars(this.ModesWithParameters, modeGroups[1]);
              AddChars(this.ModesWithParametersWhenSet, modeGroups[2]);
              AddChars(this.ModesWithoutParameters, modeGroups[3]);
            } else {
              Trace.WriteLine("Unknown CHANMODES " + value);
            }
            break;
          case "MODES":
            SetIfNumeric(this.GetType().GetProperty("MaxModes"), value);
            break;
          case "MAXCHANNELS":
            SetIfNumeric(this.GetType().GetProperty("MaxChannels"), value);
            break;
          case "CHANLIMIT":
            foreach (InfoPair chanLimitInfo in CreateInfoPairs(value)) {
              int limit = -1;
              if (int.TryParse(chanLimitInfo.Value, out limit)) {
                foreach (char c in chanLimitInfo.Key) {
                  this.ChannelLimits.Add(c.ToStringI(), limit);
                }
              }
            }
            break;
          case "NICKLEN":
          case "MAXNICKLEN":
            this.SetIfNumeric(this.GetType().GetProperty("MaxNickLength"), value);
            break;
          case "TOPICLEN":
            this.SetIfNumeric(this.GetType().GetProperty("MaxTopicLength"), value);
            break;
          case "KICKLEN":
            this.SetIfNumeric(this.GetType().GetProperty("MaxKickCommentLength"), value);
            break;
          case "CHANNELLEN":
          case "MAXCHANNELLEN":
            this.SetIfNumeric(this.GetType().GetProperty("MaxChannelNameLength"), value);
            break;
          case "MAXBANS":
            this.SetIfNumeric(this.GetType().GetProperty("MaxBans"), value);
            break;
          case "NETWORK":
            NetworkName = value;
            break;
          case "EXCEPTS":
            this.BanExceptions = true;
            break;
          case "INVEX":
            this.InvitationExceptions = true;
            break;
          case "SILENCE":
            this.SetIfNumeric(this.GetType().GetProperty("MaxSilences"), value);
            break;
          case "WALLCHOPS":
            this.MessagesToOperators = true;
            break;
          case "CASEMAPPING":
            this.CaseMapping = value;
            break;
          case "CHARSET":
            this.CharacterSet = value;
            break;
          case "RFC2812":
            this.Rfc2812 = true;
            break;
          case "CHIDLEN":
            this.SetIfNumeric(this.GetType().GetProperty("ChannelIdLength"), value);
            break;
          case "PENALTY":
            this.Penalties = true;
            break;
          case "FNC":
            this.ForcedNickChanges = true;
            break;
          case "USERIP":
            this.UserIP = true;
            break;
          case "CPRIVMSG":
            this.ChannelMessages = true;
            break;
          case "CNOTICE":
            this.ChannelNotices = true;
            break;
          case "MAXTARGETS":
            int maxTargets = -1;
            if (value != null && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out maxTargets)) {
              this.MaxMessageTargets.Add(string.Empty, maxTargets);
            }
            break;
          case "TARGMAX":
            foreach (InfoPair targmaxInfo in CreateInfoPairs(value)) {
              int targmax = -1;
              if (int.TryParse(targmaxInfo.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out targmax)) {
                this.MaxMessageTargets.Add(targmaxInfo.Key, targmax);
              } else {
                this.MaxMessageTargets.Add(targmaxInfo.Key, -1);
              }
            }
            break;
          case "KNOCK":
            this.Knock = true;
            break;
          case "VCHANS":
            this.VirtualChannels = true;
            break;
          case "SAFELIST":
            this.SafeList = true;
            break;
          case "ELIST":
            Dictionary<char, ExtendedListParameters> elistMap = new Dictionary<char, ExtendedListParameters>();
            elistMap.Add('M', ExtendedListParameters.Mask);
            elistMap.Add('N', ExtendedListParameters.NotMask);
            elistMap.Add('U', ExtendedListParameters.UserCount);
            elistMap.Add('C', ExtendedListParameters.CreationTime);
            elistMap.Add('T', ExtendedListParameters.Topic);

            this.ExtendedList = ExtendedListParameters.None;
            foreach (char c in value.ToUpperInvariant()) {
              this.ExtendedList = (this.ExtendedList | elistMap[c]);
            }

            break;
          case "WATCH":
            this.SetIfNumeric(this.GetType().GetProperty("MaxWatches"), value);
            break;
          case "MONITOR":
            this.MaxMonitors = -1;
            this.SetIfNumeric(this.GetType().GetProperty("MaxMonitors"), value);
            break;
          case "WHOX":
            this.WhoX = true;
            break;
          case "CALLERID":
          case "ACCEPT":
            this.CallerId = true;
            break;
          case "ETRACE":
            this.ETrace = true;
            break;
          case "MAXLIST":
            foreach (InfoPair maxListInfoPair in CreateInfoPairs(value)) {
              int maxLength = -1;
              if (int.TryParse(maxListInfoPair.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out maxLength)) {
                if (maxListInfoPair.Key.IndexOf('b') != -1) {
                  this.MaxBans = maxLength;
                }
                if (maxListInfoPair.Key.IndexOf('e') != -1) {
                  this.MaxBanExceptions = maxLength;
                }
                if (maxListInfoPair.Key.IndexOf('I') != -1) {
                  this.MaxInvitationExceptions = maxLength;
                }
              }
            }
            break;
          default:
            this.UnknownItems[key] = value;
            Trace.WriteLine("Unknown ServerSupport key/value " + key + " " + value);
            break;
        }
      }
    }

    private static void AddChars(StringCollection target, string source) {
      foreach (char c in source) {
        target.Add(c.ToStringI());
      }
    }

    private void SetIfNumeric(System.Reflection.PropertyInfo property, string value) {
      int intValue;
      if (value != null && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue)) {
        property.SetValue(this, intValue, null);
      } else {
        Trace.WriteLine("Expected numeric for ServerSupport target " + property.Name + " but it was '" + (value ?? string.Empty) + "'");
      }
    }

    private List<InfoPair> CreateInfoPairs(string value) {
      List<InfoPair> list = new List<InfoPair>();
      foreach (string chanLimitPair in value.Split(',')) {
        if (chanLimitPair.Contains(":")) {
          string[] chanLimitInfo = chanLimitPair.Split(':');
          if (chanLimitInfo.Length == 2 && chanLimitInfo[0].Length > 0) {
            InfoPair pair = new InfoPair(chanLimitInfo[0], chanLimitInfo[1]);
            list.Add(pair);
          }
        }
      }
      return list;
    }

    private struct InfoPair {
      public InfoPair(string key, string value) {
        this.Key = key;
        this.Value = value;
      }

      public string Key;
      public string Value;
    }

  }
}