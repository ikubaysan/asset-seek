using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;

namespace AssetSeekAPIServer.Test
{
    [TestClass]
    public class UserTests
    {
        private static string GetConnectionString()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            return config.GetConnectionString("AzureSql");
        }

        private static byte[] HashPassword(string password)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(password));
        }

        [TestMethod]
        public void AddUser_And_Verify_It_Exists()
        {
            string connStr = GetConnectionString();
            string userName = $"test_user_{Guid.NewGuid():N}";
            string email = $"{userName}@example.com";
            byte[] passwordHash = HashPassword("password123");
            string createdIp = "127.0.0.1";

            using var conn = new SqlConnection(connStr);
            conn.Open();

            using (var insertCmd = new SqlCommand(@"
                INSERT INTO dbo.Users (UserName, Email, PasswordHash, CreatedIP)
                VALUES (@UserName, @Email, @PasswordHash, @CreatedIP);", conn))
            {
                insertCmd.Parameters.AddWithValue("@UserName", userName);
                insertCmd.Parameters.AddWithValue("@Email", email);
                insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                insertCmd.Parameters.AddWithValue("@CreatedIP", createdIp);
                insertCmd.ExecuteNonQuery();
            }

            using var selectCmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE UserName = @UserName AND Email = @Email", conn);
            selectCmd.Parameters.AddWithValue("@UserName", userName);
            selectCmd.Parameters.AddWithValue("@Email", email);

            int count = (int)selectCmd.ExecuteScalar();
            Assert.AreEqual(1, count, "User was not found in the database after insertion.");
        }

        [TestMethod]
        public void AddMultipleUsers_And_Verify_They_Exist()
        {
            string connStr = GetConnectionString();
            using var conn = new SqlConnection(connStr);
            conn.Open();

            int totalToInsert = 5;
            int inserted = 0;

            for (int i = 0; i < totalToInsert; i++)
            {
                string userName = $"multi_user_{Guid.NewGuid():N}";
                string email = $"{userName}@example.com";
                byte[] passwordHash = HashPassword("password123");
                string createdIp = "127.0.0.1";

                using var insertCmd = new SqlCommand(@"
                    INSERT INTO dbo.Users (UserName, Email, PasswordHash, CreatedIP)
                    VALUES (@UserName, @Email, @PasswordHash, @CreatedIP);", conn);

                insertCmd.Parameters.AddWithValue("@UserName", userName);
                insertCmd.Parameters.AddWithValue("@Email", email);
                insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                insertCmd.Parameters.AddWithValue("@CreatedIP", createdIp);

                inserted += insertCmd.ExecuteNonQuery();
            }

            Assert.AreEqual(totalToInsert, inserted, "Not all users were inserted.");
        }
    }
}
