using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A reply to a <see cref="WhoIsMessage"/> that contains 
  ///   basic information about the user in question. </summary>
  [Serializable]
  class WhoIsUserReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="WhoIsUserReplyMessage"/> class. </summary>
    public WhoIsUserReplyMessage()
      : base(311) {
    }

    /// <summary>
    /// Gets the information about the user in question.
    /// </summary>
    public virtual User User {
      get {
        return user;
      }
      set {
        user = value;
      }
    }

    private User user = new User();

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(User.Nick);
      writer.AddParameter(User.UserName);
      writer.AddParameter(User.HostName);
      writer.AddParameter("*");
      writer.AddParameter(User.RealName);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count > 5) {
        this.user = new User();
        user.Nick = parameters[1];
        user.UserName = parameters[2];
        user.HostName = parameters[3];
        user.RealName = parameters[5];
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnWhoIsUserReply(new IrcMessageEventArgs<WhoIsUserReplyMessage>(this));
    }

  }
}