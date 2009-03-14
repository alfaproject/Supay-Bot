using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The reply to a <see cref="StatsMessage"/> query. </summary>
  [Serializable]
  public class StatsReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="StatsReplyMessage"/> class. </summary>
    public StatsReplyMessage()
      : base(250) {
    }

    /// <summary>
    /// The information requested.
    /// </summary>
    public virtual string Stats {
      get {
        return stats;
      }
      set {
        stats = value;
      }
    }
    private string stats = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Stats);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Stats = parameters[1];
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnStatsReply(new IrcMessageEventArgs<StatsReplyMessage>(this));
    }

  }
}