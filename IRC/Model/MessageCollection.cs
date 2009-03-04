using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BigSister.Irc.Messages;

namespace BigSister.Irc
{

	/// <summary>
	///     <para>
	///       A collection that stores <see cref='BigSister.Irc.Messages.IrcMessage'/> objects.
	///    </para>
	/// </summary>
	[Serializable()]
	public class MessageCollection : ObservableCollection<IrcMessage>
	{

	}
}
