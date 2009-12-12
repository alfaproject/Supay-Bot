using System;
using System.Collections.Specialized;
using System.Globalization;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   A reply to a <see cref="IrcxMessage"/> or a <see cref="IsIrcxMessage"/>. </summary>
  [Serializable]
  class IrcxReplyMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="IrcxReplyMessage"/>. </summary>
    public IrcxReplyMessage()
      : base(800) {
    }

    /// <summary>
    ///   Gets or sets if the server has set the client into ircx mode. </summary>
    public virtual bool IsIrcxClientMode {
      get {
        return this.isIrcxClientMode;
      }
      set {
        this.isIrcxClientMode = value;
      }
    }
    private bool isIrcxClientMode;

    /// <summary>
    /// Gets or sets the version of Ircx the server implements.
    /// </summary>
    public virtual string Version {
      get {
        return this.ircxVersion;
      }
      set {
        this.ircxVersion = value;
      }
    }
    private string ircxVersion = string.Empty;

    /// <summary>
    /// Gets the collection of authentication packages
    /// </summary>
    public virtual StringCollection AuthenticationPackages {
      get {
        return this.authenticationPackages;
      }
    }
    private StringCollection authenticationPackages = new StringCollection();

    /// <summary>
    /// Gets or sets the maximum message length, in bytes.
    /// </summary>
    public virtual int MaximumMessageLength {
      get {
        return this.maximumMessageLength;
      }
      set {
        this.maximumMessageLength = value;
      }
    }
    private int maximumMessageLength = -1;

    /// <summary>
    /// Gets or sets the tokens
    /// </summary>
    /// <remarks>
    /// There are no known servers that implement this property.
    /// It is almost always just *.
    /// </remarks>
    public virtual string Tokens {
      get {
        return this.tokens;
      }
      set {
        this.tokens = value;
      }
    }
    private string tokens = "*";

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      if (this.IsIrcxClientMode) {
        writer.AddParameter("1");
      } else {
        writer.AddParameter("0");
      }
      writer.AddParameter(this.Version);
      writer.AddList(this.AuthenticationPackages, ",", false);
      writer.AddParameter(this.MaximumMessageLength.ToStringI());
      writer.AddParameter(this.Tokens);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      if (parameters.Count >= 5) {
        this.IsIrcxClientMode = (parameters[1] == "1");
        this.Version = parameters[2];
        this.AuthenticationPackages.Clear();
        foreach (string package in parameters[3].Split(',')) {
          this.AuthenticationPackages.Add(package);
        }
        this.MaximumMessageLength = int.Parse(parameters[4], CultureInfo.InvariantCulture);
        if (parameters.Count == 6) {
          this.Tokens = parameters[5];
        } else {
          this.Tokens = string.Empty;
        }
      } else {
        this.IsIrcxClientMode = false;
        this.Version = string.Empty;
        this.AuthenticationPackages.Clear();
        this.MaximumMessageLength = -1;
        this.Tokens = string.Empty;
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnIrcxReply(new IrcMessageEventArgs<IrcxReplyMessage>(this));
    }

  }
}