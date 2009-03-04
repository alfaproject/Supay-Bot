using System;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// The <see cref="AdminMessage"/> is used to find the name of the administrator of the given server, or current server if <see cref="ServerQueryBase.Target"/> is empty.
	/// </summary>
	[Serializable]
	public class AdminMessage : ServerQueryBase
	{

		/// <summary>
		/// Gets the Irc command associated with this message.
		/// </summary>
		protected override string Command
		{
			get
			{
				return "ADMIN";
			}
		}

		/// <summary>
		/// Overrides <see cref="IrcMessage.AddParametersToFormat"/>.
		/// </summary>
		protected override void AddParametersToFormat( IrcMessageWriter writer )
		{
			base.AddParametersToFormat( writer );
			writer.AddParameter( this.Target );
		}


		/// <summary>
		/// Notifies the given <see cref="MessageConduit"/> by raising the appropriate event for the current <see cref="IrcMessage"/> subclass.
		/// </summary>
		public override void Notify( BigSister.Irc.Messages.MessageConduit conduit )
		{
			conduit.OnAdmin( new IrcMessageEventArgs<AdminMessage>( this ) );
		}

	}
}
