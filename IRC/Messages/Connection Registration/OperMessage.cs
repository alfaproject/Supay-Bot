using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   OperMessage is used by a normal user to obtain irc operator privileges.
  ///   (This does not refer to channel ops)
  ///   The correct combination of <see cref="Name"/> and <see cref="Password"/> are required to gain Operator privileges. </summary>
  [Serializable]
  class OperMessage : CommandMessage {

    /// <summary>
    /// Creates a new instance of the OperMessage class.
    /// </summary>
    public OperMessage() {
    }

    /// <summary>
    /// Creates a new instance of the OperMessage class with the given name and password.
    /// </summary>
    public OperMessage(string name, string password) {
      this.name = name;
      this.password = password;
    }

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "OPER";
      }
    }

    /// <summary>
    /// Gets or sets the password for the sender.
    /// </summary>
    public virtual string Password {
      get {
        return this.password;
      }
      set {
        this.password = value;
      }
    }
    private string password = string.Empty;

    /// <summary>
    /// Gets or sets the name for the sender.
    /// </summary>
    public virtual string Name {
      get {
        return this.name;
      }
      set {
        this.name = value;
      }
    }
    private string name = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Name);
      writer.AddParameter(this.Password);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count >= 2) {
        this.Name = parameters[0];
        this.Password = parameters[1];
      } else {
        this.Name = string.Empty;
        this.Password = string.Empty;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnOper(new IrcMessageEventArgs<OperMessage>(this));
    }

  }
}