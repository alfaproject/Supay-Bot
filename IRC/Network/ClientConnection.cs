using System;
using System.ComponentModel;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace BigSister.Irc.Network {
  /// <summary>
  ///   Represents a network connection to an irc server. </summary>
  /// <remarks>
  ///   Use the <see cref="ClientConnection"/> class to send a <see cref="BigSister.Irc.Messages.IrcMessage"/> to an irc server, and to be notified when it returns a <see cref="BigSister.Irc.Messages.IrcMessage"/>. </remarks>
  [System.ComponentModel.DesignerCategory("Code")]
  class ClientConnection : Component {

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientConnection"/> class.
    /// </summary>
    /// <remarks>With this Constructor, the <see cref="Address"/> default to 127.0.0.1, and the <see cref="Port"/> defaults to 6667.</remarks>
    public ClientConnection()
      : this("127.0.0.1", 6667) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientConnection"/> class with the given address on the given port.
    /// </summary>
    /// <param name="address">The network address to connect to.</param>
    /// <param name="port">The port to connect on.</param>
    public ClientConnection(string address, int port) {
      this.Encoding = System.Text.Encoding.Default;
      this.Ssl = false;
      Address = address;
      Port = port;
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the <see cref="ClientConnection"/> recieves data.
    /// </summary>
    internal event EventHandler<ConnectionDataEventArgs> DataReceived;

    /// <summary>
    /// Occurs when the <see cref="ClientConnection"/> sends data.
    /// </summary>
    internal event EventHandler<ConnectionDataEventArgs> DataSent;

    /// <summary>
    /// Occurs when starting the connecting sequence to a server
    /// </summary>
    public event EventHandler Connecting;

    /// <summary>
    /// Occurs after the connecting sequence is successful.
    /// </summary>
    public event EventHandler Connected;

    /// <summary>
    /// Occurs when the disconnecting sequence is successful.
    /// </summary>
    public event EventHandler<ConnectionDataEventArgs> Disconnected;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the internet address which the current <see cref="ClientConnection"/> uses.
    /// </summary>
    /// <remarks>A <see cref="NotSupportedException"/> will be thrown if an attempt is made to change the <see cref="ClientConnection.Address"/> if the <see cref="ClientConnection.Status"/> is not <see cref="ConnectionStatus.Disconnected"/>.</remarks>
    public string Address {
      get {
        return address;
      }
      set {
        if (this.Status == ConnectionStatus.Disconnected) {
          address = value;
        } else {
          throw new NotSupportedException("The Address cannot be changed while connected.");
        }
      }
    }

    /// <summary>
    /// Gets or sets the port which the <see cref="ClientConnection"/> will communicate over.
    /// </summary>
    /// <remarks>
    /// <para>For irc, the <see cref="Port"/> is generally between 6667 and 7000</para>
    /// <para>A <see cref="NotSupportedException"/> will be thrown if an attempt is made to change the <see cref="ClientConnection.Port"/> if the <see cref="ClientConnection.Status"/> is not <see cref="ConnectionStatus.Disconnected"/>.</para>
    /// </remarks>
    public int Port {
      get {
        return port;
      }
      set {
        if (this.Status == ConnectionStatus.Disconnected) {
          port = value;
        } else {
          throw new NotSupportedException("The Port cannot be changed while connected.");
        }
      }
    }

    /// <summary>
    /// Gets the <see cref="ConnectionStatus"/> of the <see cref="ClientConnection"/>.
    /// </summary>
    public ConnectionStatus Status {
      get {
        return status;
      }
      private set {
        this.status = value;
      }
    }

    /// <summary>
    ///   Gets or sets the <see cref="ISynchronizeInvoke"/> implementor which will be used to synchronize threads and events. </summary>
    /// <remarks>
    ///   This is usually the main form of the application. </remarks>
    public System.ComponentModel.ISynchronizeInvoke SynchronizationObject {
      get {
        return synchronizationObject;
      }
      set {
        synchronizationObject = value;
      }
    }

    /// <summary>
    /// Gets or sets the encoding used by stream reader and writer.
    /// </summary>
    /// <remarks>
    /// Generally, only ASCII and UTF-8 are supported.
    /// </remarks>
    public System.Text.Encoding Encoding {
      get {
        return _encoding;
      }
      set {
        if (this.Status == ConnectionStatus.Disconnected) {
          _encoding = value;
        } else {
          throw new NotSupportedException("The Encoding cannot be changed while connected.");
        }
      }
    }

    /// <summary>
    ///   Gets or sets if the connection will use SSL to connect to the server. </summary>
    public bool Ssl {
      get {
        return _ssl;
      }
      set {
        if (this.Status == ConnectionStatus.Disconnected) {
          _ssl = value;
        } else {
          throw new NotSupportedException("The Ssl property cannot be changed while connected.");
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a network connection to the current <see cref="ClientConnection.Address"/> and <see cref="ClientConnection.Port"/>
    /// </summary>
    /// <remarks>
    /// Only use this overload if your application is not a Windows.Forms application, you've set the <see cref="SynchronizationObject"/> property, or you want to handle threading issues yourself.
    /// </remarks>
    public virtual void Connect() {
      lock (lockObject) {
        if (this.Status != ConnectionStatus.Disconnected) {
          throw new InvalidOperationException("Already Connected To Another Server.");
        }

        this.Status = ConnectionStatus.Connecting;
        this.OnConnecting(EventArgs.Empty);
      }

      connectionWorker = new Thread(new ThreadStart(ReceiveData));
      connectionWorker.IsBackground = true;
      connectionWorker.Start();
    }

    /// <summary>
    /// Creates a network connection to the current <see cref="ClientConnection.Address"/> and <see cref="ClientConnection.Port"/>
    /// </summary>
    /// <remarks>
    /// <p>When using this class from an application, 
    /// you need to pass in a control so that data-receiving thread can sync with your application.</p>
    /// <p>If calling this from a form or other control, just pass in the current instance.</p>
    /// </remarks>
    /// <example>
    /// <code>
    /// [C#]
    /// client.Connection.Connect(this);
    /// 
    /// [VB]
    /// client.Connection.Connect(Me)
    /// </code>
    /// </example>
    public virtual void Connect(System.ComponentModel.ISynchronizeInvoke syncObject) {
      this.SynchronizationObject = syncObject;
      this.Connect();
    }

    /// <summary>
    /// Closes the current network connection.
    /// </summary>
    public virtual void Disconnect() {
      this.Status = ConnectionStatus.Disconnected;
    }

    /// <summary>
    /// Forces closing the current network connection and kills the thread running it.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public virtual void DisconnectForce() {
      this.Disconnect();
      if (connectionWorker != null) {
        try {
          connectionWorker.Abort();
        } catch {
        }
      }
    }

    /// <summary>
    /// Sends the given string over the network
    /// </summary>
    /// <param name="data">The <see cref="System.string"/> to send.</param>
    public virtual void Write(string data) {
      if (this.chatWriter == null || this.chatWriter.BaseStream == null || !this.chatWriter.BaseStream.CanWrite) {
        throw new InvalidOperationException("The connection can not be written to yet. Wait for the Connected event.");
      }

      if (data == null) {
        return;
      }

      data = data.Replace("\\c", "\x0003").Replace("\\b", "\x0002").Replace("\\u", "\x001F");

      if (!data.EndsWith("\r\n", StringComparison.Ordinal)) {
        data += "\r\n";
      }
      //if (data.Length > 512) {
      //  throw new BigSister.Irc.Messages.InvalidMessageException("Messages are limited to 512 bytes.", data);
      //}

      try {
        this.chatWriter.WriteLine(data);
        chatWriter.Flush();
        OnDataSent(new ConnectionDataEventArgs(data));
      } catch (Exception ex) {
        System.Diagnostics.Trace.WriteLine("Couldn't Send '" + data + "'. " + ex.ToString());
        throw;
      }
    }

    /// <summary>
    /// Releases the resources used by the <see cref="ClientConnection"/>
    /// </summary>
    protected override void Dispose(bool disposing) {
      try {
        if (disposing) {
          if (chatClient != null) {
            ((IDisposable)chatClient).Dispose();
          }
          if (chatReader != null) {
            ((IDisposable)chatReader).Dispose();
          }
          if (chatWriter != null) {
            ((IDisposable)chatWriter).Dispose();
          }
        }
      } finally {
        base.Dispose(disposing);
      }
    }

    #endregion

    #region Protected Event Raisers

    /// <summary>
    /// Raises the <see cref="ClientConnection.Connecting"/> event of the <see cref="ClientConnection"/> object.
    /// </summary>
    protected virtual void OnConnecting(EventArgs e) {
      if (Connecting != null) {
        Connecting(this, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="ClientConnection.Connected"/> event of the <see cref="ClientConnection"/> object.
    /// </summary>
    protected virtual void OnConnected(EventArgs e) {
      if (this.synchronizationObject != null && this.synchronizationObject.InvokeRequired) {
        SyncInvoke del = delegate {
          this.OnConnected(e);
        };
        this.synchronizationObject.Invoke(del, null);
        return;
      }

      if (Connected != null) {
        Connected(this, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="ClientConnection.DataReceived"/> event of the <see cref="ClientConnection"/> object.
    /// </summary>
    /// <param name="e">A <see cref="ConnectionDataEventArgs"/> that contains the data.</param>
    protected virtual void OnDataReceived(ConnectionDataEventArgs e) {
      if (this.synchronizationObject != null && this.synchronizationObject.InvokeRequired) {
        SyncInvoke del = delegate {
          this.OnDataReceived(e);
        };
        this.synchronizationObject.Invoke(del, null);
        return;
      }

      if (DataReceived != null) {
        DataReceived(this, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="ClientConnection.DataSent"/> event of the <see cref="ClientConnection"/> object.
    /// </summary>
    /// <param name="data">A <see cref="ConnectionDataEventArgs"/> that contains the data.</param>
    protected virtual void OnDataSent(ConnectionDataEventArgs data) {
      if (DataSent != null) {
        DataSent(this, data);
      }
    }

    /// <summary>
    /// Raises the <see cref="ClientConnection.Disconnected"/> event of the <see cref="ClientConnection"/> object.
    /// </summary>
    protected virtual void OnDisconnected(ConnectionDataEventArgs e) {
      if (this.synchronizationObject != null && this.synchronizationObject.InvokeRequired) {
        SyncInvoke del = delegate {
          this.OnDisconnected(e);
        };
        this.synchronizationObject.Invoke(del, null);
        return;
      }

      if (Disconnected != null) {
        Disconnected(this, e);
      }
    }

    #endregion

    #region Private

    private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
      if (sslPolicyErrors == SslPolicyErrors.None) {
        return true;
      }

      // Do not allow this client to communicate with unauthenticated servers.
      return false;
    }

    /// <summary>
    /// This method listens for data over the network until the Connection.State is Disconnected.
    /// </summary>
    /// <remarks>
    /// ReceiveData runs in its own thread.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private void ReceiveData() {

      try {
        chatClient = new TcpClient(Address, Port);
        Stream dataStream = null;
        if (this.Ssl) {
          dataStream = new SslStream(chatClient.GetStream(), false, ValidateServerCertificate, null);
          ((SslStream)dataStream).AuthenticateAsClient(this.Address);
        } else {
          dataStream = chatClient.GetStream();
        }


        chatReader = new StreamReader(dataStream, this.Encoding);
        chatWriter = new StreamWriter(dataStream, this.Encoding);
        chatWriter.AutoFlush = true;
      } catch (AuthenticationException e) {
        if (chatClient != null) {
          chatClient.Close();
        }
        this.Status = ConnectionStatus.Disconnected;
        this.OnDisconnected(new ConnectionDataEventArgs(e.Message));
        return;
      } catch (Exception ex) {
        this.Status = ConnectionStatus.Disconnected;
        this.OnDisconnected(new ConnectionDataEventArgs(ex.Message));
        return;
      }

      this.Status = ConnectionStatus.Connected;
      this.OnConnected(EventArgs.Empty);

      string disconnectReason = string.Empty;

      try {
        string incomingMessageLine;

        while (Status == ConnectionStatus.Connected && ((incomingMessageLine = chatReader.ReadLine()) != null)) {
          try {
            incomingMessageLine = incomingMessageLine.Trim();
            this.OnDataReceived(new ConnectionDataEventArgs(incomingMessageLine));
          } catch (ThreadAbortException ex) {
            System.Diagnostics.Trace.WriteLine(ex.Message);
            Thread.ResetAbort();
            disconnectReason = "Thread Aborted";
            break;
          }
        }
      } catch (Exception ex) {
        System.Diagnostics.Trace.WriteLine(ex.ToString());
        disconnectReason = ex.Message;
      }
      this.Status = ConnectionStatus.Disconnected;

      if (chatClient != null) {
        chatClient.Close();
        chatClient = null;
      }

      ConnectionDataEventArgs disconnectArgs = new ConnectionDataEventArgs(disconnectReason);
      this.OnDisconnected(disconnectArgs);
    }

    private Object lockObject = new Object();

    private string address;
    private int port;
    private ConnectionStatus status = ConnectionStatus.Disconnected;
    private System.Text.Encoding _encoding;
    private bool _ssl;

    private TcpClient chatClient;
    private StreamReader chatReader;
    private StreamWriter chatWriter;
    private Thread connectionWorker;

    private System.ComponentModel.ISynchronizeInvoke synchronizationObject;
    private delegate void SyncInvoke();

    #endregion

  }
}