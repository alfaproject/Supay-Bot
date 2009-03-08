using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The UserNotificationServerSideMessage is passed between servers to notify of a new user on the network. </summary>
  [Serializable]
  public class UserNotificationServerSideMessage : CommandMessage {

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
    /// Gets or sets the name of the user's host.
    /// </summary>
    public string HostName {
      get {
        return hostName;
      }
      set {
        hostName = value;
      }
    }
    private string hostName;

    /// <summary>
    /// Gets or sets the name of the server which the user is on.
    /// </summary>
    public string ServerName {
      get {
        return serverName;
      }
      set {
        serverName = value;
      }
    }
    private string serverName;

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
      if (p.Count != 4 || p[2] == "*") {
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
      writer.AddParameter(this.HostName);
      writer.AddParameter(this.ServerName);
      writer.AddParameter(this.RealName);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count >= 4) {
        this.UserName = parameters[0];
        this.HostName = parameters[1];
        this.ServerName = parameters[2];
        this.RealName = parameters[3];
      } else {
        this.UserName = string.Empty;
        this.HostName = string.Empty;
        this.ServerName = string.Empty;
        this.RealName = string.Empty;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnUserNotificationServerSide(new IrcMessageEventArgs<UserNotificationServerSideMessage>(this));
    }

  }
}