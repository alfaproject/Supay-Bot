using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// This is the reply to an empty <see cref="ChannelModeMessage"/>.
	/// </summary>
	[Serializable]
	public class ChannelModeIsReplyMessage : NumericMessage, IChannelTargetedMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="ChannelModeIsReplyMessage"/> class.
		/// </summary>
		public ChannelModeIsReplyMessage()
			: base()
		{
			this.InternalNumeric = 324;
		}

		/// <summary>
		/// Gets or sets the channel reffered to.
		/// </summary>
		public virtual string Channel
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

		/// <summary>
		/// Gets or sets the modes in effect.
		/// </summary>
		/// <remarks>
		/// An example Modes might look like "+ml".
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

		/// <summary>
		/// Gets the collection of arguments ( parameters ) for the <see cref="ChannelModeIsReplyMessage.Modes"/> property.
		/// </summary>
		/// <remarks>
		/// Some modes require a parameter, such as +l ( user limit ) requires the number being limited to.
		/// </remarks>
		public virtual StringCollection ModeArguments
		{
			get
			{
				return modeArguments;
			}
		}

		private string channel = string.Empty;
		private string modes = string.Empty;
		private StringCollection modeArguments = new StringCollection();

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Channel );
			writer.AddParameter( this.Modes );
			if ( this.ModeArguments.Count != 0 )
			{
				writer.AddList( this.ModeArguments, " ", false );
			}
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );

			this.ModeArguments.Clear();
			if ( parameters.Count > 2 )
			{
				this.Channel = parameters[ 1 ];
				this.Modes = parameters[ 2 ];
				for ( int i = 3; i < parameters.Count; i++ )
				{
					this.ModeArguments.Add( parameters[ i ] );
				}
			}
			else
			{
				this.Channel = string.Empty;
				this.Modes = string.Empty;
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnChannelModeIsReply( new IrcMessageEventArgs<ChannelModeIsReplyMessage>( this ) );
		}


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
			return MessageUtil.IsIgnoreCaseMatch( this.Channel, channelName );
			;
		}

		#endregion
	}
}
