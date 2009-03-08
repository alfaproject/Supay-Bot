using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   One of the responses to the <see cref="LusersMessage"/> query. </summary>
  [Serializable]
  public class LusersMeReplyMessage : NumericMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="LusersMeReplyMessage"/> class.
    /// </summary>
    public LusersMeReplyMessage()
      : base() {
      this.InternalNumeric = 255;
    }

    /// <summary>
    /// Gets or sets the number of clients connected to the server.
    /// </summary>
    public virtual int ClientCount {
      get {
        return clientCount;
      }
      set {
        clientCount = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of servers linked to the current server.
    /// </summary>
    public virtual int ServerCount {
      get {
        return serverCount;
      }
      set {
        serverCount = value;
      }
    }

    private int clientCount = -1;
    private int serverCount = -1;
    private string iHave = "I have ";
    private string clientsAnd = " clients and ";
    private string servers = " servers";

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.iHave + this.ClientCount + this.clientsAnd + this.serverCount + this.servers);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      string payload = parameters[1];
      this.ClientCount = Convert.ToInt32(MessageUtil.StringBetweenStrings(payload, this.iHave, this.clientsAnd), CultureInfo.InvariantCulture);
      this.ServerCount = Convert.ToInt32(MessageUtil.StringBetweenStrings(payload, this.clientsAnd, this.servers), CultureInfo.InvariantCulture);

    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnLusersMeReply(new IrcMessageEventArgs<LusersMeReplyMessage>(this));
    }

  }
}