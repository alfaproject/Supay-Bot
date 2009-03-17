using System;
using System.Data.SQLite;
using System.Globalization;

namespace BigSister {
  public sealed class Database : IDisposable {

    private static readonly Database _instance = new Database();

    private SQLiteConnection _con;

    // explicit static constructor to tell c# compiler not to mark type as beforefieldinit
    static Database() {
    }

    private Database() {
      _con = new SQLiteConnection(@"Data Source=Data\BigSister.db");
      _con.Open();
    }

    public static Database Instance {
      get {
        return _instance;
      }
    }

    public SQLiteConnection Connection {
      get {
        return _con;
      }
    }

    public static SQLiteDataReader ExecuteReader(string sql) {
      SQLiteCommand com = new SQLiteCommand(sql, Database.Instance.Connection);
      return com.ExecuteReader();
    }

    public static void ExecuteNonQuery(string sql) {
      SQLiteCommand com = new SQLiteCommand(sql, Database.Instance.Connection);
      com.ExecuteNonQuery();
    }

    public static int GetInteger(string sql, int defaultValue) {
      SQLiteCommand com = new SQLiteCommand(sql, Database.Instance.Connection);
      object result = com.ExecuteScalar();
      if (result == null)
        return defaultValue;
      else
        return Convert.ToInt32(result, CultureInfo.InvariantCulture);
    }

    public static string GetString(string sql, string defaultValue) {
      SQLiteCommand com = new SQLiteCommand(sql, Database.Instance.Connection);
      object result = com.ExecuteScalar();
      if (result == null)
        return defaultValue;
      else
        return (string)result;
    }

    public static int PlayerID(string rsn) {
      return Database.GetInteger("SELECT id FROM players WHERE rsn='" + rsn + "';", 0);
    }

    public static string LastUpdate(string rsn) {
      return Database.GetString("SELECT lastupdate FROM players WHERE rsn='" + rsn + "';", string.Empty);
    }

    public static void Insert(string table, params string[] fields_values) {
      string sql = "INSERT INTO `" + table + "` (";
      int i;
      for (i = fields_values.GetLowerBound(0); i <= fields_values.GetUpperBound(0) - 1; i += 2) {
        sql += "`" + fields_values[i] + "`, ";
      }
      sql = sql.Substring(0, sql.Length - 2) + ") VALUES (";
      for (i = fields_values.GetLowerBound(0) + 1; i <= fields_values.GetUpperBound(0); i += 2) {
        sql += "'" + fields_values[i].Replace("'", "''") + "', ";
      }

      Database.ExecuteNonQuery(sql.Substring(0, sql.Length - 2) + ");");
    }

    public static void Update(string table, string condition, params string[] fields_values) {
      string sql = "UPDATE `" + table + "` SET ";
      for (int i = fields_values.GetLowerBound(0); i <= fields_values.GetUpperBound(0) - 1; i += 2) {
        sql += "`" + fields_values[i] + "` = '" + fields_values[i + 1].Replace("'", "''") + "', ";
      }
      sql = sql.Substring(0, sql.Length - 2);

      if (condition != null) {
        sql += " WHERE " + condition;
      }

      Database.ExecuteNonQuery(sql + ";");
    }

    public static object GetValue(string table, string field, string condition) {
      string sql = "SELECT `" + field + "` FROM `" + table + "`";
      if (condition != null) {
        sql += " WHERE " + condition;
      }

      SQLiteCommand com = new SQLiteCommand(sql + " LIMIT 1;", Database.Instance.Connection);
      return com.ExecuteScalar();
    }

    public static object GetValue(string table, string field) {
      return Database.GetValue(table, field, null);
    }


    #region IDisposable Members

    private bool _disposed;

    private void Dispose(bool disposing) {
      if (!_disposed) {
        if (disposing && _con != null) {
          _con.Dispose();
        }
        _con = null;
        _disposed = true;
      }
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

  } //class DataBase
} //namespace BigSister