using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// The notification to the channel that a user has knocked on their channel.
	/// </summary>
	[Serializable]
	public class KnockRequestMessage : NumericMessage
	{
		// :irc.foxlink.net 710 #artificer2 #artificer2 artificer!artificer@12-255-177-172.client.attbi.com :has asked for an invite.

		/// <summary>
		/// Creates a new instance of the <see cref="KnockRequestMessage"/>.
		/// </summary>
		public KnockRequestMessage()
			: base()
		{
			this.InternalNumeric = 710;
		}

		/// <summary>
		/// Gets or sets the channel that was knocked on.
		/// </summary>
		public virtual string Channel
		{
			get
			{
				return this.channel;
			}
			set
			{
				this.channel = value;
			}
		}
		private string channel = string.Empty;

		/// <summary>
		/// Gets or sets the user which knocked on the channel.
		/// </summary>
		public virtual User Knocker
		{
			get
			{
				return this.knocker;
			}
			set
			{
				this.knocker = value;
			}
		}
		private User knocker = new User();

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Channel );
			writer.AddParameter( this.Knocker.ToString() );
			writer.AddParameter( "has asked for an invite." );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );
			if ( parameters.Count > 1 )
			{
				this.Channel = parameters[ 1 ];
			}
			else
			{
				this.Channel = string.Empty;
			}
			if ( parameters.Count > 2 )
			{
				this.Knocker = new User( parameters[ 2 ] );
			}
			else
			{
				this.Knocker = new User();
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnKnockRequest( new IrcMessageEventArgs<KnockRequestMessage>( this ) );
		}

	}
}
