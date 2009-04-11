using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Requests information about the given user or users. </summary>
  [Serializable]
  class WhoMessage : CommandMessage {

    /// <summary>
    /// Gets or sets the mask which is matched for users to return information about.
    /// </summary>
    public virtual BigSister.Irc.User Mask {
      get {
        return this.mask;
      }
      set {
        this.mask = value;
      }
    }
    private User mask = new BigSister.Irc.User();

    /// <summary>
    ///   Gets or sets if the results should only contain irc operators. </summary>
    public virtual bool RestrictToOps {
      get {
        return restrictToOps;
      }
      set {
        restrictToOps = value;
      }
    }
    private bool restrictToOps;

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "WHO";
      }
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Mask.ToString());
      if (this.restrictToOps) {
        writer.AddParameter("o");
      }
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Mask = new User();
      if (parameters.Count >= 1) {
        this.Mask.Nick = parameters[0];
        this.RestrictToOps = (parameters.Count > 1 && parameters[1] == "o");
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnWho(new IrcMessageEventArgs<WhoMessage>(this));
    }

  }
}