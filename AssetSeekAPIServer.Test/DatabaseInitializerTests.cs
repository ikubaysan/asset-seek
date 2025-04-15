using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssetSeekAPIServer.Data;
using Microsoft.Data.SqlClient;

namespace AssetSeekAPIServer.Test
{
    [TestClass]
    public class DatabaseInitializerTests
    {
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            // Use a configuration builder to load appsettings (or use a test-specific configuration)
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _connectionString = configuration.GetConnectionString("AzureSql");

            // Ensure the tables are created if they do not exist.
            DatabaseInitializer.Initialize(_connectionString);
        }

        [TestMethod]
        public void RequiredTables_ShouldExist_InDatabase()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            foreach (var tableDefinition in TableDefinitions.RequiredTables)
            {
                Assert.IsTrue(TableExists(connection, tableDefinition.TableName),
                    $"Expected table '{tableDefinition.TableName}' does not exist.");
            }
        }

        private bool TableExists(SqlConnection connection, string fullTableName)
        {
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
    }
}
