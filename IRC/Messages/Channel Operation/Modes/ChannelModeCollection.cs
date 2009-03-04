using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BigSister.Irc.Messages.Modes
{
    
	/// <summary>
	///     <para>
	///       A collection that stores <see cref='BigSister.Irc.Messages.Modes.ChannelMode'/> objects.
	///    </para>
	/// </summary>
	/// <seealso cref='BigSister.Irc.Messages.Modes.ChannelModeCollection'/>
	[Serializable()]
	public class ChannelModeCollection : ObservableCollection<ChannelMode> {

		/// <summary>
		/// Clears the current collection and adds the given modes
		/// </summary>
		/// <param name="newModes"></param>
		public void ResetWith( ChannelModeCollection newModes )
		{
			this.Clear();
			foreach ( ChannelMode mode in newModes )
			{
				this.Add( mode );
			}
		}

	}
}
