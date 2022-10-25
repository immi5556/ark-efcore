using Dapper;
using Microsoft.Data.Sqlite;

namespace Ark.Sqlite
{
    public class SqliteManager
    {
        string _connection_string;
        public SqliteManager(string connection_string)
        {
            _connection_string = connection_string;
        }
        public void CreateTable(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
                connection.Execute(qry);
        }
        public void ExecuteQuery(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
                connection.Execute(qry);
        }
        public IEnumerable<T> ExecuteSelect<T>(string qry)
        {
            using (var connection = new SqliteConnection(_connection_string))
                return connection.Query<T>(qry);
        }
    }
}