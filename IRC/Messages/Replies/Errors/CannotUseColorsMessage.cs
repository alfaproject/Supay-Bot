using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// Sent to a user who is trying to send control codes to a channel that is set +c.
	/// </summary>
	[Serializable]
	public class CannotUseColorsMessage : ErrorMessage, IChannelTargetedMessage
	{

		/// <summary>
		/// Creates a new instances of the <see cref="CannotUseColorsMessage"/> class.
		/// </summary>
		public CannotUseColorsMessage()
			: base()
		{
			this.InternalNumeric = 408;
		}

		/// <summary>
		/// Gets or sets the channel to which the message can't be sent.
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
		private string channel = string.Empty;

		/// <summary>
		/// Gets or sets the text which wasn't sent to the channel.
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}
		private string _text;


		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Channel );
			writer.AddParameter( "You cannot use colors on this channel. Not sent: " );
			writer.AddParameter( this.Text );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );
			this.Channel = string.Empty;
			this.Text = string.Empty;

			if ( parameters.Count > 1 )
			{
				this.Channel = parameters[ 1 ];
				if ( parameters.Count == 3 )
				{
					string freeText = parameters[ 2 ];
					this.Text = freeText.Substring( freeText.IndexOf( ": ", StringComparison.Ordinal ) + 2 );
				}
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnCannotUseColors( new IrcMessageEventArgs<CannotUseColorsMessage>( this ) );
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
