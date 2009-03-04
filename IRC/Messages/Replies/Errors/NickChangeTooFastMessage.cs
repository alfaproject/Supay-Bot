using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// The ErrorMessage sent when a user tries to change his nick too many times too quickly.
	/// </summary>
	[Serializable]
	public class NickChangeTooFastMessage : ErrorMessage
	{

		/// <summary>
		/// Creates a new instances of the <see cref="NickChangeTooFastMessage"/> class.
		/// </summary>
		public NickChangeTooFastMessage()
			: base()
		{
			this.InternalNumeric = 438;
		}

		/// <summary>
		/// Gets or sets the Nick which was attempted
		/// </summary>
		public string Nick
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
		private string nick;

		/// <summary>
		/// Gets or sets the number of seconds which must be waited before attempting again.
		/// </summary>
		public int Seconds
		{
			get
			{
				return seconds;
			}
			set
			{
				seconds = value;
			}
		}
		private int seconds;


		/// <exclude />
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Nick );
			writer.AddParameter( string.Format( CultureInfo.InvariantCulture, "Nick change too fast. Please wait {0} seconds.", this.Seconds ) );
		}

		/// <exclude />
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );
			this.Nick = string.Empty;
			this.Seconds = -1;
			if ( parameters.Count > 1 )
			{
				this.Nick = parameters[ 1 ];
				if ( parameters.Count > 2 )
				{
					this.Seconds = Convert.ToInt32( MessageUtil.StringBetweenStrings( parameters[ 2 ], "Please wait ", " seconds" ), CultureInfo.InvariantCulture );
				}
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnNickChangeTooFast( new IrcMessageEventArgs<NickChangeTooFastMessage>( this ) );
		}

	}
}
