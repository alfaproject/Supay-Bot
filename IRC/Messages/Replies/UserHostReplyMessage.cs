using System;
using System.Collections.Specialized;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// Reply for the <see cref="UserHostMessage"/> to list replies to the query list.
	/// </summary>
	[Serializable]
	public class UserHostReplyMessage : NumericMessage
	{

		/// <summary>
		/// Creates a new instance of the <see cref="UserHostReplyMessage"/> class.
		/// </summary>
		public UserHostReplyMessage()
			: base()
		{
			this.InternalNumeric = 302;
		}

		/// <summary>
		/// Gets the list of replies in the message.
		/// </summary>
		public virtual UserCollection Users
		{
			get
			{
				return replies;
			}
		}

		private UserCollection replies = new UserCollection();

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			string replyList = MessageUtil.CreateList<User>( this.Users, " ", delegate( User user )
			{
				string result = user.Nick;
				if ( user.IrcOperator )
				{
					result += "*";
				}
				result += "=";
				result += ( user.OnlineStatus == UserOnlineStatus.Away ) ? "+" : "-";
				result += user.UserName;
				result += "@";
				result += user.HostName;
				return result;
			} );
			writer.AddParameter( replyList, true );
		}

		/// <summary>
		/// Parses the parameters portion of the message.
		/// </summary>
		protected override void ParseParameters( StringCollection parameters )
		{
			base.ParseParameters( parameters );
			this.Users.Clear();
			string[] userInfo = parameters[ parameters.Count - 1 ].Split( ' ' );
			foreach ( string info in userInfo )
			{
				string nick = info.Substring( 0, info.IndexOf( "=", StringComparison.Ordinal ) );
				Boolean oper = false;
				if ( nick.EndsWith( "*", StringComparison.Ordinal ) )
				{
					oper = true;
					nick = nick.Substring( 0, nick.Length - 1 );
				}
				string away = info.Substring( info.IndexOf( "=", StringComparison.Ordinal ) + 1, 1 );
				string standardHost = info.Substring( info.IndexOf( away, StringComparison.Ordinal ) );

				User user = new User();
				user.Parse( standardHost );
				user.Nick = nick;
				user.IrcOperator = oper;
				user.OnlineStatus = ( away == "+" ) ? UserOnlineStatus.Away : UserOnlineStatus.Online;

				this.Users.Add( user );
			}
			
		}

		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnUserHostReply( new IrcMessageEventArgs<UserHostReplyMessage>( this ) );
		}

	}
}
