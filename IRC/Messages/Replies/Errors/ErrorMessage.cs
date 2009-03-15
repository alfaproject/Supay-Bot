using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   This class of message is sent to a client from a server when something bad happens. </summary>
  [Serializable]
  abstract class ErrorMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="ErrorMessage"/> class with the numeric command. </summary>
    /// <param name="internalNumeric">
    ///   Numeric command of the Message. </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public ErrorMessage(int internalNumeric)
      : base() {
      this.InternalNumeric = internalNumeric;
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="ErrorMessage"/> class. </summary>
    public ErrorMessage()
      : base() {
    }

    /// <summary>
    ///   Gets the Numeric command of the Message. </summary>
    public override int InternalNumeric {
      get {
        return base.InternalNumeric;
      }
      protected set {
        if (NumericMessage.IsError(value)) {
          base.InternalNumeric = value;
        } else {
          throw new ArgumentOutOfRangeException("value", value, "ErrorMessage numerics must be between 400 and 600.");
        }
      }
    }

  }
}