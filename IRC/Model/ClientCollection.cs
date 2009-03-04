using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BigSister.Irc.Network;

namespace BigSister.Irc
{
    
	/// <summary>
	///     <para>
	///       A collection that stores <see cref='BigSister.Irc.Client'/> objects.
	///    </para>
	/// </summary>
	/// <seealso cref='BigSister.Irc.ClientCollection'/>
	[Serializable()]
	public class ClientCollection : ObservableCollection<Client> {

		//public Client FindClient( string serverName )
		//{
		//    foreach ( Client c in this )
		//    {
		//        if ( c.ServerName == name )
		//        {
		//            return c;
		//        }
		//    }
		//    return null;
		//}

	}
}
