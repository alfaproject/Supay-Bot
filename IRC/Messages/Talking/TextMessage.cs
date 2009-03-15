using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   An <see cref="IrcMessage"/> which carries communication from a person to another person or channel. </summary>
  [Serializable]
  abstract class TextMessage : CommandMessage, IChannelTargetedMessage, IQueryTargetedMessage {

    /// <summary>
    /// Gets the target of this <see cref="TextMessage"/>.
    /// </summary>
    public virtual StringCollection Targets {
      get {
        return this.targets;
      }
    }
    private StringCollection targets = new StringCollection();

    /// <summary>
    /// Gets or sets the actual text of this <see cref="TextMessage"/>.
    /// </summary>
    /// <remarks>
    /// This property holds the core purpose of irc itself... sending text communication to others.
    /// </remarks>
    public virtual string Text {
      get {
        return this.text;
      }
      set {
        this.text = value;
      }
    }
    private string text = string.Empty;

    /// <summary>
    /// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
    /// </summary>
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddList(this.Targets, ",", false);
      writer.AddParameter(this.Text, true);
    }

    /// <summary>
    /// Parses the parameters portion of the message.
    /// </summary>
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Targets.Clear();
      if (parameters.Count >= 2) {
        this.Targets.AddRange(parameters[0].Split(','));
        this.Text = parameters[1];
      } else {
        this.Text = string.Empty;
      }
    }


    #region IChannelTargetedMessage Members

    bool IChannelTargetedMessage.IsTargetedAtChannel(string channelName) {
      return IsTargetedAtChannel(channelName);
    }

    /// <summary>
    /// Determines if the the current message is targeted at the given channel.
    /// </summary>
    protected virtual bool IsTargetedAtChannel(string channelName) {
      return MessageUtil.ContainsIgnoreCaseMatch(this.Targets, channelName);
    }

    #endregion

    #region IQueryTargetedMessage Members

    bool IQueryTargetedMessage.IsQueryToUser(User user) {
      return IsQueryToUser(user);
    }

    /// <summary>
    /// Determines if the current message is targeted at a query to the given user.
    /// </summary>
    protected virtual bool IsQueryToUser(User user) {
      foreach (string target in this.Targets) {
        if (MessageUtil.IsIgnoreCaseMatch(user.Nick, target)) {
          return true;
        }
      }
      return false;
    }

    #endregion

  }
}