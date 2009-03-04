using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// This is the reply to an empty <see cref="UserModeMessage"/>.
	/// </summary>
	[Serializable]
	public class UserModeIsReplyMessage : NumericMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="UserModeIsReplyMessage"/> class.
		/// </summary>
		public UserModeIsReplyMessage()
			: base()
		{
			this.InternalNumeric = 221;
		}

		/// <summary>
		/// Gets or sets the modes in effect.
		/// </summary>
		/// <remarks>
		/// An example Modes might look like "+i".
		/// </remarks>
		public virtual string Modes
		{
			get
			{
				return modes;
			}
			set
			{
				modes = value;
			}
		}

		private string modes = string.Empty;

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Modes );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );

			if ( parameters.Count >= 1 )
			{
				this.Modes = parameters[ 1 ];
			}
			else
			{
				this.Modes = string.Empty;
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnUserModeIsReply( new IrcMessageEventArgs<UserModeIsReplyMessage>( this ) );
		}

	}
}
