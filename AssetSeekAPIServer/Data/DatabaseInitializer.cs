using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace AssetSeekAPIServer.Data
{
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Connects to the database and ensures that all required tables exist.
        /// </summary>
        /// <param name="connectionString">The connection string to the SQL Server instance.</param>
        public static void Initialize(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            foreach (var table in TableDefinitions.RequiredTables)
            {
                if (!TableExists(connection, table.TableName))
                {
                    CreateTable(connection, table.CreateScript);
                }
            }
        }

        private static bool TableExists(SqlConnection connection, string fullTableName)
        {
            // Assume the tableName is in the format "schema.TableName"
            var parts = fullTableName.Split('.');
            if (parts.Length != 2)
                throw new ArgumentException("Table name must be in the format 'schema.TableName'");

            string schema = parts[0];
            string table = parts[1];

            var query = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@schema", schema);
            command.Parameters.AddWithValue("@table", table);

            return (int)command.ExecuteScalar() > 0;
        }

        private static void CreateTable(SqlConnection connection, string createScript)
        {
            using var command = new SqlCommand(createScript, connection);
            command.ExecuteNonQuery();
        }
    }
}
