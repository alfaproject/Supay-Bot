using System;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// An unknown <see cref="CtcpReplyMessage"/>.
	/// </summary>
	[Serializable]
	public class GenericCtcpReplyMessage : CtcpReplyMessage
	{

		/// <summary>
		/// Gets or sets the information packaged with the ctcp command.
		/// </summary>
		public virtual string DataPackage
		{
			get
			{
				return this.dataPackage;
			}
			set
			{
				this.dataPackage = value;
			}
		}
		private string dataPackage = string.Empty;

		/// <summary>
		/// Gets the data payload of the Ctcp request.
		/// </summary>
		protected override string ExtendedData
		{
			get
			{
				return this.dataPackage;
			}
		}

		/// <summary>
		/// Gets or sets the Ctcp command.
		/// </summary>
		public virtual string Command
		{
			get
			{
				return this.InternalCommand;
			}
			set
			{
				this.InternalCommand = value;
			}
		}


		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnGenericCtcpReply( new IrcMessageEventArgs<GenericCtcpReplyMessage>( this ) );
		}


		/// <summary>
		/// Parses the given string to populate this <see cref="IrcMessage"/>.
		/// </summary>
		public override void Parse( string unparsedMessage )
		{
			base.Parse( unparsedMessage );
			this.Command = CtcpUtil.GetInternalCommand( unparsedMessage );
			this.DataPackage = CtcpUtil.GetExtendedData( unparsedMessage );
		}

	}
}
