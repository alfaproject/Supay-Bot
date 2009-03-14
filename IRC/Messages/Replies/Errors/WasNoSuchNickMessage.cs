using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Returned from the server in response to a WhoWasMessage to indicate there is no history information for that nick. </summary>
  [Serializable]
  public class WasNoSuchNickMessage : ErrorMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="WasNoSuchNickMessage"/> class. </summary>
    public WasNoSuchNickMessage()
      : base(406) {
    }

    private string _nick;

    /// <summary>
    /// The nick which had no information
    /// </summary>
    public string Nick {
      get {
        return _nick;
      }
      set {
        _nick = value;
      }
    }


    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Nick);
      writer.AddParameter("There was no such nickname");
    }

    /// <summary>
    /// Parses the parameter portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count > 1) {
        this.Nick = parameters[1];
      } else {
        this.Nick = string.Empty;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnWasNoSuchNick(new IrcMessageEventArgs<WasNoSuchNickMessage>(this));
    }

  }
}