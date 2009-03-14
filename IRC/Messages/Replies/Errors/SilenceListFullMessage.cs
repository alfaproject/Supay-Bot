using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage received when a user's silence list is full, and a SilenceMessage is sent adding a User to the list. </summary>
  [Serializable]
  public class SilenceListFullMessage : ErrorMessage {
    //:irc.dkom.at 511 _aLfa_ *!*@*aol.com :Your silence list is full

    /// <summary>
    ///   Creates a new instance of the <see cref="SilenceListFullMessage"/> class. </summary>
    public SilenceListFullMessage()
      : base(511) {
    }

    /// <summary>
    /// Gets or sets the mask of the user being silenced
    /// </summary>
    public User SilenceMask {
      get {
        return silenceMask;
      }
      set {
        silenceMask = value;
      }
    }
    private User silenceMask;


    /// <exclude />
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.SilenceMask.ToString());
      writer.AddParameter("Your silence list is full");
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.SilenceMask = new User();
      if (parameters.Count > 1) {
        this.SilenceMask.Parse(parameters[1]);
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnSilenceListFull(new IrcMessageEventArgs<SilenceListFullMessage>(this));
    }

  }
}