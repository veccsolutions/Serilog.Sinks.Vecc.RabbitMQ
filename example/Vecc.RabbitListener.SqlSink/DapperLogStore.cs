using System;
using System.Data.SqlClient;
using Dapper;

namespace Vecc.RabbitListener.SqlSink
{
    public class DapperLogStore : ILogStore
    {
        private string _connectionString;

        public DapperLogStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void StoreLogEntry(string applicationName, DateTime timestamp, string logDetail)
        {
            using (var connection = GetSqlConnection())
            {
                connection.Execute("spAddLogEntry", new
                {
                    applicationName = applicationName,
                    logTime = timestamp.ToUniversalTime(),
                    localTime = timestamp,
                    logEntry = logDetail
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        private SqlConnection GetSqlConnection()
            => new SqlConnection(_connectionString);
    }
}
