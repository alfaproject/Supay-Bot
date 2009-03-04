using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BigSister.Irc.Messages.Modes
{
    
    
	/// <summary>
	///     <para>
	///       A collection that stores <see cref='BigSister.Irc.Messages.Modes.UserMode'/> objects.
	///    </para>
	/// </summary>
	/// <seealso cref='BigSister.Irc.Messages.Modes.UserModeCollection'/>
	[Serializable()]
	public class UserModeCollection : ObservableCollection<UserMode>
	{
        
	}
}
