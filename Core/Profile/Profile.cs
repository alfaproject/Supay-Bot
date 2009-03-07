using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace BigSister {
  /// <summary>
  ///   Profile class that utilizes an XML file to retrieve and save its data. </summary>
  /// <remarks>
  ///   This class works with XML files, which are text files that store their data using XML. 
  ///   Since the format of XML is very flexible, I had to decide how to best organize the data
  ///   using the section/entry paradigm.  After considering a couple of possibilities, 
  ///   I decided that the format below would be preferrable, since it allows section and 
  ///   entry names to contain spaces.  It also looks cleaner and more consistent than if I had
  ///   used the section and entry names themselves to name the elements.
  ///   <para>
  ///   Here's an illustration of the format: </para>
  ///   <code>
  ///   &lt;?xml version="1.0" encoding="utf-8"?&gt;
  ///   &lt;profile&gt;
  ///     &lt;section name="A Section"&gt;
  ///       &lt;entry name="An Entry"&gt;Some Value&lt;/entry&gt;
  ///       &lt;entry name="Another Entry"&gt;Another Value&lt;/entry&gt;
  ///     &lt;/section&gt;
  ///     &lt;section name="Another Section"&gt;
  ///       &lt;entry name="This is cool"&gt;True&lt;/entry&gt;
  ///     &lt;/section&gt;
  ///   &lt;/profile&gt;
  ///   </code></remarks>
  public class XmlProfile : IDisposable {

    // Fields
    private string _name;
    private bool _readOnly;
    private string _rootName = "profile";
    private Encoding _encoding = Encoding.UTF8;
    internal XmlBuffer _buffer;

    /// <summary>
    ///   Initializes a new instance of the XmlProfile class by setting the <see cref="Name" /> to <see cref="DefaultName" />. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected XmlProfile() {
      _name = this.DefaultName;
    }

    /// <summary>
    ///   Initializes a new instance of the XmlProfile class by setting the <see cref="Profile.Name" /> to the given file name. </summary>
    /// <param name="fileName">
    ///   The name of the file to initialize the <see cref="Profile.Name" /> property with. </param>
    public XmlProfile(string fileName) {
      _name = fileName;
    }

    /// <summary>
    ///   Initializes a new instance of the XmlProfile class based on another XmlProfile object. </summary>
    /// <param name="profile">
    ///   The XmlProfile object whose properties and events are used to initialize the object being constructed. </param>
    protected XmlProfile(XmlProfile profile) {
      _name = profile._name;
      _readOnly = profile._readOnly;
      _rootName = profile._rootName;
      _encoding = profile._encoding;
    }

    /// <summary>
    ///   Retrieves an XmlDocument object based on the <see cref="Profile.Name" /> of the file. </summary>
    /// <returns>
    ///   If <see cref="Buffering" /> is not enabled, the return value is the XmlDocument object loaded with the file, 
    ///   or null if the file does not exist. If <see cref="Buffering" /> is enabled, the return value is an 
    ///   XmlDocument object, which will be loaded with the file if it already exists.</returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="XmlException">
    ///	  Parse error in the XML being loaded from the file. </exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    protected XmlDocument GetXmlDocument() {
      if (_buffer != null)
        return _buffer.XmlDocument;

      VerifyName();
      if (!File.Exists(Name))
        return null;

      XmlDocument doc = new XmlDocument();
      doc.Load(Name);
      return doc;
    }

    /// <summary>
    ///   Saves any changes pending on an XmlDocument object, unless <see cref="Buffering" /> is enabled. </summary>
    /// <exception cref="XmlException">
    ///	  The resulting XML document would not be well formed. </exception>
    /// <remarks>
    ///   If <see cref="Buffering" /> is enabled, this method sets the <see cref="XmlBuffer.NeedsFlushing" /> property to true 
    ///   and the changes are not saved until the buffer is flushed (or closed).  If the Buffer is not active
    ///   the contents of the XmlDocument object are saved to the file. </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
    protected void Save(XmlDocument doc) {
      if (_buffer != null)
        _buffer._needsFlushing = true;
      else
        doc.Save(Name);
    }

    /// <summary>
    ///   Activates buffering on this XML-based profile object, if not already active. </summary>
    /// <param name="lockFile">
    ///   If true, the file is locked when the buffer is activated so that no other processes can write to it.  
    ///   If false, other processes can continue writing to it and the actual contents of the file can get 
    ///   out of synch with the contents of the buffer. </param>
    /// <returns>
    ///   The return value is an <see cref="XmlBuffer" /> object that may be used to control the buffer used
    ///   to read/write values from this profile.  </returns>
    /// <exception cref="InvalidOperationException">
    ///	  Attempting to lock the file  and the name is null or empty. </exception>
    /// <exception cref="SecurityException">
    ///	  Attempting to lock the file without the required permission. </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///	  Attempting to lock the file and ReadWrite access is not permitted by the operating system. </exception>
    /// <remarks>
    ///   <i>Buffering</i> is the caching of an <see cref="XmlDocument" /> object so that subsequent reads or writes
    ///   are all done through it.  This dramatically increases the performance of those operations, but it requires
    ///   that the buffer is flushed (or closed) to commit any changes done to the underlying file.
    ///   <para>
    ///   The XmlBuffer object is created and attached to this profile object, if not already present.
    ///   If it is already attached, the same object is returned in subsequent calls, until the object is closed. </para>
    ///   <para>
    ///   Since the XmlBuffer class implements <see cref="IDisposable" />, the <c>using</c> keyword in C# can be 
    ///   used to conveniently create the buffer, write to it, and then automatically flush it (when it's disposed).  
    ///   Here's an example:
    ///   <code> 
    ///   using (profile.Buffer(true)) {
    ///      profile.SetValue("A Section", "An Entry", "A Value");
    ///      profile.SetValue("A Section", "Another Entry", "Another Value");
    ///      ...
    ///   }
    ///   </code></para></remarks>
    /// <seealso cref="XmlBuffer" />
    /// <seealso cref="Buffering" />
    public XmlBuffer Buffer(bool lockFile) {
      if (_buffer == null)
        _buffer = new XmlBuffer(this, lockFile);
      return _buffer;
    }

    /// <summary>
    ///   Activates <i>locked</i> buffering on this XML-based profile object, if not already active. </summary>
    /// <returns>
    ///   The return value is an <see cref="XmlBuffer" /> object that may be used to control the buffer used
    ///   to read/write values from this profile.  </returns>
    /// <exception cref="InvalidOperationException">
    ///	  Attempting to lock the file  and the name is null or empty. </exception>
    /// <exception cref="SecurityException">
    ///	  Attempting to lock the file without the required permission. </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///	  Attempting to lock the file and ReadWrite access is not permitted by the operating system. </exception>
    /// <remarks>
    ///   <i>Buffering</i> refers to the caching of an <see cref="XmlDocument" /> object so that subsequent reads or writes
    ///   are all done through it.  This dramatically increases the performance of those operations, but it requires
    ///   that the buffer is flushed (or closed) to commit any changes done to the underlying file.
    ///   <para>
    ///   The XmlBuffer object is created and attached to this profile object, if not already present.
    ///   If it is already attached, the same object is returned in subsequent calls, until the object is closed. </para>
    ///   <para>
    ///   If the buffer is created, the underlying file (if any) is locked so that no other processes 
    ///   can write to it. This is equivalent to calling Buffer(true). </para>
    ///   <para>
    ///   Since the XmlBuffer class implements <see cref="IDisposable" />, the <c>using</c> keyword in C# can be 
    ///   used to conveniently create the buffer, write to it, and then automatically flush it (when it's disposed).  
    ///   Here's an example:
    ///   <code> 
    ///   using (profile.Buffer()) {
    ///      profile.SetValue("A Section", "An Entry", "A Value");
    ///      profile.SetValue("A Section", "Another Entry", "Another Value");
    ///      ...
    ///   }
    ///   </code></para></remarks>
    /// <seealso cref="XmlBuffer" />
    /// <seealso cref="Buffering" />
    public XmlBuffer Buffer() {
      return Buffer(true);
    }

    /// <summary>
    ///   Gets whether buffering is active or not. </summary>
    /// <remarks>
    ///   <i>Buffering</i> is the caching of an <see cref="XmlDocument" /> object so that subsequent reads or writes
    ///   are all done through it.  This dramatically increases the performance of those operations, but it requires
    ///   that the buffer is flushed (or closed) to commit any changes done to the underlying file.
    ///   <para>
    ///   This property may be used to determine if the buffer is active without actually activating it.  
    ///   The <see cref="Buffer(bool)" /> method activates the buffer, which then needs to be flushed (or closed) to update the file. </para></remarks>
    /// <seealso cref="Buffer(bool)" />
    /// <seealso cref="XmlBuffer" />
    public bool Buffering {
      get {
        return _buffer != null;
      }
    }

    /// <summary>
    ///   Gets or sets the encoding, to be used if the file is created. </summary>
    /// <exception cref="InvalidOperationException">
    ///   Setting this property if <see cref="Profile.ReadOnly" /> is true. </exception>
    /// <remarks>
    ///   By default this property is set to <see cref="System.Text.Encoding.UTF8">Encoding.UTF8</see>, but it is only 
    ///   used when the file is not found and needs to be created to write the value. 
    ///   If the file exists, the existing encoding is used and this value is ignored. 
    ///   The <see cref="Profile.Changing" /> event is raised before changing this property.  
    ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this method 
    ///   returns immediately without changing this property.  After the property has been changed, 
    ///   the <see cref="Profile.Changed" /> event is raised. </remarks>
    public Encoding Encoding {
      get {
        return _encoding;
      }
      set {
        VerifyNotReadOnly();
        if (_encoding == value)
          return;

        _encoding = value;
      }
    }

    /// <summary>
    ///   Retrieves the XPath string used for retrieving a section from the XML file. </summary>
    /// <returns>
    ///   An XPath string. </returns>
    /// <seealso cref="GetEntryPath" />
    private static string GetSectionsPath(string section) {
      return "section[@name=\"" + section + "\"]";
    }

    /// <summary>
    ///   Retrieves the XPath string used for retrieving an entry from the XML file. </summary>
    /// <returns>
    ///   An XPath string. </returns>
    /// <seealso cref="GetSectionsPath" />
    private static string GetEntryPath(string entry) {
      return "entry[@name=\"" + entry + "\"]";
    }

    /// <summary>
    ///   Gets or sets the name associated with the profile. </summary>
    /// <exception cref="NullReferenceException">
    ///   Setting this property to null. </exception>
    /// <exception cref="InvalidOperationException">
    ///   Setting this property if ReadOnly is true. </exception>
    /// <remarks>
    ///   This is usually the name of the file where the data is stored. 
    ///   The <see cref="Changing" /> event is raised before changing this property.  
    ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this property 
    ///   returns immediately without being changed.  After the property is changed, 
    ///   the <see cref="Changed" /> event is raised. </remarks>
    /// <seealso cref="DefaultName" />
    public string Name {
      get {
        return _name;
      }
      set {
        VerifyNotReadOnly();
        if (_name == value.Trim())
          return;

        _name = value.Trim();
      }
    }

    /// <summary>
    ///   Gets or sets the name of the root element, to be used if the file is created. </summary>
    /// <exception cref="InvalidOperationException">
    ///   Setting this property if <see cref="Profile.ReadOnly" /> is true. </exception>
    /// <exception cref="NullReferenceException">
    ///   Setting this property to null. </exception>
    /// <remarks>
    ///   By default this property is set to "profile", but it is only used when the file 
    ///   is not found and needs to be created to write the value. 
    ///   If the file exists, the name of the root element inside the file is ignored. 
    ///   The <see cref="Profile.Changing" /> event is raised before changing this property.  
    ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this method 
    ///   returns immediately without changing this property.  After the property has been changed, 
    ///   the <see cref="Profile.Changed" /> event is raised. </remarks>
    public string RootName {
      get {
        return _rootName;
      }
      set {
        VerifyNotReadOnly();
        if (_rootName == value.Trim())
          return;

        _rootName = value.Trim();
      }
    }

    /// <summary>
    ///   Gets or sets whether the profile is read-only or not. </summary>
    /// <exception cref="InvalidOperationException">
    ///   Setting this property if it's already true. </exception>
    /// <remarks>
    ///   A read-only profile does not allow any operations that alter sections,
    ///   entries, or values, such as <see cref="SetValue" /> or <see cref="RemoveEntry" />.  
    ///   Once a profile has been marked read-only, it may no longer go back; 
    ///   attempting to do so causes an InvalidOperationException to be raised.
    ///   The <see cref="Changing" /> event is raised before changing this property.  
    ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this property 
    ///   returns immediately without being changed.  After the property is changed, 
    ///   the <see cref="Changed" /> event is raised. </remarks>
    /// <seealso cref="CloneReadOnly" />
    /// <seealso cref="IReadOnlyProfile" />
    public bool ReadOnly {
      get {
        return _readOnly;
      }

      set {
        VerifyNotReadOnly();
        if (_readOnly == value)
          return;

        _readOnly = value;
      }
    }

    /// <summary>
    ///   Gets the default name for the XML file. </summary>
    /// <remarks>
    ///   For Windows apps, this property returns the name of the executable plus .xml ("program.exe.xml").
    ///   For Web apps, this property returns the full path of <i>web.xml</i> based on the root folder.
    ///   This property is used to set the <see cref="Profile.Name" /> property inside the default constructor.</remarks>
    public string DefaultName {
      get {
        return DefaultNameWithoutExtension + ".xml";
      }
    }

    /// <summary>
    ///   Retrieves a copy of itself. </summary>
    /// <returns>
    ///   The return value is a copy of itself as an object. </returns>
    /// <seealso cref="Profile.CloneReadOnly" />
    public object Clone() {
      return new XmlProfile(this);
    }

    /// <summary>
    ///   Sets the value for an entry inside a section. </summary>
    /// <param name="section">
    ///   The name of the section that holds the entry. </param>
    /// <param name="entry">
    ///   The name of the entry where the value will be set. </param>
    /// <param name="value">
    ///   The value to set. If it's null, the entry is removed. </param>
    /// <exception cref="InvalidOperationException">
    ///   <see cref="Profile.Name" /> is null or empty, 
    ///   <see cref="Profile.ReadOnly" /> is true, or
    ///   the resulting XML document is invalid. </exception>
    /// <exception cref="ArgumentNullException">
    ///   Either section or entry is null. </exception>
    /// <exception cref="XmlException">
    ///	  Parse error in the XML being loaded from the file or
    ///	  the resulting XML document would not be well formed. </exception>
    /// <remarks>
    ///   If the XML file does not exist, it is created, unless <see cref="XmlProfile.Buffering" /> is enabled.
    ///   The <see cref="Profile.Changing" /> event is raised before setting the value.  
    ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this method 
    ///   returns immediately without setting the value.  After the value has been set, 
    ///   the <see cref="Profile.Changed" /> event is raised. 
    ///   <para>
    ///   Note: If <see cref="XmlProfile.Buffering" /> is enabled, the value is not actually written to the
    ///   XML file until the buffer is flushed (or closed). </para></remarks>
    /// <seealso cref="GetValue" />
    public void SetValue(string section, string entry, object value) {
      // If the value is null, remove the entry
      if (value == null) {
        RemoveEntry(section, entry);
        return;
      }

      VerifyNotReadOnly();
      VerifyName();
      VerifyAndAdjustSection(ref section);
      VerifyAndAdjustEntry(ref entry);

      string valueString = value.ToString();

      // If the file does not exist, use the writer to quickly create it
      if ((_buffer == null || _buffer.IsEmpty) && !File.Exists(Name)) {
        XmlTextWriter writer = null;

        // If there's a buffer, write to it without creating the file
        if (_buffer == null)
          writer = new XmlTextWriter(Name, Encoding);
        else
          writer = new XmlTextWriter(new MemoryStream(), Encoding);

        writer.Formatting = Formatting.Indented;

        writer.WriteStartDocument();

        writer.WriteStartElement(_rootName);
        writer.WriteStartElement("section");
        writer.WriteAttributeString("name", null, section);
        writer.WriteStartElement("entry");
        writer.WriteAttributeString("name", null, entry);
        writer.WriteString(valueString);
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();

        if (_buffer != null)
          _buffer.Load(writer);
        writer.Close();

        return;
      }

      // The file exists, edit it

      XmlDocument doc = GetXmlDocument();
      XmlElement root = doc.DocumentElement;

      // Get the section element and add it if it's not there
      XmlNode sectionNode = root.SelectSingleNode(GetSectionsPath(section));
      if (sectionNode == null) {
        XmlElement element = doc.CreateElement("section");
        XmlAttribute attribute = doc.CreateAttribute("name");
        attribute.Value = section;
        element.Attributes.Append(attribute);
        sectionNode = root.AppendChild(element);
      }

      // Get the entry element and add it if it's not there
      XmlNode entryNode = sectionNode.SelectSingleNode(GetEntryPath(entry));
      if (entryNode == null) {
        XmlElement element = doc.CreateElement("entry");
        XmlAttribute attribute = doc.CreateAttribute("name");
        attribute.Value = entry;
        element.Attributes.Append(attribute);
        entryNode = sectionNode.AppendChild(element);
      }

      // Add the value and save the file
      entryNode.InnerText = valueString;
      Save(doc);
    }

    /// <summary>
    ///   Retrieves the value of an entry inside a section. </summary>
    /// <param name="section">
    ///   The name of the section that holds the entry with the value. </param>
    /// <param name="entry">
    ///   The name of the entry where the value is stored. </param>
    /// <returns>
    ///   The return value is the entry's value, or null if the entry does not exist. </returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="ArgumentNullException">
    ///   Either section or entry is null. </exception>
    /// <exception cref="XmlException">
    ///	  Parse error in the XML being loaded from the file. </exception>
    /// <seealso cref="SetValue" />
    /// <seealso cref="Profile.HasEntry" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public object GetValue(string section, string entry) {
      VerifyAndAdjustSection(ref section);
      VerifyAndAdjustEntry(ref entry);

      try {
        XmlDocument doc = GetXmlDocument();
        XmlElement root = doc.DocumentElement;

        XmlNode entryNode = root.SelectSingleNode(GetSectionsPath(section) + "/" + GetEntryPath(entry));
        return entryNode.InnerText;
      } catch {
        return null;
      }
    }

    /// <summary>
    ///   Retrieves the string value of an entry inside a section, or a default value if the entry does not exist. </summary>
    /// <param name="section">
    ///   The name of the section that holds the entry with the value. </param>
    /// <param name="entry">
    ///   The name of the entry where the value is stored. </param>
    /// <param name="defaultValue">
    ///   The value to return if the entry (or section) does not exist. </param>
    /// <returns>
    ///   The return value is the entry's value converted to a string, or the given default value if the entry does not exist. </returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="ArgumentNullException">
    ///   Either section or entry is null. </exception>
    /// <remarks>
    ///   This method calls <c>GetValue(section, entry)</c> of the derived class, so check its 
    ///   documentation to see what other exceptions may be raised. </remarks>
    /// <seealso cref="SetValue" />
    /// <seealso cref="HasEntry" />
    public virtual string GetValue(string section, string entry, string defaultValue) {
      object value = GetValue(section, entry);
      return (value == null ? defaultValue : value.ToString());
    }

    /// <summary>
    ///   Retrieves the integer value of an entry inside a section, or a default value if the entry does not exist. </summary>
    /// <param name="section">
    ///   The name of the section that holds the entry with the value. </param>
    /// <param name="entry">
    ///   The name of the entry where the value is stored. </param>
    /// <param name="defaultValue">
    ///   The value to return if the entry (or section) does not exist. </param>
    /// <returns>
    ///   The return value is the entry's value converted to an integer.  If the value
    ///   cannot be converted, the return value is 0.  If the entry does not exist, the
    ///   given default value is returned. </returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="ArgumentNullException">
    ///   Either section or entry is null. </exception>
    /// <remarks>
    ///   This method calls <c>GetValue(section, entry)</c> of the derived class, so check its 
    ///   documentation to see what other exceptions may be raised. </remarks>
    /// <seealso cref="SetValue" />
    /// <seealso cref="HasEntry" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public virtual int GetValue(string section, string entry, int defaultValue) {
      object value = GetValue(section, entry);
      if (value == null)
        return defaultValue;

      try {
        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
      } catch {
        return 0;
      }
    }

    /// <summary>
    ///   Retrieves the double value of an entry inside a section, or a default value if the entry does not exist. </summary>
    /// <param name="section">
    ///   The name of the section that holds the entry with the value. </param>
    /// <param name="entry">
    ///   The name of the entry where the value is stored. </param>
    /// <param name="defaultValue">
    ///   The value to return if the entry (or section) does not exist. </param>
    /// <returns>
    ///   The return value is the entry's value converted to a double.  If the value
    ///   cannot be converted, the return value is 0.  If the entry does not exist, the
    ///   given default value is returned. </returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="ArgumentNullException">
    ///   Either section or entry is null. </exception>
    /// <remarks>
    ///   This method calls <c>GetValue(section, entry)</c> of the derived class, so check its 
    ///   documentation to see what other exceptions may be raised. </remarks>
    /// <seealso cref="SetValue" />
    /// <seealso cref="HasEntry" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public virtual double GetValue(string section, string entry, double defaultValue) {
      object value = GetValue(section, entry);
      if (value == null)
        return defaultValue;

      try {
        return Convert.ToDouble(value, CultureInfo.InvariantCulture);
      } catch {
        return 0;
      }
    }

    /// <summary>
    ///   Retrieves the bool value of an entry inside a section, or a default value if the entry does not exist. </summary>
    /// <param name="section">
    ///   The name of the section that holds the entry with the value. </param>
    /// <param name="entry">
    ///   The name of the entry where the value is stored. </param>
    /// <param name="defaultValue">
    ///   The value to return if the entry (or section) does not exist. </param>
    /// <returns>
    ///   The return value is the entry's value converted to a bool.  If the value
    ///   cannot be converted, the return value is <c>false</c>.  If the entry does not exist, the
    ///   given default value is returned. </returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="ArgumentNullException">
    ///   Either section or entry is null. </exception>
    /// <remarks>
    ///   Note: Boolean values are stored as "True" or "False". 
    ///   <para>
    ///   This method calls <c>GetValue(section, entry)</c> of the derived class, so check its 
    ///   documentation to see what other exceptions may be raised. </para></remarks>
    /// <seealso cref="SetValue" />
    /// <seealso cref="HasEntry" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public virtual bool GetValue(string section, string entry, bool defaultValue) {
      object value = GetValue(section, entry);
      if (value == null)
        return defaultValue;

      try {
        return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
      } catch {
        return false;
      }
    }

    /// <summary>
    ///   Determines if an entry exists inside a section. </summary>
    /// <param name="section">
    ///   The name of the section that holds the entry. </param>
    /// <param name="entry">
    ///   The name of the entry to be checked for existence. </param>
    /// <returns>
    ///   If the entry exists inside the section, the return value is true; otherwise false. </returns>
    /// <exception cref="ArgumentNullException">
    ///   section is null. </exception>
    /// <remarks>
    ///   This method calls GetEntryNames of the derived class, so check its 
    ///   documentation to see what other exceptions may be raised. </remarks>
    /// <seealso cref="HasSection" />
    /// <seealso cref="GetEntryNames" />
    public virtual bool HasEntry(string section, string entry) {
      string[] entries = GetEntryNames(section);

      if (entries == null)
        return false;

      VerifyAndAdjustEntry(ref entry);
      return Array.IndexOf(entries, entry) >= 0;
    }

    /// <summary>
    ///   Determines if a section exists. </summary>
    /// <param name="section">
    ///   The name of the section to be checked for existence. </param>
    /// <returns>
    ///   If the section exists, the return value is true; otherwise false. </returns>
    /// <seealso cref="HasEntry" />
    /// <seealso cref="GetSectionNames" />
    public virtual bool HasSection(string section) {
      string[] sections = GetSectionNames();

      if (sections == null)
        return false;

      VerifyAndAdjustSection(ref section);
      return Array.IndexOf(sections, section) >= 0;
    }

    /// <summary>
    ///   Removes an entry from a section. </summary>
    /// <param name="section">
    ///   The name of the section that holds the entry. </param>
    /// <param name="entry">
    ///   The name of the entry to remove. </param>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty or
    ///   <see cref="Profile.ReadOnly" /> is true. </exception>
    /// <exception cref="ArgumentNullException">
    ///   Either section or entry is null. </exception>
    /// <exception cref="XmlException">
    ///	  Parse error in the XML being loaded from the file or
    ///	  the resulting XML document would not be well formed. </exception>
    /// <remarks>
    ///   The <see cref="Profile.Changing" /> event is raised before removing the entry.  
    ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this method 
    ///   returns immediately without removing the entry.  After the entry has been removed, 
    ///   the <see cref="Profile.Changed" /> event is raised.
    ///   <para>
    ///   Note: If <see cref="XmlProfile.Buffering" /> is enabled, the entry is not removed from the
    ///   XML file until the buffer is flushed (or closed). </para></remarks>
    /// <seealso cref="RemoveSection" />
    public void RemoveEntry(string section, string entry) {
      VerifyNotReadOnly();
      VerifyAndAdjustSection(ref section);
      VerifyAndAdjustEntry(ref entry);

      // Verify the document exists
      XmlDocument doc = GetXmlDocument();
      if (doc == null)
        return;

      // Get the entry's node, if it exists
      XmlElement root = doc.DocumentElement;
      XmlNode entryNode = root.SelectSingleNode(GetSectionsPath(section) + "/" + GetEntryPath(entry));
      if (entryNode == null)
        return;

      entryNode.ParentNode.RemoveChild(entryNode);
      Save(doc);
    }

    /// <summary>
    ///   Removes a section. </summary>
    /// <param name="section">
    ///   The name of the section to remove. </param>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty or
    ///   <see cref="Profile.ReadOnly" /> is true. </exception>
    /// <exception cref="ArgumentNullException">
    ///   section is null. </exception>
    /// <exception cref="XmlException">
    ///	  Parse error in the XML being loaded from the file or
    ///	  the resulting XML document would not be well formed. </exception>
    /// <remarks>
    ///   The <see cref="Profile.Changing" /> event is raised before removing the section.  
    ///   If its <see cref="ProfileChangingArgs.Cancel" /> property is set to true, this method 
    ///   returns immediately without removing the section.  After the section has been removed, 
    ///   the <see cref="Profile.Changed" /> event is raised.
    ///   <para>
    ///   Note: If <see cref="XmlProfile.Buffering" /> is enabled, the section is not removed from the
    ///   XML file until the buffer is flushed (or closed). </para></remarks>
    /// <seealso cref="RemoveEntry" />
    public void RemoveSection(string section) {
      VerifyNotReadOnly();
      VerifyAndAdjustSection(ref section);

      // Verify the document exists
      XmlDocument doc = GetXmlDocument();
      if (doc == null)
        return;

      // Get the root node, if it exists
      XmlElement root = doc.DocumentElement;
      if (root == null)
        return;

      // Get the section's node, if it exists
      XmlNode sectionNode = root.SelectSingleNode(GetSectionsPath(section));
      if (sectionNode == null)
        return;

      root.RemoveChild(sectionNode);
      Save(doc);
    }

    /// <summary>
    ///   Retrieves the names of all the entries inside a section. </summary>
    /// <param name="section">
    ///   The name of the section holding the entries. </param>
    /// <returns>
    ///   If the section exists, the return value is an array with the names of its entries; 
    ///   otherwise it's null. </returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="ArgumentNullException">
    ///   section is null. </exception>
    /// <exception cref="XmlException">
    ///	  Parse error in the XML being loaded from the file. </exception>
    /// <seealso cref="Profile.HasEntry" />
    /// <seealso cref="GetSectionNames" />
    public string[] GetEntryNames(string section) {
      // Verify the section exists
      if (!HasSection(section))
        return null;

      VerifyAndAdjustSection(ref section);

      XmlDocument doc = GetXmlDocument();
      XmlElement root = doc.DocumentElement;

      // Get the entry nodes
      XmlNodeList entryNodes = root.SelectNodes(GetSectionsPath(section) + "/entry[@name]");
      if (entryNodes == null)
        return null;

      // Add all entry names to the string array			
      string[] entries = new string[entryNodes.Count];
      int i = 0;

      foreach (XmlNode node in entryNodes)
        entries[i++] = node.Attributes["name"].Value;

      return entries;
    }

    /// <summary>
    ///   Retrieves the names of all the sections. </summary>
    /// <returns>
    ///   If the XML file exists, the return value is an array with the names of all the sections;
    ///   otherwise it's null. </returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="XmlException">
    ///	  Parse error in the XML being loaded from the file. </exception>
    /// <seealso cref="Profile.HasSection" />
    /// <seealso cref="GetEntryNames" />
    public string[] GetSectionNames() {
      // Verify the document exists
      XmlDocument doc = GetXmlDocument();
      if (doc == null)
        return null;

      // Get the root node, if it exists
      XmlElement root = doc.DocumentElement;
      if (root == null)
        return null;

      // Get the section nodes
      XmlNodeList sectionNodes = root.SelectNodes("section[@name]");
      if (sectionNodes == null)
        return null;

      // Add all section names to the string array			
      string[] sections = new string[sectionNodes.Count];
      int i = 0;

      foreach (XmlNode node in sectionNodes)
        sections[i++] = node.Attributes["name"].Value;

      return sections;
    }

    /// <summary>
    ///   Retrieves a copy of itself and makes it read-only. </summary>
    /// <returns>
    ///   The return value is a copy of itself as a IReadOnlyProfile object. </returns>
    /// <remarks>
    ///   This method serves as a convenient way to pass a read-only copy of the profile to methods 
    ///   that are not allowed to modify it. </remarks>
    /// <seealso cref="ReadOnly" />
    public virtual XmlProfile CloneReadOnly() {
      XmlProfile profile = (XmlProfile)Clone();
      profile._readOnly = true;

      return profile;
    }

    /// <summary>
    ///   Retrieves a DataSet object containing every section, entry, and value in the profile. </summary>
    /// <returns>
    ///   If the profile exists, the return value is a DataSet object representing the profile; otherwise it's null. </returns>
    /// <exception cref="InvalidOperationException">
    ///	  <see cref="Profile.Name" /> is null or empty. </exception>
    /// <remarks>
    ///   The returned DataSet will be named using the <see cref="Name" /> property.  
    ///   It will contain one table for each section, and each entry will be represented by a column inside the table.
    ///   Each table will contain only one row where the values will stored corresponding to each column (entry). 
    ///   <para>
    ///   This method serves as a convenient way to extract the profile's data to this generic medium known as the DataSet.  
    ///   This allows it to be moved to many different places, including a different type of profile object 
    ///   (eg., INI to XML conversion). </para>
    ///   <para>
    ///   This method calls GetSectionNames, GetEntryNames, and GetValue of the derived class, so check the 
    ///   documentation to see what other exceptions may be raised. </para></remarks>
    /// <seealso cref="SetDataSet" />
    public virtual DataSet GetDataSet() {
      VerifyName();

      string[] sections = GetSectionNames();
      if (sections == null)
        return null;

      DataSet ds = new DataSet(Name);
      ds.Locale = CultureInfo.InvariantCulture;

      // Add a table for each section
      foreach (string section in sections) {
        DataTable table = ds.Tables.Add(section);

        // Retrieve the column names and values
        string[] entries = GetEntryNames(section);
        DataColumn[] columns = new DataColumn[entries.Length];
        object[] values = new object[entries.Length];

        int i = 0;
        foreach (string entry in entries) {
          object value = GetValue(section, entry);

          columns[i] = new DataColumn(entry, value.GetType());
          values[i++] = value;
        }

        // Add the columns and values to the table
        table.Columns.AddRange(columns);
        table.Rows.Add(values);
      }

      return ds;
    }

    /// <summary>
    ///   Writes the data of every table from a DataSet into this profile. </summary>
    /// <param name="ds">
    ///   The DataSet object containing the data to be set. </param>
    /// <exception cref="InvalidOperationException">
    ///   <see cref="Profile.ReadOnly" /> is true or
    ///   <see cref="Profile.Name" /> is null or empty. </exception>
    /// <exception cref="ArgumentNullException">
    ///   ds is null. </exception>
    /// <remarks>
    ///   Each table in the DataSet represents a section of the profile.  
    ///   Each column of each table represents an entry.  And for each column, the corresponding value
    ///   of the first row is the value to be passed to <see cref="SetValue" />.  
    ///   Note that only the first row is imported; additional rows are ignored.
    ///   <para>
    ///   This method serves as a convenient way to take any data inside a generic DataSet and 
    ///   write it to any of the available profiles. </para>
    ///   <para>
    ///   This method calls SetValue of the derived class, so check its 
    ///   documentation to see what other exceptions may be raised. </para></remarks>
    /// <seealso cref="GetDataSet" />
    public virtual void SetDataSet(DataSet ds) {
      if (ds == null)
        throw new ArgumentNullException("ds");

      // Create a section for each table
      foreach (DataTable table in ds.Tables) {
        string section = table.TableName;
        DataRowCollection rows = table.Rows;
        if (rows.Count == 0)
          continue;

        // Loop through each column and add it as entry with value of the first row				
        foreach (DataColumn column in table.Columns) {
          string entry = column.ColumnName;
          object value = rows[0][column];

          SetValue(section, entry, value);
        }
      }
    }

    /// <summary>
    ///   Gets the name of the file to be used as the default, without the profile-specific extension. </summary>
    /// <remarks>
    ///   This property is used by file-based Profile implementations 
    ///   when composing the DefaultName.  These implementations take the value returned by this
    ///   property and add their own specific extension (.ini, .xml, .config, etc.).
    ///   <para>
    ///   For Windows applications, this property returns the full path of the executable.  
    ///   For Web applications, this returns the full path of the web.config file without 
    ///   the .config extension.  </para></remarks>
    /// <seealso cref="DefaultName" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    protected static string DefaultNameWithoutExtension {
      get {
        try {
          string file = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
          return file.Substring(0, file.LastIndexOf('.'));
        } catch {
          return "profile";  // if all else fails
        }
      }
    }

    /// <summary>
    ///   Verifies the given section name is not null and trims it. </summary>
    /// <param name="section">
    ///   The section name to verify and adjust. </param>
    /// <exception cref="ArgumentNullException">
    ///   section is null. </exception>
    /// <remarks>
    ///   This method may be used by derived classes to make sure that a valid
    ///   section name has been passed, and to make any necessary adjustments to it
    ///   before passing it to the corresponding APIs. </remarks>
    /// <seealso cref="VerifyAndAdjustEntry" />
    protected virtual void VerifyAndAdjustSection(ref string section) {
      if (section == null)
        throw new ArgumentNullException("section");

      section = section.Trim();
    }

    /// <summary>
    ///   Verifies the given entry name is not null and trims it. </summary>
    /// <param name="entry">
    ///   The entry name to verify and adjust. </param>
    /// <remarks>
    ///   This method may be used by derived classes to make sure that a valid
    ///   entry name has been passed, and to make any necessary adjustments to it
    ///   before passing it to the corresponding APIs. </remarks>
    /// <exception cref="ArgumentNullException">
    ///   entry is null. </exception>
    /// <seealso cref="VerifyAndAdjustSection" />
    protected virtual void VerifyAndAdjustEntry(ref string entry) {
      if (entry == null)
        throw new ArgumentNullException("entry");

      entry = entry.Trim();
    }

    /// <summary>
    ///   Verifies the Name property is not empty or null. </summary>
    /// <remarks>
    ///   This method may be used by derived classes to make sure that the 
    ///   APIs are working with a valid Name (file name) </remarks>
    /// <exception cref="InvalidOperationException">
    ///   name is empty or null. </exception>
    /// <seealso cref="Name" />
    protected internal virtual void VerifyName() {
      if (string.IsNullOrEmpty(_name))
        throw new InvalidOperationException("Operation not allowed because Name property is null or empty.");
    }

    /// <summary>
    ///   Verifies the ReadOnly property is not true. </summary>
    /// <remarks>
    ///   This method may be used by derived classes as a convenient way to 
    ///   validate that modifications to the profile can be made. </remarks>
    /// <exception cref="InvalidOperationException">
    ///   ReadOnly is true. </exception>
    /// <seealso cref="ReadOnly" />
    protected internal virtual void VerifyNotReadOnly() {
      if (_readOnly)
        throw new InvalidOperationException("Operation not allowed because ReadOnly property is true.");
    }

    #region IDisposable Members

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (disposing && _buffer != null)
        _buffer.Dispose();
    }

    #endregion

  } //class Profile
} //namespace BigSister