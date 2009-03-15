using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Requests information about the nicks supplied in the Nick property. </summary>
  [Serializable]
  class UserHostMessage : CommandMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "USERHOST";
      }
    }

    /// <summary>
    /// Gets the collection of nicks to request information for.
    /// </summary>
    public virtual StringCollection Nicks {
      get {
        return nicks;
      }
    }

    private StringCollection nicks = new StringCollection();

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddList(this.Nicks, " ");
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Nicks.Clear();
      foreach (string nick in parameters) {
        this.Nicks.Add(nick);
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnUserHost(new IrcMessageEventArgs<UserHostMessage>(this));
    }

  }
}