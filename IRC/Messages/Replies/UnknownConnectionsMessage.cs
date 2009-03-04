using System;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// This message indicates the number of unknown connections on the server.
	/// </summary>
	/// <remarks>
	/// This is most likely the number of clients who are starting to connect 
	/// but not yet registered with the server. (fully connected) 
	/// Unsure if this is server only or network wide. 
	/// Note that if there are no unknown connections, this will not be sent. 
	/// </remarks>
	[Serializable]
	public class UnknownConnectionsMessage : NumericMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="UnknownConnectionsMessage"/> class.
		/// </summary>
		public UnknownConnectionsMessage()
			: base()
		{
			this.InternalNumeric = 253;
		}

		/// <summary>
		/// Gets or sets the number of unknown connections
		/// </summary>
		public virtual int UnknownConnectionCount
		{
			get
			{
				return userCount;
			}
			set
			{
				userCount = value;
			}
		}
		private int userCount = -1;

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			// :irc.ptptech.com 253 artificer 15 :unknown connection(s)
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.UnknownConnectionCount.ToString( CultureInfo.InvariantCulture ) );
			writer.AddParameter( "unknown connection(s)" );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			// :irc.ptptech.com 253 artificer 15 :unknown connection(s)
			base.ParseParameters( parameters );
			this.UnknownConnectionCount = Convert.ToInt32( parameters[ 1 ], CultureInfo.InvariantCulture );
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnUnknownConnections( new IrcMessageEventArgs<UnknownConnectionsMessage>( this ) );
		}

	}
}
