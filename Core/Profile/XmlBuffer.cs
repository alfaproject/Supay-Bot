using System;
using System.IO;
using System.Security;
using System.Xml;

namespace BigSister {
  /// <summary>
  ///   Buffer class for <see cref="XmlProfile" /> class. </summary>
  /// <remarks>
  ///   This class provides buffering functionality for <see cref="XmlProfile" /> class.
  ///   <i>Buffering</i> refers to the caching of an <see cref="XmlDocument" /> object so that subsequent reads or writes
  ///   are all done through it.  This dramatically increases the performance of those operations, but it requires
  ///   that the buffer is flushed (or closed) to commit any changes done to the underlying file. 
  ///   <para>
  ///   Since an XmlProfile object can only have one buffer attached to it at a time, this class may not
  ///   be instanciated directly.  Instead, use the <see cref="XmlProfile.Buffer(bool)" /> method of the profile object. </para></remarks>
  /// <seealso cref="XmlProfile.Buffer(bool)" />
  class XmlBuffer : IDisposable {

    private XmlProfile _profile;
    private XmlDocument _doc;
    private FileStream _file;
    internal bool _needsFlushing;

    /// <summary>
    ///   Initializes a new instance of the XmlBuffer class and optionally locks the file. </summary>
    /// <param name="profile">
    ///   The XmlProfile object to associate with the buffer and to assign this object to. </param>
    /// <param name="lockFile">
    ///   If true and the file exists, the file is locked to prevent other processes from writing to it
    ///   until the buffer is closed. </param>
    /// <exception cref="InvalidOperationException">
    ///	  Attempting to lock the file  and the name is null or empty. </exception>
    /// <exception cref="SecurityException">
    ///	  Attempting to lock the file without the required permission. </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///	  Attempting to lock the file and ReadWrite access is not permitted by the operating system. </exception>
    internal XmlBuffer(XmlProfile profile, bool lockFile) {
      _profile = profile;

      if (lockFile) {
        _profile.VerifyName();
        if (File.Exists(_profile.Name))
          _file = new FileStream(_profile.Name, FileMode.Open, _profile.ReadOnly ? FileAccess.Read : FileAccess.ReadWrite, FileShare.Read);
      }
    }

    /// <summary>
    ///   Loads the XmlDocument object with the contents of an XmlTextWriter object. </summary>
    /// <param name="writer">
    ///   The XmlTextWriter object to load the XmlDocument with. </param>
    /// <remarks>
    ///   This method is used to load the buffer with new data. </remarks>
    internal void Load(XmlTextWriter writer) {
      writer.Flush();
      writer.BaseStream.Position = 0;
      _doc.Load(writer.BaseStream);

      _needsFlushing = true;
    }

    /// <summary>
    ///   Gets the XmlDocument object associated with this buffer, based on the profile's Name. </summary>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="XmlException">
    ///	  Parse error in the XML being loaded from the file. </exception>
    internal XmlDocument XmlDocument {
      get {
        if (_doc == null) {
          _doc = new XmlDocument();

          if (_file != null) {
            _file.Position = 0;
            _doc.Load(_file);
          } else {
            _profile.VerifyName();
            if (File.Exists(_profile.Name))
              _doc.Load(_profile.Name);
          }
        }
        return _doc;
      }
    }

    /// <summary>
    ///   Gets whether the buffer's XmlDocument object is empty. </summary>
    internal bool IsEmpty {
      get {
        return string.IsNullOrEmpty(XmlDocument.InnerXml);
      }
    }

    /// <summary>
    ///   Gets whether changes have been made to the XmlDocument object that require
    ///   the buffer to be flushed so that the file gets updated. </summary>
    /// <remarks>
    ///   This property returns true when the XmlDocument object has been changed and the 
    ///   <see cref="Flush" /> (or <see cref="Close" />) method needs to be called to 
    ///   update the file. </remarks>
    /// <seealso cref="Flush" />
    /// <seealso cref="Close" />
    public bool NeedsFlushing {
      get {
        return _needsFlushing;
      }
    }

    /// <summary>
    ///   Gets whether the file associated with the buffer's profile is locked. </summary>
    /// <remarks>
    ///   This property returns true when this object has been created with the <i>lockFile</i> parameter set to true,
    ///   provided the file exists.  When locked, other processes will not be allowed to write to the profile's
    ///   file until the buffer is closed. </remarks>
    /// <seealso cref="Close" />
    public bool Locked {
      get {
        return _file != null;
      }
    }

    /// <summary>
    ///   Writes the contents of the XmlDocument object to the file associated with this buffer's profile. </summary>
    /// <remarks>
    ///   This method may be used to explictly commit any changes made to the <see cref="XmlProfile" /> profile from the time 
    ///   the buffer was last flushed or created.  It writes the contents of the XmlDocument object to the profile's file.
    ///   When the buffer is being closed (with <see cref="Close" /> or <see cref="Dispose" />) this method is 
    ///   called if <see cref="NeedsFlushing" /> is true. After the buffer is closed, this method may not be called. </remarks>
    /// <exception cref="InvalidOperationException">
    ///   This object is closed. </exception>
    /// <seealso cref="Close" />
    /// <seealso cref="Reset" />
    public void Flush() {
      if (_profile == null)
        throw new InvalidOperationException("Cannot flush an XmlBuffer object that has been closed.");

      if (_doc == null)
        return;

      if (_file == null)
        _doc.Save(_profile.Name);
      else {
        _file.SetLength(0);
        _doc.Save(_file);
      }

      _needsFlushing = false;
    }

    /// <summary>
    ///   Resets the buffer by discarding its XmlDocument object. </summary>
    /// <remarks>
    ///   This method may be used to rollback any changes made to the <see cref="XmlProfile" /> profile from the time 
    ///   the buffer was last flushed or created. After the buffer is closed, this method may not be called. </remarks>
    /// <exception cref="InvalidOperationException">
    ///   This object is closed. </exception>
    /// <seealso cref="Flush" />
    /// <seealso cref="Close" />
    public void Reset() {
      if (_profile == null)
        throw new InvalidOperationException("Cannot reset an XmlBuffer object that has been closed.");

      _doc = null;
      _needsFlushing = false;
    }

    /// <summary>
    ///   Closes the buffer by flushing the contents of its XmlDocument object (if necessary) and dettaching itself 
    ///   from its <see cref="XmlProfile" /> profile. </summary>
    /// <remarks>
    ///   This method may be used to explictly deactivate the <see cref="XmlProfile" /> profile buffer. 
    ///   This means that the buffer is flushed (if <see cref="NeedsFlushing" /> is true) and it gets 
    ///   dettached from the profile. The <see cref="Dispose" /> method automatically calls this method. </remarks>
    /// <seealso cref="Flush" />
    /// <seealso cref="Dispose" />
    public void Close() {
      if (_profile == null)
        return;

      if (_needsFlushing)
        Flush();

      _doc = null;

      if (_file != null) {
        _file.Close();
        _file = null;
      }

      if (_profile != null)
        _profile._buffer = null;
      _profile = null;
    }

    /// <summary>
    ///   Disposes of this object's resources by closing the buffer. </summary>
    /// <remarks>
    ///   This method calls <see cref="Close" />, which flushes the buffer and dettaches it from the profile. </remarks>
    /// <seealso cref="Close" />
    /// <seealso cref="Flush" />
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (disposing)
        Close();
    }

  } //class XmlBuffer
} //namespace BigSister