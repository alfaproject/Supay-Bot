using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   Represents a single generic rfc1459 irc message to or from an irc server. </summary>
  [Serializable]
  class GenericMessage : IrcMessage {

    #region Properties

    /// <summary>
    /// Gets or sets the message's Command
    /// </summary>
    public virtual string Command {
      get {
        return this._command;
      }
      set {
        if (value == null) {
          throw new ArgumentNullException("value");
        }
        this._command = value;
      }
    }
    private string _command = string.Empty;

    /// <summary>
    /// Gets the message's parameters after the command.
    /// </summary>
    public virtual StringCollection Parameters {
      get {
        return this._parameters;
      }
    }
    private StringCollection _parameters = new StringCollection();

    #endregion

    #region Methods

    /// <summary>
    /// This is not meant to be used from your code.
    /// </summary>
    /// <remarks>
    /// The conduit calls Notify on messages to have the message raise the appropriate event on the conduit.
    /// This is done automaticly by your <see cref="Client"/> after message are recieved and parsed.
    /// </remarks>
    public override void Notify(MessageConduit conduit) {
      conduit.OnGenericMessage(new IrcMessageEventArgs<GenericMessage>(this));
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Command);
      foreach (string param in this.Parameters) {
        writer.AddParameter(param);
      }
    }


    /// <summary>
    ///   Determines if the given string is parsable by this <see cref="IrcMessage"/> subclass. </summary>
    /// <remarks>
    ///   <see cref="GenericMessage"/> always returns true. </remarks>
    public override bool CanParse(string unparsedMessage) {
      return true;
    }

    /// <summary>
    /// Parses the command portion of the message.
    /// </summary>
    protected override void ParseCommand(string command) {
      base.ParseCommand(command);
      this._command = command;
    }

    /// <summary>
    /// Parses the parameter portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this._parameters = parameters;
    }

    #endregion

  }
}