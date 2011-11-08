using System;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  static class Database {

    private static readonly Lazy<SQLiteConnection> _connection = new Lazy<SQLiteConnection>(() => new SQLiteConnection());

    static Database() {
      // initialize connection
      _connection.Value.ConnectionString = @"Data Source=Data/BigSister.db";
      _connection.Value.Open();
    }

    public static SQLiteDataReader ExecuteReader(string sql) {
      SQLiteCommand com = new SQLiteCommand(sql, _connection.Value);
      return com.ExecuteReader();
    }

    public static void ExecuteNonQuery(string sql) {
      SQLiteCommand com = new SQLiteCommand(sql, _connection.Value);
      com.ExecuteNonQuery();
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
      Database.Update(table, condition, field, fieldValue);
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

      SQLiteCommand command = new SQLiteCommand(sql + " LIMIT 1", _connection.Value);
      if (parameters != null) {
        command.Parameters.AddRange(parameters);
      }
      object result = command.ExecuteScalar();
      if (result == null || result is DBNull) {
        return defaultValue;
      }
      return (T) result;
    }

  } //class DataBase
} //namespace Supay.Bot