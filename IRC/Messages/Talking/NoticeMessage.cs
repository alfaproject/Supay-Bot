using System;

namespace Supay.Bot.Irc.Messages {
  /// <summary>
  ///   This message is similar to a <see cref="ChatMessage"/>, 
  ///   except that no auto-replies should ever be sent after receiving a <see cref="NoticeMessage"/>. </summary>
  [Serializable]
  class NoticeMessage : TextMessage {

    /// <summary>
    /// Creates a new instance of the <see cref="NoticeMessage"/> class.
    /// </summary>
    public NoticeMessage()
      : base() {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="NoticeMessage"/> class with the given text string.
    /// </summary>
    public NoticeMessage(string text)
      : base() {
      this.Text = text;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="NoticeMessage"/> class with the given text string and target channel or user.
    /// </summary>
    public NoticeMessage(string text, string target)
      : this(text) {
      this.Targets.Add(target);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="NoticeMessage"/> class with the given text string and target channels or users.
    /// </summary>
    public NoticeMessage(string text, params string[] targets)
      : this(text) {
      this.Targets.AddRange(targets);
    }

    /// <summary>
    /// Gets the Irc command associated with this message.
    /// </summary>
    protected override string Command {
      get {
        return "NOTICE";
      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(Supay.Bot.Irc.Messages.MessageConduit conduit) {
      conduit.OnNotice(new IrcMessageEventArgs<TextMessage>(this));
    }

  }
}