using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// Marks the end of the replies to the <see cref="SilenceMessage"/> query.
	/// </summary>
	[Serializable]
	public class SilenceEndReplyMessage : NumericMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="SilenceEndReplyMessage"/> class.
		/// </summary>
		public SilenceEndReplyMessage()
			: base()
		{
			this.InternalNumeric = 272;
		}

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( "End of /SILENCE list." );
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnSilenceEndReply( new IrcMessageEventArgs<SilenceEndReplyMessage>( this ) );
		}

	}
}
