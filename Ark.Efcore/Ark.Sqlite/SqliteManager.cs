using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ark.Sqlite
{
    public class SqliteManager
    {
        private static readonly Random _random = new Random();
        public static string RandomString(int size, bool lowerCase = true)
        {
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26
            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        public static string RemoveSpecialChar(string str)
        {
            string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
            //Iterate the number of times based on the String array length.
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "");
                }
            }
            return str;
        }
        public static string ReplaceSpecialChar(string str, Dictionary<string, string?> replace)
        {
            replace = replace ?? new Dictionary<string, string?>();
            foreach (var v in replace)
            {
                if (str.Contains(v.Key))
                {
                    if (!string.IsNullOrEmpty(v.Value))
                        str = str.Replace(v.Key, v.Value);
                    else
                        str = str.Replace(v.Key, "");
                }
            }
            return str;
        }

        string _connection_string;
        public SqliteManager(string connection_string)
        {
            _connection_string = connection_string;
        }
        public void UpdateTable(string table, Dictionary<string, object> cols, Dictionary<string, object> where)
        {
            ExecuteQuery(new TableScript().GenerateUpdateScript(table, cols, where));
        }
        /// <summary>
        /// inserts values passed and returns the last inserted identity
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public object InsertTable(string table, Dictionary<string, object> cols)
        {
            return ExecuteQuery(new TableScript().GenerateInsertScript(table, cols));
        }
        public void CreateTable(string table, Dictionary<string, ColumnProp> cols)
        {
            CreateTable(new TableScript().GenerateCreateScript(table, cols));
        }
        public void CreateTable(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
            {
                connection.Execute(qry);
            }
        }
        public bool IsColumnExist(string tbl, string column)
        {
            return ExecuteCount(new TableScript().GenerateColumnExistScript(tbl, column)) > 0;
        }
        public void AlterTable(string table, string col_name, ColumnProp col, bool overwrite = false)
        {
            if (overwrite && IsColumnExist(table, col_name)) Execute(new TableScript().GenerateAlterDropColumn(table, col_name));
            if (IsColumnExist(table, col_name)) return; // check if this is right
            Execute(new TableScript().GenerateAlterAddColumn(table, col_name, col));
        }
        public void Execute(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
            {
                connection.Execute(qry);
            }
        }
        public object ExecuteQuery(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
            {
                connection.Execute(qry);
                return connection.ExecuteScalar("select last_insert_rowid()");
            }
        }
        public object? ExecuteScalar(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
            {
                return connection.ExecuteScalar(qry);
            }
        }
        public T? ExecuteScalar<T>(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
            {
                return connection.ExecuteScalar<T>(qry);
            }
        }
        /// <summary>
        /// Scaler query, reutrns only the aggregate count values, (ex: count, max, min etc)
        /// </summary>
        /// <param name="qry"></param>
        /// <returns>long</returns>
        public long ExecuteCount(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
                return (long)connection.ExecuteScalar(qry);
        }
        /// <summary>
        /// T (primitive type) = long, int, double, decimal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry"></param>
        /// <returns></returns>
        public T ExecuteCount<T>(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
                return (T)connection.ExecuteScalar(qry);
        }
        public IEnumerable<T> ExecuteSelect<T>(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
                return connection.Query<T>(qry);
        }
        //public DataTable ExecuteSelect(string qry)
        //{
        //    using (var connection = new SqliteConnection(_connection_string))
        //    {
        //        DataTable dt = new DataTable();
        //        using (var cmd = connection.CreateCommand())
        //        {
        //            cmd.CommandText = qry;
        //            cmd.CommandType = CommandType.Text;

        //        }
        //    }
        //    return connection.Query<T>(qry);
        //}
        public dynamic Select(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
                return connection.Query(qry);
        }
        public T? First<T>(string qry)
        {
            try
            {
                using (var connection = new SqliteConnection(_connection_string))
                    return connection.QueryFirst<T>(qry);
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("Sequence contains no elements")) return default(T?);
                throw;
            }
        }
    }
}