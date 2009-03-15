using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Writes <see cref="BigSister.Irc.Messages.IrcMessage"/> data to a <see cref="TextWriter"/> in irc protocol format. </summary>
  class IrcMessageWriter : IDisposable {

    #region Constructors

    /// <summary>
    /// Creates a new instance of the IrcMessageWriter class without an <see cref="InnerWriter"/> to write to.
    /// </summary>
    public IrcMessageWriter() {
      this.resetDefaults();
    }

    /// <summary>
    /// Creates a new instance of the IrcMessageWriter class with the given <see cref="InnerWriter"/> to write to.
    /// </summary>
    public IrcMessageWriter(TextWriter writer) {
      this.writer = writer;
      this.resetDefaults();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> which will written to.
    /// </summary>
    public TextWriter InnerWriter {
      get {
        return this.writer;
      }
      set {
        this.writer = value;
      }
    }

    private TextWriter writer;

    /// <summary>
    /// Gets or sets the ID of the sender of the message.
    /// </summary>
    public string Sender {
      get {
        return sender;
      }
      set {
        sender = value;
      }
    }
    private string sender;

    /// <summary>
    ///   Gets or sets if a new line is appended to the end of messages when they are written. </summary>
    public bool AppendNewLine {
      get;
      set;
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Adds the given possibly-splittable parameter to the writer. </summary>
    /// <param name="value">
    ///   The parameter to add to the writer. </param>
    /// <param name="splittable">
    ///   Indicates if the parameter can be split across messages written. </param>
    public void AddParameter(string value, bool splittable) {
      this.parameters.Add(value);
      if (splittable) {
        this.AddSplittableParameter();
      }
    }

    /// <summary>
    /// Adds the given non-splittable parameter to the writer.
    /// </summary>
    /// <param name="value">The parameter to add to the writer</param>
    public void AddParameter(string value) {
      this.AddParameter(value, false);
    }

    /// <summary>
    ///   Adds a possibly-splittable list of parameters to the writer. </summary>
    /// <param name="value">
    ///   The list of parameters to add. </param>
    /// <param name="separator">
    ///   The seperator to write between values in the list. </param>
    /// <param name="splittable">
    ///   Indicates if the parameters can be split across messages written. </param>
    public void AddList(IList value, string separator, bool splittable) {
      this.parameters.Add(value);
      this.listParams[(this.parameters.Count - 1).ToString(CultureInfo.InvariantCulture)] = separator;
      if (splittable) {
        this.AddSplittableParameter();
      }
    }

    /// <summary>
    /// Adds a splittable list of parameters to the writer.
    /// </summary>
    /// <param name="value">The list of parameters to add</param>
    /// <param name="separator">The seperator to write between values in the list</param>
    public void AddList(IList value, string separator) {
      this.AddList(value, separator, true);
    }

    /// <summary>
    /// Writes the current message data to the inner writer in irc protocol format.
    /// </summary>
    public void Write() {
      //TODO Implement message splitting on IrcMessageWriter.Write
      if (this.writer == null) {
        this.writer = new StringWriter(CultureInfo.InvariantCulture);
      }

      if (sender != null && sender.Length != 0) {
        this.writer.Write(":");
        this.writer.Write(this.sender);
        this.writer.Write(" ");
      }


      int paramCount = this.parameters.Count;
      if (paramCount > 0) {
        for (int i = 0; i < paramCount - 1; i++) {
          writer.Write(this.GetParamValue(i));
          writer.Write(" ");
        }
        string lastParam = GetParamValue(paramCount - 1);
        if (lastParam.IndexOf(" ", StringComparison.Ordinal) > 0) {
          this.writer.Write(":");
        }
        this.writer.Write(lastParam);
      }
      if (this.AppendNewLine) {
        this.writer.Write(System.Environment.NewLine);
      }

      this.resetDefaults();
    }


    #endregion

    #region Helpers

    private void resetDefaults() {
      this.AppendNewLine = true;
      this.sender = null;
      this.parameters.Clear();
      this.listParams.Clear();
    }

    private ArrayList parameters = new ArrayList();
    private NameValueCollection listParams = new NameValueCollection();
    private NameValueCollection splitParams = new NameValueCollection();

    private void AddSplittableParameter() {
      splitParams[parameters.Count.ToString(CultureInfo.InvariantCulture)] = string.Empty;
    }

    private string GetParamValue(int index) {
      Object thisParam = this.parameters[index];

      StringCollection thisParamAsCollection = thisParam as StringCollection;
      if (thisParamAsCollection != null) {
        string seperator = this.listParams[index.ToString(CultureInfo.InvariantCulture)];
        return BigSister.Irc.Messages.MessageUtil.CreateList(thisParamAsCollection, seperator);
      }

      IList thisParamAsList = thisParam as IList;
      if (thisParamAsList != null) {
        string seperator = this.listParams[index.ToString(CultureInfo.InvariantCulture)];
        return BigSister.Irc.Messages.MessageUtil.CreateList(thisParamAsList, seperator);
      }

      return thisParam.ToString();
    }

    #endregion

    #region IDisposable Members

    private bool disposed;

    /// <summary>
    /// Implements IDisposable.Dispose
    /// </summary>
    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
      if (!this.disposed) {
        if (disposing) {
          if (this.writer != null) {
            this.writer.Dispose();
          }
        }
        disposed = true;
      }
    }

    /// <summary>
    /// The IrcMessageWriter destructor
    /// </summary>
    ~IrcMessageWriter() {
      Dispose(false);
    }

    #endregion

  }
}