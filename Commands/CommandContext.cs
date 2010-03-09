using System.Data.SQLite;
using System.Linq;
using Supay.Irc;
using Supay.Irc.Messages;

namespace Supay.Bot {
  class CommandContext {

    private Client _irc;
    private UserCollection _users;

    private Channel _channel;
    private string _message;
    private string[] _messageTokens;
    private bool _replyNotice;

    public CommandContext(Client irc, UserCollection users, User from, Channel channel, string message) {
      _irc = irc;
      _users = users;

      this.From = from;
      _channel = channel;

      if (message[0] == '!' || message[0] == '.') {
        _replyNotice = true;
      } else {
        _replyNotice = false;
      }

      if (message[0] == '!' || message[0] == '.' || message[0] == '@') {
        this.Message = message.Substring(1);
      } else {
        this.Message = message;
      }
    }

    public UserCollection Users {
      get {
        return _users;
      }
    }

    public string Message {
      get {
        return _message;
      }
      set {
        if (_message != value) {
          _message = value;
          _messageTokens = _message.Split(' ');
        }
      }
    }

    public string[] MessageTokens {
      get {
        return _messageTokens;
      }
    }

    public User From {
      get;
      private set;
    }

    public bool IsAdmin {
      get {
        if (From.Nickname.EqualsI("_aLfa_") || From.Nickname.EqualsI("_aLfa_|laptop") || From.Nickname.EqualsI("P_Gertrude")) {
          return true;
        }
        return Properties.Settings.Default.Administrators.Split(';').Any(admin => From.Nickname.EqualsI(admin));
      }
    }

    public bool ReplyNotice {
      get {
        return _replyNotice;
      }
      set {
        _replyNotice = value;
      }
    }

    public string Channel {
      get {
        if (_channel == null) {
          return null;
        }
        return _channel.Name;
      }
    }

    public string GetPlayerName(string query) {
      // remove leading and trailing whitespace
      query = query.Trim();

      // fix player name
      if (query.StartsWithI("&") || query.EndsWithI("&")) {
        return query.Trim(new[] { '&' }).ToRsn();
      }

      // lookup player in users collection and get his name from database
      string playerName = (from peer in _users
                           where peer.Nickname.EqualsI(query)
                           select Database.Lookup<string>("rsn", "users", "fingerprint=@fp", new[] { new SQLiteParameter("@fp", peer.FingerPrint) }))
                           .FirstOrDefault();

      // if player was found return it, else just convert the query to a valid player name
      return playerName ?? query.ToRsn();
    }

    public void SendReply(string message) {
      if (_replyNotice) {
        _irc.Send(new NoticeMessage(message, this.From.Nickname));
      } else {
        if (_channel == null) {
          _irc.SendChat(message, this.From.Nickname);
        } else {
          _irc.SendChat(message, _channel.Name);
        }
      }
    }

  } //class CommandContext
} //namespace Supay.Bot