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

        public static async Task<int> ExecuteNonQuery(string sql)
        {
            using (var command = new MySqlCommand(sql, _instance.Value._connection))
            {
                return await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task<string> LastUpdate(string rsn)
        {
            return await Lookup<string>("lastUpdate", "players", "rsn=@rsn", new[] { new MySqlParameter("@rsn", rsn) });
        }

        public static async Task Insert(string table, params string[] fieldsValues)
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

            await ExecuteNonQuery(sql.Substring(0, sql.Length - 2) + ")");
        }

        public static async Task Update(string table, string condition, params string[] fieldsValues)
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

            await ExecuteNonQuery(sql);
        }

        public static async Task<string> GetStringParameter(string table, string field, string condition, string parameter, string defaultValue)
        {
            var fieldValue = await Lookup(field, table, condition, null, string.Empty);
            if (fieldValue.ContainsI(parameter))
            {
                return Regex.Match(fieldValue, parameter + ":([^;]+)").Groups[1].Value;
            }
            return defaultValue;
        }

        public static async Task SetStringParameter(string table, string field, string condition, string parameter, string value)
        {
            var fieldValue = await Lookup(field, table, condition, null, string.Empty);
            if (fieldValue.ContainsI(parameter))
            {
                fieldValue = Regex.Replace(fieldValue, parameter + ":([^;]*)", parameter + ":" + value);
            }
            else
            {
                fieldValue += parameter + ":" + value + ";";
            }
            await Update(table, condition, field, fieldValue);
        }

        public static async Task<T> Lookup<T>(string field, string table, string condition = null, IEnumerable<MySqlParameter> parameters = null, T defaultValue = default(T))
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
            using (var command = new MySqlCommand(sql + " LIMIT 1", _instance.Value._connection))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                result = await command.ExecuteScalarAsync();
            }
            if (result == null || result is DBNull)
            {
                return defaultValue;
            }
            return (T) result;
        }
    }
}
