using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The reply to the <see cref="SilenceMessage"/> query. </summary>
  [Serializable]
  class SilenceReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="SilenceReplyMessage"/>. </summary>
    public SilenceReplyMessage()
      : base(271) {
    }

    /// <summary>
    /// Gets or sets the user being silenced.
    /// </summary>
    public virtual User SilencedUser {
      get {
        return this.silencedUser;
      }
      set {
        this.silencedUser = value;
      }
    }
    private User silencedUser = new User();

    /// <summary>
    /// Gets or sets the nick of the owner of the silence list
    /// </summary>
    public virtual string SilenceListOwner {
      get {
        return this.silenceListOwner;
      }
      set {
        this.silenceListOwner = value;
      }
    }
    private string silenceListOwner = string.Empty;


    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.SilenceListOwner);
      writer.AddParameter(this.SilencedUser.ToString());
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count > 2) {
        this.SilenceListOwner = parameters[1];
        this.SilencedUser = new User(parameters[2]);
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnSilenceReply(new IrcMessageEventArgs<SilenceReplyMessage>(this));
    }

  }
}