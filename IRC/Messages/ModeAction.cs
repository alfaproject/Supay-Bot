using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BigSister.Irc.Messages {
  /// <summary>
  ///   Possible actions for each mode change in a <see cref="BigSister.Irc.Messages.ChannelModeMessage"/> or <see cref="BigSister.Irc.Messages.UserModeMessage"/> message. </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2229:ImplementSerializationConstructors", Justification = "Using IObjectReference instead"), Serializable]
  public sealed class ModeAction : MarshalByRefObject, IComparable, ISerializable {

    #region Static Instances

    /// <summary>
    /// Gets the <see cref="ModeAction"/> representing the addition of a mode to a user or channel.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ModeAction Add = new ModeAction("+");
    /// <summary>
    /// Gets the <see cref="ModeAction"/> representing the removal of a mode from a user or channel.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ModeAction Remove = new ModeAction("-");

    #endregion

    #region Static Properties

    /// <summary>
    /// Gets an array of <see cref="ModeAction"/> instances representing all the possible actions.
    /// </summary>
    public static IList<ModeAction> Values {
      get {
        if (values == null) {
          values = new ReadOnlyCollection<ModeAction>(new List<ModeAction> { ModeAction.Add, ModeAction.Remove });
        }
        return values;
      }
    }

    /// <summary>
    ///   Determines if the given string value is representative of any defined ModeActions. </summary>
    public static bool IsDefined(string value) {
      foreach (ModeAction modeAction in ModeAction.Values) {
        if (modeAction.ircName == value) {
          return true;
        }
      }
      return false;
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Returns the correct <see cref="ModeAction"/> for the given string value.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    public static ModeAction Parse(string value) {
      return Parse(value, false);
    }

    /// <summary>
    ///   Returns the correct <see cref="ModeAction"/> for the given string value. </summary>
    /// <param name="value">
    ///   The string to parse. </param>
    /// <param name="ignoreCase">
    ///   Decides whether the parsing is case-specific. </param>
    public static ModeAction Parse(string value, bool ignoreCase) {
      if (value == null) {
        throw new ArgumentNullException("value");
      }
      foreach (ModeAction modeAction in ModeAction.Values) {
        StringComparison compareMode = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (string.Compare(modeAction.ircName, value, compareMode) == 0) {
          return modeAction;
        }
      }
      throw new ArgumentException("ModeAction '{0}' does not exist.".FormatWith(value), "value");
    }

    #endregion

    #region CTor

    /// <summary>
    /// Creates a new instance of the <see cref="ModeAction"/> class.
    /// </summary>
    /// <remarks>
    /// This is private so that only the Enum-like static references can ever be used.
    /// </remarks>
    private ModeAction(string ircName) {
      this.ircName = ircName;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a string representing the <see cref="ModeAction"/> in irc format.
    /// </summary>
    public override string ToString() {
      return this.ircName;
    }

    #endregion

    #region Equality/Operators

    /// <summary>
    /// Implements Equals based on a string value.
    /// </summary>
    public override bool Equals(object obj) {
      ModeAction other = obj as ModeAction;
      if (other == null) {
        return base.Equals(obj);
      }
      return this.ircName.Equals(other.ircName);
    }

    /// <summary>
    /// Implements Equals based on a string value.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() {
      return this.ircName.GetHashCode();
    }

    /// <summary>
    ///   Implements the operator based on a string value. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    public static bool operator ==(ModeAction x, ModeAction y) {
      // If both are null, or both are same instance, return true.
      if (System.Object.ReferenceEquals(x, y)) {
        return true;
      }

      // If one is null, but not both, return false.
      if (((object)x == null) || ((object)y == null)) {
        return false;
      }

      return x.ircName == y.ircName;
    }

    /// <summary>
    ///   Implements the operator based on a string value. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    public static bool operator !=(ModeAction x, ModeAction y) {
      return !(x == y);
    }

    /// <summary>
    ///   Implements the operator based on a string value. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    public static bool operator <(ModeAction x, ModeAction y) {
      return string.CompareOrdinal(x.ircName, y.ircName) < 0;
    }

    /// <summary>
    ///   Implements the operator based on a string value. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    public static bool operator >(ModeAction x, ModeAction y) {
      return string.CompareOrdinal(x.ircName, y.ircName) > 0;
    }

    #endregion

    #region IComparable

    /// <summary>
    /// Implements <see cref="IComparable.CompareTo"/>
    /// </summary>
    public int CompareTo(Object obj) {
      if (obj == null) {
        return 1;
      }
      ModeAction ma = obj as ModeAction;
      if (ma == null) {
        throw new ArgumentException("Object must be of type ModeAction.", "obj");
      }
      return string.Compare(this.ircName, ma.ircName, StringComparison.Ordinal);
    }

    #endregion

    #region ISerializable

    /// <summary>
    /// Implements ISerializable.GetObjectData
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      if (info != null) {
        info.SetType(typeof(ModeActionProxy));
        info.AddValue("IrcName", this.ircName);
      }
    }

    [Serializable]
    private sealed class ModeActionProxy : IObjectReference, ISerializable {
      private ModeActionProxy(SerializationInfo info, StreamingContext context) {
        this.ircName = info.GetString("IrcName");
      }
      private string ircName = string.Empty;
      public object GetRealObject(StreamingContext context) {
        return ModeAction.Parse(this.ircName);
      }
      [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
      public void GetObjectData(SerializationInfo info, StreamingContext context) {
        throw new NotImplementedException();
      }
    }

    #endregion

    private string ircName;
    private static ReadOnlyCollection<ModeAction> values;

  }
}