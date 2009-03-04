using System;
using System.Collections.Specialized;
using System.Text;


namespace BigSister.Irc.Messages
{

	/// <summary>
	/// This message is a request that the target of the message reply with a human-readable
	/// list stating what Ctcp commands they support.
	/// </summary>
	[Serializable]
	public class ClientInfoRequestMessage : CtcpRequestMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="ClientInfoRequestMessage"/> class
		/// </summary>
		public ClientInfoRequestMessage()
			: base()
		{
			this.InternalCommand = "CLIENTINFO";
		}

		/// <summary>
		/// Gets the list of parameters which signify interest in a specific command or subcommand.
		/// </summary>
		/// <remarks>
		/// To specificly ask about support for the "TIME" command, add "TIME" as the first parameter.
		/// </remarks>
		public virtual StringCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}
		private StringCollection parameters = new StringCollection();

		/// <summary>
		/// Gets the data payload of the Ctcp request.
		/// </summary>
		protected override string ExtendedData
		{
			get
			{
				return MessageUtil.CreateList( this.parameters, " " );
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnClientInfoRequest( new IrcMessageEventArgs<ClientInfoRequestMessage>( this ) );
		}

		/// <summary>
		/// Parses the given string to populate this <see cref="IrcMessage"/>.
		/// </summary>
		public override void Parse( string unparsedMessage )
		{
			base.Parse( unparsedMessage );
			this.Parameters.Clear();
			string paramsList = CtcpUtil.GetExtendedData( unparsedMessage );
			this.Parameters.AddRange( paramsList.Split( ' ' ) );
		}
	}

}
