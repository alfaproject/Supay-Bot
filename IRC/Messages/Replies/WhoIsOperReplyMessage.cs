using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// A reply to a <see cref="WhoIsMessage"/> when the user is an irc operator.
	/// </summary>
	[Serializable]
	public class WhoIsOperReplyMessage : NumericMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="WhoIsOperReplyMessage"/> class.
		/// </summary>
		public WhoIsOperReplyMessage()
			: base()
		{
			this.InternalNumeric = 313;
		}

		/// <summary>
		/// Gets or sets the Nick of the user being examined.
		/// </summary>
		public virtual string Nick
		{
			get
			{
				return nick;
			}
			set
			{
				nick = value;
			}
		}

		private string nick = string.Empty;

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Nick );
			writer.AddParameter( "is an IRC operator" );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );
			if ( parameters.Count == 3 )
			{
				this.Nick = parameters[ 1 ];
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnWhoIsOperReply( new IrcMessageEventArgs<WhoIsOperReplyMessage>( this ) );
		}

	}
}
