using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{
	/// <summary>
	/// The PartMessage causes the client sending the message to be removed 
	/// from the list of active users for all given channels listed in the <see cref="PartMessage.Channels"/> property.
	/// </summary>
	[Serializable]
	public class PartMessage : CommandMessage, IChannelTargetedMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="PartMessage"/> class.
		/// </summary>
		public PartMessage() : base()
		{
		}

		/// <summary>
		/// Creates a new instance of the <see cref="PartMessage"/> class with the given channel.
		/// </summary>
		public PartMessage( string channel )
			: base()
		{
			this.channels.Add( channel );
		}

		/// <summary>
		/// Gets the channel name parted.
		/// </summary>
		public virtual StringCollection Channels
		{
			get
			{
				return this.channels;
			}
		}

		/// <summary>
		/// Gets or sets the reason for the part.
		/// </summary>
		public virtual string Reason
		{
			get
			{
				return this.reason;
			}
			set
			{
				if ( value == null )
				{
					throw new ArgumentNullException( "value" );
				}
				this.reason = value;
			}
		}

		/// <summary>
		/// Gets the Irc command associated with this message.
		/// </summary>
		protected override string Command
		{
			get
			{
				return "PART";
			}
		}

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddList( this.Channels, ",", true );
			if ( this.Reason.Length != 0 )
			{
				writer.AddParameter( this.Reason );
			}
		}

		/// <summary>
		/// Validates this message against the given server support
		/// </summary>
		public override void Validate( ServerSupport serverSupport )
		{
			base.Validate( serverSupport );
			for ( int i = 0; i < Channels.Count; i++ )
			{
				Channels[ i ] = MessageUtil.EnsureValidChannelName( Channels[ i ], serverSupport );
			}
		}

		/// <summary>
		/// Parse the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );
			this.Channels.Clear();
			if ( parameters.Count >= 1 )
			{
				this.Channels.AddRange( parameters[ 0 ].Split( ',' ) );
				if ( parameters.Count >= 2 )
				{
					this.Reason = parameters[ 1 ];
				}
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnPart( new IrcMessageEventArgs<PartMessage>( this ) );
		}

		private StringCollection channels = new StringCollection();
		private string reason = string.Empty;



		#region IChannelTargetedMessage Members

		bool IChannelTargetedMessage.IsTargetedAtChannel( string channelName )
		{
			return IsTargetedAtChannel( channelName );
		}

		/// <summary>
		/// Determines if the the current message is targeted at the given channel.
		/// </summary>
		protected virtual bool IsTargetedAtChannel( string channelName )
		{
			return MessageUtil.ContainsIgnoreCaseMatch( this.Channels, channelName );
		}

		#endregion
	}
}
