using System;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// This message indicates the number of local-server users.
	/// </summary>
	[Serializable]
	public class LocalUsersReplyMessage : NumericMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="LocalUsersReplyMessage"/> class.
		/// </summary>
		public LocalUsersReplyMessage()
			: base()
		{
			this.InternalNumeric = 265;
		}

		/// <summary>
		/// Gets or sets the number of local users.
		/// </summary>
		public virtual int UserCount
		{
			get
			{
				return userCount;
			}
			set
			{
				userCount = value;
			}
		}
		private int userCount = -1;

		/// <summary>
		/// Gets or sets the maximum number of users for the server.
		/// </summary>
		public virtual int UserLimit
		{
			get
			{
				return userLimit;
			}
			set
			{
				userLimit = value;
			}
		}
		private int userLimit = -1;

		private string currentLocalUsers = "Current local users: ";
		private string max = " Max: ";

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			// official version:
			// :irc.server.com 265 artificer :Current local users: 606 Max: 610";

			// seen in the wild:
			// :irc.ptptech.com 265 artificer 606 610 :Current local users 606, max 610

			base.AddParametersToFormat( writer );
			writer.AddParameter( this.currentLocalUsers + this.UserCount + this.max + this.UserLimit );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			// official version:
			// :irc.server.com 265 artificer :Current local users: 606 Max: 610";

			// seen in the wild:
			// :irc.ptptech.com 265 artificer 606 610 :Current local users 606, max 610

			base.ParseParameters( parameters );
			if ( parameters.Count == 2 )
			{
				string payload = parameters[ 1 ];
				this.UserCount = Convert.ToInt32( MessageUtil.StringBetweenStrings( payload, this.currentLocalUsers, this.max ), CultureInfo.InvariantCulture );
				this.UserLimit = Convert.ToInt32( payload.Substring( payload.IndexOf( this.max, StringComparison.Ordinal ) + this.max.Length ), CultureInfo.InvariantCulture );
			}
			else if ( parameters.Count == 4 )
			{
				this.UserCount = Convert.ToInt32( parameters[ 1 ], CultureInfo.InvariantCulture );
				this.UserLimit = Convert.ToInt32( parameters[ 2 ], CultureInfo.InvariantCulture );
			}
			else
			{
				this.UserCount = -1;
				this.UserLimit = -1;
			}
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnLocalUsersReply( new IrcMessageEventArgs<LocalUsersReplyMessage>( this ) );
		}

	}
}
