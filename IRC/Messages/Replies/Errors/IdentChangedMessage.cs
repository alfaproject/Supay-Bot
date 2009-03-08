using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The ErrorMessage sent when a user tries to connect with an ident containing invalid characters. </summary>
  /// <remarks>
  ///   Not all networks will send this message, some will silently change your ident,
  ///   while others will simply disconnect you. </remarks>
  [Serializable]
  public class IdentChangedMessage : ErrorMessage {
    //:irc.dkom.at 443 _aLfa_ :Your username a//b//c contained the invalid character(s) //// and has been changed to abc. Please use only the characters 0-9 a-z A-Z _ - or . in your username. Your username is the part before the @ in your email address.

    /// <summary>
    /// Creates a new instances of the <see cref="IdentChangedMessage"/> class.
    /// </summary>
    public IdentChangedMessage()
      : base() {
      this.InternalNumeric = 455;
    }

    /// <summary>
    /// Gets or sets the ident that was attempted
    /// </summary>
    public string Ident {
      get {
        return ident;
      }
      set {
        ident = value;
      }
    }
    private string ident;

    /// <summary>
    /// Gets or sets the characters in the attempted ident which were invalid
    /// </summary>
    public string InvalidCharacters {
      get {
        return invalidCharacters;
      }
      set {
        invalidCharacters = value;
      }
    }
    private string invalidCharacters;

    /// <summary>
    /// Gets or sets the new ident being assigned
    /// </summary>
    public string NewIdent {
      get {
        return newIdent;
      }
      set {
        newIdent = value;
      }
    }
    private string newIdent;

    /// <exclude />
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      string param = string.Format(CultureInfo.InvariantCulture, "Your username {0} contained the invalid character(s) {1} and has been changed to {2}. Please use only the characters 0-9 a-z A-Z _ - or . in your username. Your username is the part before the @ in your email address.", this.Ident, this.InvalidCharacters, this.NewIdent);
      writer.AddParameter(param);
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      string param = parameters[1];
      this.Ident = MessageUtil.StringBetweenStrings(param, "Your username ", " contained the invalid ");
      this.InvalidCharacters = MessageUtil.StringBetweenStrings(param, "invalid character(s) ", " and has ");
      this.NewIdent = MessageUtil.StringBetweenStrings(param, "has been changed to ", ". Please");
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnIdentChanged(new IrcMessageEventArgs<IdentChangedMessage>(this));
    }

  }
}