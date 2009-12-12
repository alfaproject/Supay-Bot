using System;
using System.Collections.Specialized;
using System.Globalization;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   Represents an error message with a numeric command that is either unparsable or unimplemented. </summary>
  [Serializable]
  class GenericErrorMessage : ErrorMessage {

    /// <summary>
    /// Gets or sets the Numeric command of the Message
    /// </summary>
    public virtual int Command {
      get {
        return InternalNumeric;
      }
      set {
        InternalNumeric = value;
      }
    }

    /// <summary>
    /// Gets the text of the Message
    /// </summary>
    public virtual StringCollection Data {
      get {
        return data;
      }
    }
    private StringCollection data = new StringCollection();


    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddList(this.Data, " ", false);
    }

    /// <summary>
    /// Parses the command portion of the message.
    /// </summary>
    protected override void ParseCommand(string command) {
      base.ParseCommand(command);
      this.Command = Convert.ToInt32(command, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.data = parameters;
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnGenericErrorMessage(new IrcMessageEventArgs<GenericErrorMessage>(this));
    }

  }
}