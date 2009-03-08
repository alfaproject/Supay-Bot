using System;

namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   A channel mode sent in a <see cref="ChannelModeMessage"/> in its <see cref="ChannelModeMessage.ModeChanges"/> property. </summary>
  public abstract class ChannelMode {

    /// <summary>
    /// Gets the irc string representation of the mode being changed or applied.
    /// </summary>
    protected abstract string Symbol {
      get;
    }

    /// <summary>
    /// Gets or sets the <see cref="ModeAction"/> applied.
    /// </summary>
    public ModeAction Action {
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
    ///   Applies the mode to the given <see cref="ChannelModeMessage"/>. </summary>
    /// <param name="msg">
    ///   The message which will be modified to include this mode. </param>
    /// <param name="includeAction">
    ///   Specifies if the action modifier should be applied. </param>
    public virtual void ApplyTo(ChannelModeMessage msg, bool includeAction) {
      this.AddChanges(msg, includeAction);
      this.AddParameter(msg);
    }

    /// <summary>
    ///   Applies this mode to the ModeChanges property of the given <see cref="ChannelModeMessage" />. </summary>
    /// <param name="msg">
    ///   The message which will be modified to include this mode. </param>
    /// <param name="includeAction">
    ///   Specifies if the action modifier should be applied. </param>
    protected virtual void AddChanges(ChannelModeMessage msg, bool includeAction) {
      if (includeAction) {
        msg.ModeChanges += this.Action.ToString();
      }
      msg.ModeChanges += this.Symbol;
    }

    /// <summary>
    /// Applies this mode to the ModeArguments property of the given <see cref="ChannelModeMessage" />.
    /// </summary>
    /// <param name="msg">The message which will be modified to include this mode.</param>
    protected virtual void AddParameter(ChannelModeMessage msg) {

    }

  }
}