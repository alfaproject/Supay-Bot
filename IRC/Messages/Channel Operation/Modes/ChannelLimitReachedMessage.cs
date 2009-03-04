using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// The ErrorMessage received when attempting to join a channel which has reached its limit of users.
	/// </summary>
	/// <remarks>
	/// A channel can set it's user limit with a ChannelModeMessage containing a LimitMode.
	/// Once that many users are in the channel, any other users attempting to join will get this reply.
	/// On some networks, an Invite allows a user to bypass the limit.
	/// </remarks>
	[Serializable]
	public class ChannelLimitReachedMessage : ErrorMessage
	{
		//:irc.dkom.at 471 artificer #chaos :Cannot join channel (+l)

		/// <summary>
		/// Creates a new instances of the <see cref="ChannelLimitReachedMessage"/> class.
		/// </summary>
		public ChannelLimitReachedMessage()
			: base()
		{
			this.InternalNumeric = 471;
		}

		/// <summary>
		/// Gets or sets the channel which has reached its limit
		/// </summary>
		public string Channel
		{
			get
			{
				return channel;
			}
			set
			{
				channel = value;
			}
		}
		private string channel;

		/// <exclude />
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Channel );
			writer.AddParameter( "Cannot join channel (+l)" );
		}

		/// <exclude />
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );
			this.Channel = string.Empty;
			if ( parameters.Count > 2 )
			{
				this.Channel = parameters[ 1 ];
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnChannelLimitReached( new IrcMessageEventArgs<ChannelLimitReachedMessage>( this ) );
		}

	}
}
