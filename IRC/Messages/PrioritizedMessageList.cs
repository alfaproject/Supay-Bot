using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BigSister.Irc.Messages
{

	/// <summary>
	/// A list which contains IrcMessages.
	/// </summary>
	/// <remarks>
	/// Call <see cref="m:Prioritize"/> on frequently accessed nodes in order to make
	/// finding common messages faster.
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix" ), Serializable]
	public class PrioritizedMessageList : System.Collections.Generic.LinkedList<IrcMessage>
	{

		/// <summary>
		/// Creates a new instances of the <see cref="PrioritizedMessageList"/> class.
		/// </summary>
		public PrioritizedMessageList()
			: base()
		{
		}

		/// <summary>
		/// Creates a new instances of the <see cref="PrioritizedMessageList"/> class.
		/// </summary>
		protected PrioritizedMessageList( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}


		/// <summary>
		/// Moves the given <see cref="LinkedListNode&lt;IrcMessage&gt;"/> to the front of an enumeration of the set.
		/// </summary>
		/// <param name="node">The <see cref="LinkedListNode&lt;IrcMessage&gt;"/> to prioritize</param>
		public void Prioritize( LinkedListNode<IrcMessage> node )
		{
			if ( node != null && node != this.First )
			{
				this.Remove( node );
				this.AddFirst( node );
			}
		}

	}
}
