using System;
using System.Collections.Generic;
using System.Text;

using BigSister.Irc;
using BigSister.Irc.Messages;

namespace BigSister {
  class BotCommand {
    private Client _irc;
    private UserCollection _users;

    private User _from;
    private Channel _channel;
    private string _message;
    private string[] _messagetokens;
    private bool _replynotice;

    public BotCommand(Client irc, UserCollection users, User from, Channel channel, string message) {
      _irc = irc;
      _users = users;

      _from = from;
      _channel = channel;

      if (message[0] == '!' || message[0] == '.') {
        _replynotice = true;
      } else {
        _replynotice = false;
      }

      this.Message = message.Substring(1);
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
          _messagetokens = _message.Split(' ');
        }
      }
    }

    public string[] MessageTokens {
      get {
        return _messagetokens;
      }
    }

    public User From {
      get {
        return _from;
      }
    }

    public bool ReplyNotice {
      get {
        return _replynotice;
      }
      set {
        _replynotice = value;
      }
    }

    public string Channel {
      get {
        return _channel.Name;
      }
    }

    public string NickToRSN(string nick) {
      User u = _users.Find(nick);
      if (u != null)
        return u.RSN;
      return RSUtil.FixRSN(nick);
    }

    public void SendReply(string message) {
      if (_replynotice) {
        _irc.Send(new NoticeMessage(message, _from.Nick));
      } else {
        if (_channel == null)
          _irc.SendChat(message, _from.Nick);
        else
          _irc.SendChat(message, _channel.Name);
      }
    }

  }
}