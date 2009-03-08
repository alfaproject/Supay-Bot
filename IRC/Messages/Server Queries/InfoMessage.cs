using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The <see cref="InfoMessage"/> requests information which describes the server;
  ///   its version, when it was compiled, the patchlevel, when it was started, 
  ///   and any other miscellaneous information which may be considered to be relevant. </summary>
  [Serializable]
  public class InfoMessage : ServerQueryBase {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "INFO";
      }
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Target);
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnInfo(new IrcMessageEventArgs<InfoMessage>(this));
    }

  }
}