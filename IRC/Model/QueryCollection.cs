using System.Collections.ObjectModel;

namespace BigSister.Irc {
  /// <summary>
  ///   A collection of <see href="Query" /> objects. </summary>
  public class QueryCollection : ObservableCollection<Query> {

    /// <summary>
    /// Finds the <see href="Query" /> instance within the colleciton which is with the given user.
    /// </summary>
    /// <returns>The found query, or null.</returns>
    public Query FindQuery(User user) {
      foreach (Query q in this) {
        if (q.User == user || q.User.IsMatch(user)) {
          return q;
        }
      }
      return null;
    }

    /// <summary>
    /// Either finds or creates a <see href="Query" /> instance for the given <see href="User" /> on the given <see href="Client" />.
    /// </summary>
    public Query EnsureQuery(User user, Client client) {
      Query q = FindQuery(user);
      if (q == null) {
        q = new Query(client, user);
        this.Add(q);
      }
      return q;
    }

  }
}