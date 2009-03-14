using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The base class for all numeric messages sent from the server to the client. </summary>
  [Serializable]
  public abstract class NumericMessage : IrcMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="NumericMessage"/> class with the numeric command. </summary>
    /// <param name="internalNumeric">
    ///   Numeric command of the Message. </param>
    public NumericMessage(int internalNumeric)
      : base() {
      _internalNumeric = internalNumeric;
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="NumericMessage"/> class. </summary>
    public NumericMessage()
      : this(-1) {
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(_internalNumeric.ToString("000", CultureInfo.InvariantCulture));
      if (this.Target.Length != 0) {
        writer.AddParameter(this.Target);
      }
    }

    /// <summary>
    /// Gets or sets the target of the message.
    /// </summary>
    public virtual string Target {
      get {
        return target;
      }
      set {
        target = value;
      }
    }
    private string target = string.Empty;

    /// <summary>
    ///   Determines if the given numeric is an error message. </summary>
    public static bool IsError(int numeric) {
      int normalStart = 400;
      int normalEnd = 599;
      int ircxStart = 900;
      int ircxEnd = 998;
      return ((normalStart <= numeric && numeric <= normalEnd) || (ircxStart <= numeric && numeric <= ircxEnd));
    }

    /// <summary>
    ///   Determines if the given numeric is a command reply message. </summary>
    public static bool IsCommandReply(int numeric) {
      return (!IsError(numeric) && !IsDirect(numeric));
    }

    /// <summary>
    ///   Determines if the given numeric is a direct message. </summary>
    public static bool IsDirect(int numeric) {
      return (0 < numeric && numeric < 100);
    }

    /// <summary>
    ///   Gets the Numeric command of the Message. </summary>
    public virtual int InternalNumeric {
      get {
        return _internalNumeric;
      }
      protected set {
        _internalNumeric = value;
      }
    }
    private int _internalNumeric;

    /// <summary>
    ///   Determines if the message can be parsed by this type. </summary>
    public override bool CanParse(string unparsedMessage) {
      double foo;
      if (double.TryParse(MessageUtil.GetCommand(unparsedMessage), System.Globalization.NumberStyles.Integer, null, out foo)) {
        if (0 <= foo && foo < 1000) {
          if (InternalNumeric == -1) {
            return true;
          } else {
            int parsedNumeric = Convert.ToInt32(foo);
            return (InternalNumeric == parsedNumeric);
          }
        } else {
          return false;
        }
      }
      return false;
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count > 0) {
        this.Target = parameters[0];
      } else {
        this.Target = string.Empty;
      }
    }

  }
}