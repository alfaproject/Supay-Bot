using System;
using System.Collections.Specialized;
using System.Text;


namespace BigSister.Irc.Messages
{

	/// <summary>
	/// This message is sent from the server after connection,
	/// and contains information about the creation of the server.
	/// </summary>
	[Serializable]
	public class ServerCreatedMessage : NumericMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="ServerCreatedMessage"/> class.
		/// </summary>
		public ServerCreatedMessage()
			: base()
		{
			this.InternalNumeric = 003;
		}

		/// <summary>
		/// Gets or sets the date on which the server was created.
		/// </summary>
		public virtual string CreatedDate
		{
			get
			{
				return createdDate;
			}
			set
			{
				createdDate = value;
			}
		}
		private string createdDate = string.Empty;

		private string thisServerCreated = "This server was created ";

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( thisServerCreated + this.CreatedDate );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );

			string reply = parameters[ 1 ];
			if ( reply.IndexOf( thisServerCreated, StringComparison.Ordinal ) != -1 )
			{
				int startOfDate = thisServerCreated.Length;
				this.CreatedDate = reply.Substring( startOfDate );
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnServerCreated( new IrcMessageEventArgs<ServerCreatedMessage>( this ) );
		}

	}
}
