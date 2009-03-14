using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Signals the end of the <see cref="WhoReplyMessage"/> list. </summary>
  [Serializable]
  public class WhoEndReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="WhoEndReplyMessage"/> class. </summary>
    public WhoEndReplyMessage()
      : base(315) {
    }

    /// <summary>
    /// Gets or sets the nick of the user in the Who reply list.
    /// </summary>
    public virtual string Nick {
      get {
        return nick;
      }
      set {
        nick = value;
      }
    }
    private string nick = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Nick);
      writer.AddParameter("End of /WHO list");
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count == 3) {
        this.Nick = parameters[1];
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnWhoEndReply(new IrcMessageEventArgs<WhoEndReplyMessage>(this));
    }

  }
}