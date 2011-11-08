using System;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  internal class Database {
    private static readonly Lazy<Database> _instance = new Lazy<Database>(() => new Database());
    private readonly SQLiteConnection _connection;

    private Database() {
      // initialize connection
      _connection = new SQLiteConnection(@"Data Source=Data/BigSister.db");
      _connection.Open();
    }

    public static SQLiteDataReader ExecuteReader(string sql) {
      SQLiteDataReader reader;
      using (var command = new SQLiteCommand(sql, _instance.Value._connection)) {
        reader = command.ExecuteReader();
      }
      return reader;
    }

    public static void ExecuteNonQuery(string sql) {
      using (var command = new SQLiteCommand(sql, _instance.Value._connection)) {
        command.ExecuteNonQuery();
      }
    }

    public static string LastUpdate(string rsn) {
      return Lookup<string>("lastUpdate", "players", "rsn=@rsn", new[] { new SQLiteParameter("@rsn", rsn) });
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

      ExecuteNonQuery(sql.Substring(0, sql.Length - 2) + ");");
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

      ExecuteNonQuery(sql + ";");
    }

    public static string GetStringParameter(string table, string field, string condition, string parameter, string defaultValue) {
      string fieldValue = Lookup(field, table, condition, null, string.Empty);
      if (fieldValue.ContainsI(parameter)) {
        return Regex.Match(fieldValue, parameter + ":([^;]+)").Groups[1].Value;
      }
      return defaultValue;
    }

    public static void SetStringParameter(string table, string field, string condition, string parameter, string value) {
      string fieldValue = Lookup(field, table, condition, null, string.Empty);
      if (fieldValue.ContainsI(parameter)) {
        fieldValue = Regex.Replace(fieldValue, parameter + ":([^;]*)", parameter + ":" + value);
      } else {
        fieldValue += parameter + ":" + value + ";";
      }
      Update(table, condition, field, fieldValue);
    }

    public static T Lookup<T>(string field, string table, string condition = null, SQLiteParameter[] parameters = null, T defaultValue = default(T)) {
      string sql = "SELECT " + field + " FROM " + table;
      if (condition != null) {
        if (condition.StartsWithI("ORDER BY")) {
          sql += " " + condition;
        } else {
          sql += " WHERE " + condition;
        }
      }

      object result;
      using (var command = new SQLiteCommand(sql + " LIMIT 1", _instance.Value._connection)) {
        if (parameters != null) {
          command.Parameters.AddRange(parameters);
        }
        result = command.ExecuteScalar();
      }
      if (result == null || result is DBNull) {
        return defaultValue;
      }
      return (T) result;
    }
  }
}
