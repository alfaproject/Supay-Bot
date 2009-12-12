using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Supay.Bot.Irc {
  /// <summary>
  /// The nick prefixes that represent user level status in a channel.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2229:ImplementSerializationConstructors", Justification = "Using IObjectReference instead"), Serializable]
  public sealed class ChannelStatus : MarshalByRefObject, IComparable, ISerializable {

    private static List<ChannelStatus> newItems = new List<ChannelStatus>();
    private static List<ChannelStatus> definedItems = new List<ChannelStatus>();
    private static ReadOnlyCollection<ChannelStatus> values;


    #region Built In Static Instances

    /// <summary>
    /// Gets the <see cref="ChannelStatus"/> representing the operator status level.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ChannelStatus Operator = new ChannelStatus("@");
    /// <summary>
    /// Gets the <see cref="ChannelStatus"/> representing the half-operator status level.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ChannelStatus HalfOperator = new ChannelStatus("%");
    /// <summary>
    /// Gets the <see cref="ChannelStatus"/> representing the voiced status level.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ChannelStatus Voice = new ChannelStatus("+");
    /// <summary>
    /// Gets the <see cref="ChannelStatus"/> representing the no special status level.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ChannelStatus None = new ChannelStatus(string.Empty);

    #endregion

    #region Static Methods

    /// <summary>
    /// Gets a ChannelStatus instance with the given symbol.
    /// </summary>
    /// <remarks>
    /// If the given status is not defined already, a new status is created.
    /// This same new status is used for all future calls to GetInstance.
    /// </remarks>
    public static ChannelStatus GetInstance(string symbol) {
      foreach (ChannelStatus definedItem in definedItems) {
        if (definedItem.symbol == symbol) {
          return definedItem;
        }
      }

      foreach (ChannelStatus newItem in newItems) {
        if (newItem.symbol == symbol) {
          return newItem;
        }
      }
      ChannelStatus newRef = new ChannelStatus(symbol, false);
      newItems.Add(newRef);
      return newRef;
    }

    /// <summary>
    ///   Determines if the given ChannelStatus is one of the defined ChannelStatuses. </summary>
    public static bool IsDefined(ChannelStatus item) {
      foreach (ChannelStatus definedItem in definedItems) {
        if (definedItem.Equals(item)) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    ///   Determines if the given symbol is any of the known channel statuses. </summary>
    public static bool Exists(string symbol) {
      foreach (ChannelStatus definedItem in definedItems) {
        if (definedItem.symbol == symbol) {
          return true;
        }
      }
      foreach (ChannelStatus newItem in newItems) {
        if (newItem.symbol == symbol) {
          return true;
        }
      }
      return false;
    }

    #endregion

    #region Static Properties

    /// <summary>
    /// Gets a collection of <see cref="ChannelStatus"/> instances representing all built statuses.
    /// </summary>
    public static IList<ChannelStatus> Values {
      get {
        if (values == null) {
          values = new ReadOnlyCollection<ChannelStatus>(new List<ChannelStatus>
					{
						ChannelStatus.None,
						ChannelStatus.Operator,
						ChannelStatus.HalfOperator,
						ChannelStatus.Voice
					});
        }
        return values;
      }
    }

    #endregion

    #region CTor

    /// <summary>
    /// Creates a new instance of the <see cref="ChannelStatus"/> class.
    /// </summary>
    private ChannelStatus(string symbol)
      :
      this(symbol, true) {
    }

    /// <summary>
    ///   Creates a new instance of the <see cref="ChannelStatus"/> class. </summary>
    private ChannelStatus(string symbol, bool isDefined) {
      this.symbol = symbol;
      if (isDefined) {
        definedItems.Add(this);
      }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the string representation of the status.
    /// </summary>
    public string Symbol {
      get {
        return this.symbol;
      }
    }
    private string symbol;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a representation of the message in irc format.
    /// </summary>
    public override string ToString() {
      return this.Symbol;
    }

    #endregion

    #region Equality/Operators

    /// <summary>
    /// Determines if the given ChannelStatus is equal to this one.
    /// </summary>
    /// <remarks>This equality is determined by equality of the Symbol property.</remarks>
    public override bool Equals(object obj) {
      ChannelStatus other = obj as ChannelStatus;
      if (other == null) {
        return base.Equals(obj);
      }
      return this.symbol.Equals(other.symbol);
    }

    /// <summary>
    /// Implements GetHashCode using the Symbol property.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() {
      return this.symbol.GetHashCode();
    }

    /// <summary>
    ///   Implements the operator based on the Symbol property. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    public static bool operator ==(ChannelStatus x, ChannelStatus y) {
      // If both are null, or both are same instance, return true.
      if (System.Object.ReferenceEquals(x, y)) {
        return true;
      }

      // If one is null, but not both, return false.
      if (((object)x == null) || ((object)y == null)) {
        return false;
      }

      return x.symbol == y.symbol;
    }

    /// <summary>
    ///   Implements the operator based on the Symbol property. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    public static bool operator !=(ChannelStatus x, ChannelStatus y) {
      return !(x == y);
    }

    /// <summary>
    ///   Implements the operator based on the Symbol property. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    public static bool operator <(ChannelStatus x, ChannelStatus y) {
      return string.Compare(x.symbol, y.symbol, StringComparison.OrdinalIgnoreCase) < 0;
    }

    /// <summary>
    ///   Implements the operator based on the Symbol property. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x")]
    public static bool operator >(ChannelStatus x, ChannelStatus y) {
      return string.Compare(x.symbol, y.symbol, StringComparison.OrdinalIgnoreCase) > 0;
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
      ChannelStatus other = obj as ChannelStatus;
      if (other == null) {
        throw new ArgumentException("Object must be of type ChannelStatus.", "obj");
      }
      return string.Compare(this.symbol, other.symbol, StringComparison.OrdinalIgnoreCase);
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
        info.SetType(typeof(ChannelStatusProxy));
        info.AddValue("Symbol", this.symbol);
      }
    }

    [Serializable]
    private sealed class ChannelStatusProxy : IObjectReference, ISerializable {
      private ChannelStatusProxy(SerializationInfo info, StreamingContext context) {
        this.symbol = info.GetString("Symbol");
      }
      private string symbol = string.Empty;
      public object GetRealObject(StreamingContext context) {
        return ChannelStatus.GetInstance(this.symbol);
      }
      [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
      public void GetObjectData(SerializationInfo info, StreamingContext context) {
        throw new NotImplementedException();
      }
    }

    #endregion

  }
}