using System;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  sealed class Database : IDisposable {

    private static readonly Database _instance = new Database();

    private SQLiteConnection _con;

    private Database() {
      _con = new SQLiteConnection(@"Data Source=Data/BigSister.db");
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
      SQLiteCommand com = new SQLiteCommand(sql, Instance.Connection);
      object result = com.ExecuteScalar();
      if (result == null || result is DBNull) {
        return defaultValue;
      }
      return Convert.ToInt32(result, CultureInfo.InvariantCulture);
    }

    public static string GetString(string sql, string defaultValue) {
      SQLiteCommand com = new SQLiteCommand(sql, Database.Instance.Connection);
      object result = com.ExecuteScalar();
      if (result == null || result is DBNull)
        return defaultValue;
      else
        return (string)result;
    }

    public static int PlayerId(string rsn) {
      return Database.GetInteger("SELECT id FROM players WHERE rsn='" + rsn + "';", 0);
    }

    public static string LastUpdate(string rsn) {
      return Database.GetString("SELECT lastupdate FROM players WHERE rsn='" + rsn + "';", string.Empty);
    }

    public static void Insert(string table, params string[] fieldsValues) {
      string sql = "INSERT INTO `" + table + "` (";
      int i;
      for (i = fieldsValues.GetLowerBound(0); i <= fieldsValues.GetUpperBound(0) - 1; i += 2) {
        sql += "`" + fieldsValues[i] + "`, ";
      }
      sql = sql.Substring(0, sql.Length - 2) + ") VALUES (";
      for (i = fieldsValues.GetLowerBound(0) + 1; i <= fieldsValues.GetUpperBound(0); i += 2) {
        sql += "'" + fieldsValues[i].Replace("'", "''") + "', ";
      }

      Database.ExecuteNonQuery(sql.Substring(0, sql.Length - 2) + ");");
    }

    public static void Update(string table, string condition, params string[] fieldsValues) {
      string sql = "UPDATE `" + table + "` SET ";
      for (int i = fieldsValues.GetLowerBound(0); i <= fieldsValues.GetUpperBound(0) - 1; i += 2) {
        sql += "`" + fieldsValues[i] + "` = '" + fieldsValues[i + 1].Replace("'", "''") + "', ";
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

    public static string GetStringParameter(string table, string field, string condition, string parameter, string defaultValue) {
      string fieldValue = Database.GetString("SELECT `" + field + "` FROM `" + table + "` WHERE " + condition + " LIMIT 1;", string.Empty);
      if (fieldValue.ContainsI(parameter)) {
        return Regex.Match(fieldValue, parameter + ":([^;]+)").Groups[1].Value;
      } else {
        return defaultValue;
      }
    }

    public static void SetStringParameter(string table, string field, string condition, string parameter, string value) {
      string fieldValue = Database.GetString("SELECT `" + field + "` FROM `" + table + "` WHERE " + condition + " LIMIT 1;", string.Empty);
      if (fieldValue.ContainsI(parameter)) {
        fieldValue = Regex.Replace(fieldValue, parameter + ":([^;]*)", parameter + ":" + value);
      } else {
        fieldValue += parameter + ":" + value + ";";
      }
      Database.Update(table, condition, field, fieldValue);
    }

    public static T Lookup<T>(string field, string table, string condition = null, SQLiteParameter[] parameters = null, T defaultValue = default(T)) {
      string sql = "SELECT " + field + " FROM " + table;
      if (condition != null) {
        sql += " WHERE " + condition;
      }

      SQLiteCommand command = new SQLiteCommand(sql + " LIMIT 1", _instance.Connection);
      command.Parameters.AddRange(parameters);
      object result = command.ExecuteScalar();
      if (result == null || result is DBNull) {
        return defaultValue;
      }
      return (T) result;
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
} //namespace Supay.Bot