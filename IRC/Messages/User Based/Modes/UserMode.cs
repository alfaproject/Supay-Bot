using System;

namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   The list of known user modes sent in a <see cref="UserModeMessage"/> in its <see cref="UserModeMessage.ModeChanges"/> property. </summary>
  public abstract class UserMode {

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected abstract string Symbol {
      get;
    }

    /// <summary>
    /// Gets or sets the <see cref="ModeAction"/> applied.
    /// </summary>
    public virtual ModeAction Action {
      get {
        return action;
      }
      set {
        action = value;
      }
    }
    private ModeAction action = ModeAction.Add;

    /// <summary>
    /// A string representation of the mode.
    /// </summary>
    public override string ToString() {
      return this.Action.ToString() + this.Symbol;
    }

    /// <summary>
    ///   Applies the mode to the given <see cref="UserModeMessage"/>. </summary>
    /// <param name="msg">
    ///   The message which will be modified to include this mode. </param>
    /// <param name="includeAction">
    ///   Specifies if the action modifier should be applied. </param>
    public virtual void ApplyTo(UserModeMessage msg, bool includeAction) {
      if (msg == null) {
        return;
      }
      if (includeAction) {
        msg.ModeChanges += this.Action.ToString();
      }
      msg.ModeChanges += this.Symbol;
    }

  }
}