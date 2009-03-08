using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The UserNotificationMessage is used at the beginning of connection to specify the username, _hostName and realname of a new user. </summary>
  [Serializable]
  public class UserNotificationMessage : CommandMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "USER";
      }
    }

    /// <summary>
    /// Gets or sets the UserName of client.
    /// </summary>
    public virtual string UserName {
      get {
        return this.userName;
      }
      set {
        this.userName = value;
      }
    }
    private string userName = string.Empty;

    /// <summary>
    ///   Gets or sets if the client is initialized with a user mode of invisible. </summary>
    public virtual bool InitialInvisibility {
      get {
        return initialInvisibility;
      }
      set {
        initialInvisibility = value;
      }
    }
    private bool initialInvisibility = true;

    /// <summary>
    ///   Gets or sets if the client is initialized with a user mode of receiving wallops. </summary>
    public virtual bool InitialWallops {
      get {
        return initialWallops;
      }
      set {
        initialWallops = value;
      }
    }
    private bool initialWallops = false;

    /// <summary>
    /// Gets or sets the real name of the client.
    /// </summary>
    public virtual string RealName {
      get {
        return this.realName;
      }
      set {
        this.realName = value;
      }
    }
    private string realName = string.Empty;

    /// <exclude />
    public override bool CanParse(string unparsedMessage) {
      if (!base.CanParse(unparsedMessage)) {
        return false;
      }
      StringCollection p = MessageUtil.GetParameters(unparsedMessage);
      int tempInt;
      if (p.Count != 4 || !int.TryParse(p[1], out tempInt) || p[2] != "*") {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.UserName);
      int modeBitMask = 0;
      if (this.InitialInvisibility)
        modeBitMask += 8;
      if (this.InitialWallops)
        modeBitMask += 4;
      writer.AddParameter(modeBitMask.ToString(CultureInfo.InvariantCulture));
      writer.AddParameter("*");
      writer.AddParameter(this.RealName);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count >= 4) {
        this.UserName = parameters[0];
        this.RealName = parameters[3];
        int modeBitMask = Convert.ToInt32(parameters[1], CultureInfo.InvariantCulture);
        this.InitialInvisibility = ((modeBitMask & 8) == 8);
        this.InitialWallops = ((modeBitMask & 4) == 4);
      } else {
        this.UserName = string.Empty;
        this.RealName = string.Empty;
        this.InitialInvisibility = true;
        this.InitialWallops = false;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnUserNotification(new IrcMessageEventArgs<UserNotificationMessage>(this));
    }

  }
}