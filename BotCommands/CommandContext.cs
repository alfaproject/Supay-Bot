using BigSister.Irc;
using BigSister.Irc.Messages;

namespace BigSister {
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

    public string NickToRSN(string nick) {
      User u = _users.Find(nick);
      if (u != null)
        return u.RSN;
      return nick.ToRsn();
    }

    public void SendReply(string message) {
      if (_replyNotice) {
        _irc.Send(new NoticeMessage(message, this.From.Nick));
      } else {
        if (_channel == null) {
          _irc.SendChat(message, this.From.Nick);
        } else {
          _irc.SendChat(message, _channel.Name);
        }
      }
    }

  } //class CommandContext
} //namespace BigSister