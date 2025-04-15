namespace AssetSeekAPIServer.Data
{
    public class TableDefinition
    {
        public string TableName { get; set; }
        public string CreateScript { get; set; }
    }

    public static class TableDefinitions
    {
        // List all required table definitions here.
        public static readonly List<TableDefinition> RequiredTables = new List<TableDefinition>
        {
            new TableDefinition
            {
                TableName = "dbo.ExampleTable",
                CreateScript = @"
                    CREATE TABLE dbo.ExampleTable (
                        Id INT PRIMARY KEY,
                        Name NVARCHAR(100) NOT NULL
                    );
                "
            },
            // Add additional table definitions as needed.
        };
    }
}
