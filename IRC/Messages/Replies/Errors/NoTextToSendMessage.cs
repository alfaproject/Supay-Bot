using System;
using System.Collections.Specialized;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// The ErrorMessage sent when a TextMessage is sent with an empty Text property.
	/// </summary>
	[Serializable]
	public class NoTextToSendMessage : ErrorMessage
	{

		/// <summary>
		/// Creates a new instances of the <see cref="NoTextToSendMessage"/> class.
		/// </summary>
		public NoTextToSendMessage()
			: base()
		{
			this.InternalNumeric = 412;
		}

		/// <summary>
		/// Overrides <see href="IrcMessage.AddParametersToFormat" />
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( "No text to send" );
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnNoTextToSend( new IrcMessageEventArgs<NoTextToSendMessage>( this ) );
		}

	}
}
