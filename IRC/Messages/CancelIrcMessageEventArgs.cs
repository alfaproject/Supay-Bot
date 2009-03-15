using System;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   The information for a handler of an IrcMessage event which can be canceled. </summary>
  [Serializable]
  class CancelIrcMessageEventArgs<T> : EventArgs where T : IrcMessage {

    /// <summary>
    /// Initializes a new instance of the <see cref="IrcMessageEventArgs&lt;T&gt;"/> class with the given <see cref="IrcMessage"/>.
    /// </summary>
    public CancelIrcMessageEventArgs(T msg) {
      this.Message = msg;
    }

    /// <summary>
    /// Gets or sets the Message for the event.
    /// </summary>
    public T Message {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the event should be canceled.
    /// </summary>
    public bool Cancel {
      get;
      set;
    }

  }
}