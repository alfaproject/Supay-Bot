using System;
using System.Collections.Generic;
using System.Text;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// An interface implemented by messages which are, or can be, within the context of a query with a user.
	/// </summary>
	public interface IQueryTargetedMessage
	{

		/// <summary>
		/// Determines if the the current message is targeted at a query with the given user.
		/// </summary>
		Boolean IsQueryToUser( User user );

	}
}
