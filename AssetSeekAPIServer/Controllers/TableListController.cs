using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AssetSeekAPIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableListController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TableListController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetTables()
        {
            var connStr = _configuration.GetConnectionString("AzureSql");
            var tableNames = new List<string>();

            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new SqlCommand(
                @"SELECT TABLE_SCHEMA + '.' + TABLE_NAME 
                  FROM INFORMATION_SCHEMA.TABLES 
                  WHERE TABLE_TYPE = 'BASE TABLE'", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tableNames.Add(reader.GetString(0));
            }

            return Ok(tableNames);
        }
    }
}
