using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   A Monitor system error message informing the user which nicks couldn't be added 
  ///   to their monitor list because it is full. </summary>
  [Serializable]
  class MonitorListFullMessage : NumericMessage {

    /// <summary>
    ///   Creates a new instance of the <see cref="MonitorListFullMessage"/>. </summary>
    public MonitorListFullMessage()
      : base(734) {
    }

    /// <summary>
    /// Gets or sets the limit of monitor lists on the server.
    /// </summary>
    public int Limit {
      get {
        return limit;
      }
      set {
        limit = value;
      }
    }
    private int limit;

    /// <summary>
    /// Gets the collection of nicks which couldn't be added to the monitor list.
    /// </summary>
    public StringCollection Nicks {
      get {
        if (nicks == null) {
          nicks = new StringCollection();
        }
        return nicks;
      }
    }
    private StringCollection nicks;

    /// <exclude />
    protected override void AddParametersToFormat(IrcMessageWriter writer) {
      base.AddParametersToFormat(writer);
      writer.AddParameter(this.Limit.ToStringI());
      writer.AddParameter(MessageUtil.CreateList(this.Nicks, ","));
      writer.AddParameter("Monitor list is full.");
    }

    /// <exclude />
    protected override void ParseParameters(StringCollection parameters) {
      base.ParseParameters(parameters);
      this.Limit = -1;
      this.Nicks.Clear();

      if (parameters.Count > 2) {
        this.Limit = Convert.ToInt32(parameters[1], CultureInfo.InvariantCulture);

        string userListParam = parameters[2];
        string[] userList = userListParam.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string nick in userList) {
          this.Nicks.Add(nick);
        }

      }
    }

    /// <summary>
    /// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
    /// </summary>
    public override void Notify(BigSister.Irc.Messages.MessageConduit conduit) {
      conduit.OnMonitorListFull(new IrcMessageEventArgs<MonitorListFullMessage>(this));
    }

  }
}