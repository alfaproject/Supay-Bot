using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Supay.Bot.Properties;

namespace Supay.Bot
{
    internal class Database
    {
        private static readonly Lazy<Database> _instance = new Lazy<Database>(() => new Database());
        private readonly MySqlConnection _connection;

        private Database()
        {
            // initialize connection
            this._connection = new MySqlConnection("Server=" + Settings.Default.DatabaseHost + ";Uid=" + Settings.Default.DatabaseUser + ";Pwd=" + Settings.Default.DatabasePass + ";Database=" + Settings.Default.DatabaseName + ";Charset=utf8");
            this._connection.Open();
        }

        public async static Task<List<IDataRecord>> FetchAll(string sql, params MySqlParameter[] parameters)
        {
            using (var command = new MySqlCommand(sql, _instance.Value._connection))
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    return reader.Cast<IDataRecord>().ToList();
                }
            }
        }

        public static async Task<IDataRecord> FetchFirst(string sql, params MySqlParameter[] parameters)
        {
            using (var command = new MySqlCommand(sql, _instance.Value._connection))
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    return reader.Cast<IDataRecord>().FirstOrDefault();
                }
            }
        }

        public static void ExecuteNonQuery(string sql)
        {
            lock (_instance.Value._connection)
            {
                using (var command = new MySqlCommand(sql, _instance.Value._connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static string LastUpdate(string rsn)
        {
            return Lookup<string>("lastUpdate", "players", "rsn=@rsn", new[] { new MySqlParameter("@rsn", rsn) });
        }

        public static void Insert(string table, params string[] fieldsValues)
        {
            string sql = "INSERT INTO `" + table + "` (";
            int i;
            for (i = fieldsValues.GetLowerBound(0); i <= fieldsValues.GetUpperBound(0) - 1; i += 2)
            {
                sql += "`" + fieldsValues[i] + "`, ";
            }
            sql = sql.Substring(0, sql.Length - 2) + ") VALUES (";
            for (i = fieldsValues.GetLowerBound(0) + 1; i <= fieldsValues.GetUpperBound(0); i += 2)
            {
                sql += "'" + fieldsValues[i].Replace("'", "''") + "', ";
            }

            ExecuteNonQuery(sql.Substring(0, sql.Length - 2) + ")");
        }

        public static void Update(string table, string condition, params string[] fieldsValues)
        {
            string sql = "UPDATE `" + table + "` SET ";
            for (int i = fieldsValues.GetLowerBound(0); i <= fieldsValues.GetUpperBound(0) - 1; i += 2)
            {
                sql += "`" + fieldsValues[i] + "` = '" + fieldsValues[i + 1].Replace("'", "''") + "', ";
            }
            sql = sql.Substring(0, sql.Length - 2);

            if (condition != null)
            {
                sql += " WHERE " + condition;
            }

            ExecuteNonQuery(sql);
        }

        public static string GetStringParameter(string table, string field, string condition, string parameter, string defaultValue)
        {
            string fieldValue = Lookup(field, table, condition, null, string.Empty);
            if (fieldValue.ContainsI(parameter))
            {
                return Regex.Match(fieldValue, parameter + ":([^;]+)").Groups[1].Value;
            }
            return defaultValue;
        }

        public static void SetStringParameter(string table, string field, string condition, string parameter, string value)
        {
            string fieldValue = Lookup(field, table, condition, null, string.Empty);
            if (fieldValue.ContainsI(parameter))
            {
                fieldValue = Regex.Replace(fieldValue, parameter + ":([^;]*)", parameter + ":" + value);
            }
            else
            {
                fieldValue += parameter + ":" + value + ";";
            }
            Update(table, condition, field, fieldValue);
        }

        public static T Lookup<T>(string field, string table, string condition = null, MySqlParameter[] parameters = null, T defaultValue = default(T))
        {
            string sql = "SELECT " + field + " FROM " + table;
            if (condition != null)
            {
                if (condition.StartsWithI("ORDER BY"))
                {
                    sql += " " + condition;
                }
                else
                {
                    sql += " WHERE " + condition;
                }
            }

            object result;
            lock (_instance.Value._connection)
            {
                using (var command = new MySqlCommand(sql + " LIMIT 1", _instance.Value._connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    result = command.ExecuteScalar();
                }
            }
            if (result == null || result is DBNull)
            {
                return defaultValue;
            }
            return (T) result;
        }
    }
}
