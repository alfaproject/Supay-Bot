using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   By using the NamesMessage, a user can list all nicknames that are visible to them on any channel that they can see. </summary>
  /// <remarks>
  ///   Channel names which they can see are those which aren't private ( <see cref="Modes.PrivateMode"/> ) or secret ( <see cref="Modes.SecretMode"/> ) or those which they are actually on. </remarks>
  [Serializable]
  class NamesMessage : CommandMessage {


    /// <summary>
    /// Gets the channels that should be queried for their users.
    /// </summary>
    public virtual StringCollection Channels {
      get {
        return this.channels;
      }
    }
    private StringCollection channels = new StringCollection();

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "NAMES";
      }
    }


    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      if (this.Channels.Count != 0) {
        writer.AddList(this.Channels, ",", true);
      }
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Channels.Clear();
      if (parameters.Count >= 1) {
        this.Channels.AddRange(parameters[0].Split(','));
      }
    }


    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnNames(new IrcMessageEventArgs<NamesMessage>(this));
    }

  }
}