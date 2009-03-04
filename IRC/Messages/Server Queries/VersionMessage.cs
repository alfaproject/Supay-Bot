using System;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// The <see cref="VersionMessage"/> is used to query the version of the server program.
	/// </summary>
	[Serializable]
	public class VersionMessage : ServerQueryBase
	{

		/// <summary>
		/// Gets the Irc command associated with this message.
		/// </summary>
		protected override string Command
		{
			get
			{
				return "VERSION";
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
			conduit.OnVersion( new IrcMessageEventArgs<VersionMessage>( this ) );
		}

	}
}
