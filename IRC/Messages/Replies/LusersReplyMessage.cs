using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   One of the responses to the <see cref="LusersMessage"/> query. </summary>
  [Serializable]
  public class LusersReplyMessage : NumericMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="LusersReplyMessage"/> class.
    /// </summary>
    public LusersReplyMessage()
      : base() {
      this.InternalNumeric = 251;
    }


    /// <summary>
    /// Gets or sets the number of users connected to irc.
    /// </summary>
    public virtual int UserCount {
      get {
        return userCount;
      }
      set {
        userCount = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of invisible users connected to irc.
    /// </summary>
    public virtual int InvisibleCount {
      get {
        return invisibleCount;
      }
      set {
        invisibleCount = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of servers connected on the network.
    /// </summary>
    public virtual int ServerCount {
      get {
        return serverCount;
      }
      set {
        serverCount = value;
      }
    }

    private int userCount = -1;
    private int invisibleCount = -1;
    private int serverCount = -1;
    private string thereAre = "There are ";
    private string usersAnd = " users and ";
    private string invisibleOn = " invisible on ";
    private string servers = " servers";

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.thereAre + this.UserCount + this.usersAnd + this.InvisibleCount + this.invisibleOn + this.serverCount + this.servers);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      string payload = parameters[1];
      this.UserCount = Convert.ToInt32(MessageUtil.StringBetweenStrings(payload, this.thereAre, this.usersAnd), CultureInfo.InvariantCulture);
      this.InvisibleCount = Convert.ToInt32(MessageUtil.StringBetweenStrings(payload, this.usersAnd, this.invisibleOn), CultureInfo.InvariantCulture);
      this.ServerCount = Convert.ToInt32(MessageUtil.StringBetweenStrings(payload, this.invisibleOn, this.servers), CultureInfo.InvariantCulture);

    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnLusersReply(new IrcMessageEventArgs<LusersReplyMessage>(this));
    }

  }
}