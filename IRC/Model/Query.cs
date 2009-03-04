using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BigSister.Irc.Network;
using System.Collections.Specialized;

namespace BigSister.Irc
{

	/// <summary>
	/// Represents a query window for private chat with one User
	/// </summary>
	public class Query : INotifyPropertyChanged
	{

		#region CTor

		/// <summary>
		/// Creates a new instance of the <see cref="Query"/> class on the given client with the given User.
		/// </summary>
		public Query( Client client, User user )
		{
			this.client = client;
			this.journal.CollectionChanged += new NotifyCollectionChangedEventHandler( journal_CollectionChanged );
			this.User = user;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the User in the private chat.
		/// </summary>
		public User User
		{
			get
			{
				return user;
			}
			private set
			{
				user = value;
				NotifyPropertyChanged( "User" );
			}
		}
		private User user;

		/// <summary>
		/// Gets the journal of messages on the query
		/// </summary>
		public virtual Journal Journal
		{
			get
			{
				return journal;
			}
		}
		private Journal journal = new Journal();

		/// <summary>
		/// Gets the client which the query is on.
		/// </summary>
		public virtual Client Client
		{
			get
			{
				return client;
			}
		}
		private Client client;

		#endregion

		#region Event Handlers

		void journal_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			this.OnPropertyChanged( new PropertyChangedEventArgs( "Journal" ) );
		}

		#endregion

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Raised when a property value has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged( PropertyChangedEventArgs e )
		{
			if ( this.PropertyChanged != null )
			{
				this.PropertyChanged( this, e );
			}
		}

		private void NotifyPropertyChanged( string propertyName )
		{
			this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
		}

		#endregion
	}
}
