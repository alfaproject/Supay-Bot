using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   A Message that edits the list of users on your accept list. </summary>
  [Serializable]
  public class AcceptListEditorMessage : CommandMessage {

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "ACCEPT";
      }
    }

    /// <summary>
    /// Validates this message against the given server support
    /// </summary>
    public override void Validate(ServerSupport serverSupport) {
      base.Validate(serverSupport);
      if (serverSupport != null && !serverSupport.CallerId) {
        throw new InvalidMessageException("Server does not support accept.");
      }

    }

    #region Properties

    /// <summary>
    /// Gets the collection of nicks being added to the accept list.
    /// </summary>
    public StringCollection AddedNicks {
      get {
        if (addedNicks == null) {
          addedNicks = new StringCollection();
        }
        return addedNicks;
      }
    }
    private StringCollection addedNicks;

    /// <summary>
    /// Gets the collection of nicks being removed from the accept list.
    /// </summary>
    public StringCollection RemovedNicks {
      get {
        if (removedNicks == null) {
          removedNicks = new StringCollection();
        }
        return removedNicks;
      }
    }
    private StringCollection removedNicks;

    #endregion

    #region Parsing

    /// <summary>
    ///   Determines if the message can be parsed by this type. </summary>
    public override bool CanParse(string unparsedMessage) {
      if (!base.CanParse(unparsedMessage)) {
        return false;
      }
      string firstParam = MessageUtil.GetParameter(unparsedMessage, 0);
      return (firstParam != "*");
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);

      foreach (string nick in parameters[0].Split(',')) {
        if (nick.StartsWith("-", StringComparison.Ordinal)) {
          this.RemovedNicks.Add(nick.Substring(1));
        } else {
          this.AddedNicks.Add(nick);
        }
      }
    }

    #endregion

    #region Formatting

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      StringCollection allNicks = new StringCollection();
      foreach (string rNick in RemovedNicks) {
        allNicks.Add("-" + rNick);
      }
      foreach (string aNick in AddedNicks) {
        allNicks.Add(aNick);
      }

      writer.AddList(allNicks, ",", true);
    }

    #endregion

    #region Events

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(MessageConduit conduit) {
      conduit.OnAcceptListEditor(new IrcMessageEventArgs<AcceptListEditorMessage>(this));
    }

    #endregion

  }
}