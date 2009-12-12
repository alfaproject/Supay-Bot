using System;
using System.Collections.Specialized;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   The error recieved when a message containing target parameters has too many targets specified. </summary>
  [Serializable]
  class TooManyTargetsMessage : ErrorMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="TooManyTargetsMessage"/> class. </summary>
    public TooManyTargetsMessage()
      : base(407) {
    }

    /// <summary>
    /// Gets or sets the target which was invalid.
    /// </summary>
    public virtual string InvalidTarget {
      get {
        return invalidTarget;
      }
      set {
        invalidTarget = value;
      }
    }
    private string invalidTarget = string.Empty;

    /// <summary>
    /// Gets or sets the errorcode
    /// </summary>
    /// <remarks>An example error code might be, "Duplicate"</remarks>
    public virtual string ErrorCode {
      get {
        return errorCode;
      }
      set {
        errorCode = value;
      }
    }
    private string errorCode = string.Empty;

    /// <summary>
    /// Gets or sets the message explaining what was done about the error.
    /// </summary>
    public virtual string AbortMessage {
      get {
        return abortMessage;
      }
      set {
        abortMessage = value;
      }
    }
    private string abortMessage = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.InvalidTarget);
      writer.AddParameter(this.ErrorCode + " recipients. " + this.AbortMessage);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);

      this.InvalidTarget = string.Empty;
      this.ErrorCode = string.Empty;
      this.AbortMessage = string.Empty;

      if (parameters.Count > 1) {
        this.InvalidTarget = parameters[1];
        if (parameters.Count > 2) {
          string[] messagePieces = System.Text.RegularExpressions.Regex.Split(parameters[2], " recipients.");
          if (messagePieces.Length == 2) {
            this.ErrorCode = messagePieces[0];
            this.AbortMessage = messagePieces[1];
          }
        }
      }
    }

    // "<target> :<error code> recipients. <abort message>"

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnTooManyTargets(new IrcMessageEventArgs<TooManyTargetsMessage>(this));
    }

  }
}