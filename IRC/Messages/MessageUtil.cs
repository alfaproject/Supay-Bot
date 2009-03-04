using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using System.Globalization;


namespace BigSister.Irc.Messages
{

	/// <summary>
	/// Provides simple utilities for parsing and generating messages.
	/// </summary>
	/// <remarks>
	/// Client code will probably not need to use most of these routines.
	/// </remarks>
	public static class MessageUtil
	{

		/// <summary>
		/// Takes the given channel name, and returns a name that is valid according to the given server support.
		/// </summary>
		/// <param name="channelName">The channel name to examine</param>
		/// <param name="support">The feature support of an irc server</param>
		/// <returns>A valid channel name on the given server.</returns>
		public static string EnsureValidChannelName( string channelName, ServerSupport support )
		{
			if ( channelName == null || channelName.Length == 0 )
			{
				return "#irc";
			}
			if ( support == null )
			{
				support = ServerSupport.DefaultSupport;
			}

			string result = channelName.Replace( " ", "_" );
			result = result.Replace( ",", "_" );
			result = result.Replace( ":", "_" );
			string firstChar = result.Substring( 0, 1 );
			if ( firstChar != "#" && firstChar != "&" && firstChar != "+" && firstChar != "!" )
			{
				result = "#" + result;
			}

			if ( result.Length > support.MaxChannelNameLength )
			{
				return result.Substring( 0, support.MaxChannelNameLength );
			}
			else
			{
				return result;
			}
		}

		/// <summary>
		/// Determines if the given channel name has a valid namespace prefix.
		/// </summary>
		/// <remarks>
		/// This is according to the IRC spec, and is not representative of what a particular server may support.
		/// </remarks>
		public static Boolean HasValidChannelPrefix( string channelName )
		{
			if ( string.IsNullOrEmpty( channelName ) )
			{
				return false;
			}

			if ( channelName.StartsWith( "#", StringComparison.Ordinal ) )
			{
				return true;
			}
			if ( channelName.StartsWith( "&", StringComparison.Ordinal ) )
			{
				return true;
			}
			if ( channelName.StartsWith( "+", StringComparison.Ordinal ) )
			{
				return true;
			}
			if ( channelName.StartsWith( "!", StringComparison.Ordinal ) )
			{
				return true;
			}

			return false;
		}


		#region Parameters To string

		/// <summary>
		/// Creates a list of irc parameters from the given collection of strings.
		/// </summary>
		public static string ParametersToString( Boolean useColon, StringCollection parameters )
		{
			if ( parameters == null )
			{
				return string.Empty;
			}
			string[] foo = new string[ parameters.Count ];
			parameters.CopyTo( foo, 0 );
			return ParametersToString( useColon, foo );
		}

		/// <summary>
		/// Creates a list of irc parameters from the given array of strings.
		/// </summary>
		public static string ParametersToString( Boolean useColon, params string[] parameters )
		{
			StringBuilder result = new StringBuilder();
			if ( parameters.Length > 1 )
			{
				for ( int i = 0; i < parameters.Length - 1; i++ )
				{
					result.Append( parameters[ i ] );
					result.Append( " " );
				}
			}
			if ( parameters.Length != 0 )
			{
				if ( useColon )
				{
					result.Append( ":" );
				}
				result.Append( parameters[ parameters.Length - 1 ] );
			}
			return result.ToString();
		}

		/// <summary>
		/// Creates a list of irc parameters from the given collection of strings.
		/// </summary>
		public static string ParametersToString( StringCollection parameters )
		{
			return MessageUtil.ParametersToString( true, parameters );
		}

		/// <summary>
		/// Creates a list of irc parameters from the given array of strings.
		/// </summary>
		public static string ParametersToString( params string[] parameters )
		{
			return MessageUtil.ParametersToString( true, parameters );
		}

		#endregion

		#region Create Lists

		/// <summary>
		/// Creates a space-delimited list from the given StringCollection, using delimiter.
		/// </summary>
		public static string CreateList( StringCollection items, string delimiter )
		{
			if ( items == null )
			{
				return string.Empty;
			}
			string[] itemsArray = new string[ items.Count ];
			items.CopyTo( itemsArray, 0 );
			return CreateList( itemsArray, delimiter );
		}

		/// <summary>
		/// Creates a char-delimited list from the given string[], using delimiter.
		/// </summary>
		public static string CreateList( string[] items, string delimiter )
		{
			return string.Join( delimiter, items );
		}

		/// <summary>
		/// Creates a char-delimited list from the given IList of objects, using delimiter.
		/// </summary>
		public static string CreateList( IList items, string delimiter )
		{
			if ( items == null || items.Count == 0 )
			{
				return string.Empty;
			}

			StringBuilder result = new StringBuilder();
			result.Append( items[ 0 ].ToString() );
			for ( int i = 1; i < items.Count; i++ )
			{
				result.Append( delimiter );
				result.Append( items[ i ].ToString() );
			}
			return result.ToString();
		}

		/// <summary>
		/// Creates a char-delimited list from the given IEnumerable of objects, using delimiter.
		/// </summary>
		public static string CreateList<T>( IEnumerable<T> items, string delimiter, CustomListItemRendering<T> render )
		{
			if ( items == null )
			{
				return string.Empty;
			}
			StringBuilder result = new StringBuilder();
			foreach ( T item in items )
			{
				string itemValue = render( item );
				result.Append( itemValue );
				result.Append( delimiter );
			}
			if ( result.Length > delimiter.Length )
			{
				result.Remove( result.Length - delimiter.Length, delimiter.Length );
			}
			return result.ToString();
			
		}

		#endregion


		/// <summary>
		/// Extracts the Prefix from a string message.
		/// </summary>
		public static string GetPrefix( string rawMessage )
		{
			if ( !string.IsNullOrEmpty( rawMessage ) && rawMessage.StartsWith( ":", StringComparison.Ordinal ) )
			{
				return rawMessage.Substring( 1, rawMessage.IndexOf( " ", StringComparison.Ordinal ) ).Trim();
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Extracts the Command from a string message.
		/// </summary>
		public static string GetCommand( string rawMessage )
		{
			if ( string.IsNullOrEmpty( rawMessage ) )
			{
				return string.Empty;
			}

			// remove prefix
			if ( rawMessage.StartsWith( ":", StringComparison.Ordinal ) )
			{
				rawMessage = rawMessage.Substring( rawMessage.IndexOf( " ", 1, StringComparison.Ordinal ) + 1 );
				if ( string.IsNullOrEmpty( rawMessage ) )
				{
					return string.Empty;
				}
			}

			// the first token is the command
			int indexOfFirstSpace = rawMessage.IndexOf( " ", StringComparison.Ordinal );
			if ( indexOfFirstSpace == -1 )
			{
				return rawMessage;
			}
			return rawMessage.Substring( 0, indexOfFirstSpace );
		}

		/// <summary>
		/// Gets the parameters of the raw message.
		/// </summary>
		/// <param name="rawMessage">the message string which has the parameters.</param>
		public static StringCollection GetParameters( string rawMessage )
		{
			if ( string.IsNullOrEmpty( rawMessage ) )
			{
				return new StringCollection();
			}

			int startIndex;

			if ( rawMessage.StartsWith( ":", StringComparison.Ordinal ) )
			{
				// then the params start after space 2
				startIndex = MessageUtil.NthIndexOf( rawMessage, " ", 0, 2 ) + 1;
			}
			else
			{
				// then the params start after space 1
				startIndex = MessageUtil.NthIndexOf( rawMessage, " ", 0, 1 ) + 1;
			}

			if ( startIndex == 0 )
			{
				return new StringCollection();
			}
			return Tokenize( rawMessage, startIndex );
		}

		/// <summary>
		/// Seperates the given space-delimted parameter string into a collection.
		/// </summary>
		public static StringCollection Tokenize( string rawMessage, int startIndex )
		{
			if ( rawMessage == null )
			{
				throw new ArgumentNullException( "rawMessage" );
			}

			if ( rawMessage == cachedRawMessage )
			{
				return cachedParams;
			}

			StringCollection parameters = new StringCollection();
			StringBuilder param = new StringBuilder();
			for ( int i = startIndex; i < rawMessage.Length; i++ )
			{
				char c = rawMessage[ i ];
				if ( c.ToString() == " " )
				{
					parameters.Add( param.ToString() );
					param = new StringBuilder();
				}
				else if ( i + 1 == rawMessage.Length )
				{
					param.Append( c );
					parameters.Add( param.ToString() );
					param = new StringBuilder();
				}
				else if ( c.ToString() == ":" && param.Length == 0 )
				{
					parameters.Add( rawMessage.Substring( i + 1 ) );
					break;
				}
				else
				{
					param.Append( c );
				}
			}

			cachedRawMessage = rawMessage;
			cachedParams = parameters;

			return parameters;
		}

		private static string cachedRawMessage = string.Empty;
		private static StringCollection cachedParams;


		/// <summary>
		/// Gets the last parameter in the parameters collection of the given unparsed message.
		/// </summary>
		public static string GetLastParameter( string rawMessage )
		{
			StringCollection p = MessageUtil.GetParameters( rawMessage );
			if ( p.Count > 0 )
			{
				return p[ p.Count - 1 ];
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the nth parameter in the parameters collection of the given unparsed message.
		/// </summary>
		public static string GetParameter( string rawMessage, int index )
		{
			StringCollection p = MessageUtil.GetParameters( rawMessage );
			if ( p.Count > index )
			{
				return p[ index ];
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the substring in the input string existing between the given two strings.
		/// </summary>
		/// <param name="input">The string to search in.</param>
		/// <param name="before">The string before the one you want.</param>
		/// <param name="after">The string after the one you want.</param>
		public static string StringBetweenStrings( string input, string before, string after )
		{
			if ( input == null )
			{
				throw new ArgumentNullException( "input" );
			}
			if ( before == null )
			{
				throw new ArgumentNullException( "before" );
			}
			if ( after == null )
			{
				throw new ArgumentNullException( "after" );
			}

			int startOfBetween = 0;
			if ( before.Length != 0 )
			{
				int indexOfBefore = input.IndexOf( before, StringComparison.Ordinal );
				if ( indexOfBefore > -1 )
				{
					startOfBetween = indexOfBefore + before.Length;
				}
			}

			if ( after.Length == 0 )
			{
				return input.Substring( startOfBetween );
			}
			else
			{
				int startOfEnd = input.LastIndexOf( after, StringComparison.Ordinal );
				if ( startOfEnd == -1 )
				{
					return input.Substring( startOfBetween );
				}
				int lengthOfBetween = startOfEnd - startOfBetween;
				return input.Substring( startOfBetween, lengthOfBetween );
			}
		}

		/// <summary>
		/// Gets the index of the nth time that searchValue shows up in text.
		/// </summary>
		/// <param name="text">The string to search in.</param>
		/// <param name="searchValue">The string to search for.</param>
		/// <param name="startIndex">The place to start looking.</param>
		/// <param name="nthItem">The item to stop at.</param>
		public static int NthIndexOf( string text, string searchValue, int startIndex, int nthItem )
		{
			if ( text == null )
			{
				throw new ArgumentNullException( "text" );
			}

			int result = -1;
			int currentStartIndex = startIndex;
			for ( int i = 0; i < nthItem; i++ )
			{
				result = text.IndexOf( searchValue, currentStartIndex, StringComparison.Ordinal );
				if ( result == -1 )
					return result;
				else
					currentStartIndex = result + 1;
			}
			return result;
		}

		/// <summary>
		/// Turns a <see cref="DateTime"/> into the int representation as is commonly used on Unix machines
		/// </summary>
		public static int ConvertToUnixTime( DateTime dt )
		{
			DateTime unixEpochStartDate = new DateTime( 1970, 1, 1, 0, 0, 0, 0 );
			long ticksSinceEpochStart = dt.Ticks - unixEpochStartDate.Ticks;
			TimeSpan ts = new TimeSpan( ticksSinceEpochStart );
			return Convert.ToInt32( ts.TotalSeconds );
		}

		/// <summary>
		/// Turns the given Unix representation of a datetime into a <see cref="DateTime"/>.
		/// </summary>
		public static DateTime ConvertFromUnixTime( int ut )
		{
			DateTime unixEpochStartDate = new DateTime( 1970, 1, 1, 0, 0, 0, 0 );
			return unixEpochStartDate.AddSeconds( ut );
		}

		/// <summary>
		/// Converts the given Unix representation of a datetime into a <see cref="Nullable&lt;DateTime&gt;"/>.
		/// </summary>
		/// <remarks>
		/// If the string can't be parsed, a null DateTime is returned.
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string" )]
		public static DateTime? ConvertFromUnixTime( string unixTimeString )
		{
			if ( string.IsNullOrEmpty( unixTimeString ) )
			{
				return null;
			}

			int unixTime;
			if ( int.TryParse( unixTimeString, System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out unixTime ) )
			{
				return ConvertFromUnixTime( unixTime );
			}
			return null;
		}

		/// <summary>
		/// Determines if the given strings match eachother using <see href="StringComparison.InvariantCultureIgnoreCase" /> matching.
		/// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a"),
     System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.string.Compare(System.string,System.string,System.StringComparison)")]
    public static Boolean IsIgnoreCaseMatch(string a, string b) {
      return (string.Compare(a, b, StringComparison.InvariantCultureIgnoreCase) == 0);
    }

		/// <summary>
		/// Determines if the given collection of strings contains a string which matches the given string using <see href="StringComparison.InvariantCultureIgnoreCase" /> matching.
		/// </summary>
		/// <param name="strings">The list to look in</param>
		/// <param name="match">The string to look for</param>
		public static Boolean ContainsIgnoreCaseMatch( StringCollection strings, string match )
		{
			foreach ( string item in strings )
			{
				if ( IsIgnoreCaseMatch( item, match ) )
				{
					return true;
				}
			}
			return false;
		}
	}
}
