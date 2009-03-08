using System;
using System.Collections.ObjectModel;
using BigSister.Irc.Messages;

namespace BigSister.Irc {
  /// <summary>
  ///   A collection that stores <see cref='BigSister.Irc.User'/> objects. </summary>
  [Serializable()]
  public class UserCollection : ObservableCollection<User> {

    /// <summary>
    ///   Removes the first User in the collection which is matched by the Predicate. </summary>
    /// <returns>
    ///   True if a User was removed, false if no User was removed. </returns>
    public bool RemoveFirst(Predicate<User> match) {
      for (int i = 0; i < this.Count; i++) {
        if (match(this[i])) {
          this.RemoveAt(i);
          return true;
        }
      }
      return false;
    }

    /// <summary>
    ///   Removes the first User in the collection which has the given nick. </summary>
    public bool RemoveFirst(string nick) {
      Predicate<User> match = delegate(User userToMatch) {
        return MessageUtil.IsIgnoreCaseMatch(userToMatch.Nick, nick);
      };
      return this.RemoveFirst(match);
    }

    /// <summary>
    /// Finds the first User in the collection which matches the given Predicate.
    /// </summary>
    public User Find(Predicate<User> match) {
      if (match == null) {
        throw new ArgumentNullException("match");
      }
      for (int i = 0; i < this.Count; i++) {
        if (match(this[i])) {
          return this[i];
        }
      }
      return null;
    }

    /// <summary>
    /// Finds the first User in the collection which matches the given nick.
    /// </summary>
    public User Find(string nick) {
      Predicate<User> match = delegate(User userToMatch) {
        return BigSister.Irc.Messages.MessageUtil.IsIgnoreCaseMatch(userToMatch.Nick, nick);
      };
      return Find(match);
    }

    /// <summary>
    /// Ensures that the collection has a User with the given nick.
    /// </summary>
    /// <remarks>
    /// If no User has the given nick, then a new User is created with the nick, and is added to the collection.
    /// </remarks>
    /// <param name="nick">The nick to ensure.</param>
    /// <returns>The User in the collection with the given nick.</returns>
    public User EnsureUser(string nick) {
      User knownUser = this.Find(nick);
      if (knownUser == null) {
        knownUser = new User(nick);
        this.Add(knownUser);
      }
      return knownUser;
    }

    /// <summary>
    /// Ensures that the collection has a User which matches the nick of the given User.
    /// </summary>
    /// <remarks>
    /// If no User matches the given User, then the given User added to the collection.
    /// If a User is found, then the existing User is merged with the given User.
    /// </remarks>
    /// <returns>The User in the collection which matches the given User.</returns>
    public User EnsureUser(User tempUser) {
      if (tempUser == null) {
        throw new ArgumentNullException("tempUser");
      }
      User knownUser = this.Find(tempUser.Nick);
      if (knownUser == null) {
        knownUser = tempUser;
        this.Add(knownUser);
      } else {
        knownUser.MergeWith(tempUser);
      }
      return knownUser;
    }

  }
}