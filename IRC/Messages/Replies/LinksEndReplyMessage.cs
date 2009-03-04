using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// Marks the end of the replies to the <see cref="LinksMessage"/> query.
	/// </summary>
	[Serializable]
	public class LinksEndReplyMessage : NumericMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="LinksEndReplyMessage"/> class.
		/// </summary>
		public LinksEndReplyMessage()
			: base()
		{
			this.InternalNumeric = 365;
		}

		/// <summary>
		/// Gets or sets the server mask that the links list used.
		/// </summary>
		public virtual string Mask
		{
			get
			{
				return mask;
			}
			set
			{
				mask = value;
			}
		}

		private string mask = string.Empty;

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Mask );
			writer.AddParameter( "End of /LINKS list" );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );
			if ( parameters.Count == 3 )
			{
				this.Mask = parameters[ 1 ];
			}
			else
			{
				this.Mask = string.Empty;
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnLinksEndReply( new IrcMessageEventArgs<LinksEndReplyMessage>( this ) );
		}

	}
}
