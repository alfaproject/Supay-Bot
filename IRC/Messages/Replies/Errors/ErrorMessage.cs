using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   This class of message is sent to a client from a server when something bad happens. </summary>
  [Serializable]
  public abstract class ErrorMessage : NumericMessage {

    /// <summary>
    /// Gets the Numeric command of the Message
    /// </summary>
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