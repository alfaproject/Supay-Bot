using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   A reply to a <see cref="KnockMessage"/>. </summary>
  [Serializable]
  class KnockReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="KnockReplyMessage"/>. </summary>
    public KnockReplyMessage()
      : base(711) {
    }

    /// <summary>
    /// Gets or sets the channel that was knocked on.
    /// </summary>
    public virtual string Channel {
      get {
        return this.channel;
      }
      set {
        this.channel = value;
      }
    }
    private string channel = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Channel);
      writer.AddParameter("Your KNOCK has been delivered.");
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count > 1) {
        this.Channel = parameters[1];
      } else {
        this.Channel = string.Empty;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnKnockReply(new IrcMessageEventArgs<KnockReplyMessage>(this));
    }

  }
}