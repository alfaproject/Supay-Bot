using System;
using System.Globalization;
using System.Text;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The reply to the <see cref="FingerRequestMessage"/>, containing the user's name and idle time. </summary>
  [Serializable]
  class FingerReplyMessage : CtcpReplyMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="FingerReplyMessage"/> class.
    /// </summary>
    public FingerReplyMessage()
      : base() {
      this.InternalCommand = "FINGER";
    }


    /// <summary>
    /// Gets or sets the real name of the user.
    /// </summary>
    public virtual string RealName {
      get {
        return this.realName;
      }
      set {
        this.realName = value;
      }
    }

    /// <summary>
    /// Gets or sets the login name of the user.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
    public virtual string LoginName {
      get {
        return this.loginName;
      }
      set {
        this.loginName = value;
      }
    }

    /// <summary>
    ///   Gets or sets the number of seconds that the user has been idle. </summary>
    public virtual double IdleSeconds {
      get {
        return this.idleSeconds;
      }
      set {
        this.idleSeconds = value;
      }
    }


    /// <summary>
    /// Gets the data payload of the Ctcp request.
    /// </summary>
    protected override string ExtendedData {
      get {
        StringBuilder result = new StringBuilder();
        result.Append(":");
        result.Append(this.RealName);
        result.Append(" (");
        result.Append(this.LoginName);
        result.Append(") - Idle ");
        result.Append(this.IdleSeconds.ToStringI());
        result.Append(" seconds");
        return result.ToString();
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnFingerReply(new IrcMessageEventArgs<FingerReplyMessage>(this));
    }

    private string realName = string.Empty;
    private string loginName = string.Empty;
    private double idleSeconds;

    /// <summary>
    ///   Parses the given string to populate this <see cref="IrcMessage"/>. </summary>
    public override void Parse(string unparsedMessage) {
      base.Parse(unparsedMessage);
      string payload = CtcpUtil.GetExtendedData(unparsedMessage);
      if (payload.StartsWith(":", StringComparison.Ordinal)) {
        payload = payload.Substring(1);
      }
      this.RealName = payload.Substring(0, payload.IndexOf(' '));

      int startOfLoginName = payload.IndexOf(" (", StringComparison.Ordinal);
      int endOfLoginName = payload.IndexOf(')');
      if (startOfLoginName > 0) {
        startOfLoginName += 2;
        this.LoginName = payload.Substring(startOfLoginName, endOfLoginName - startOfLoginName);

        int startOfIdle = payload.IndexOf("- Idle ", StringComparison.Ordinal);
        if (startOfIdle > 0) {
          startOfIdle += 6;
          string idleSecs = payload.Substring(startOfIdle, payload.Length - startOfIdle - 8);
          double foo;
          if (double.TryParse(idleSecs, System.Globalization.NumberStyles.Any, null, out foo)) {
            this.IdleSeconds = foo;
          } else {
            this.IdleSeconds = -1;
          }

        }
      }
    }

  }
}