namespace AssetSeekAPIServer.Data
{
    public class TableDefinition
    {
        public string TableName { get; set; }
        public string CreateScript { get; set; }
    }

    public static class TableDefinitions
    {
        public static readonly List<TableDefinition> RequiredTables = new()
        {
            new TableDefinition
            {
                TableName = "dbo.Users",
                CreateScript = @"
                    CREATE TABLE dbo.Users (
                        UserID INT IDENTITY PRIMARY KEY,
                        UserName NVARCHAR(50) NOT NULL UNIQUE,
                        Email NVARCHAR(255) NOT NULL UNIQUE,
                        PasswordHash VARBINARY(256) NOT NULL,
                        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                        CreatedIP VARCHAR(45) NOT NULL
                    );"
            },
            new TableDefinition
            {
                TableName = "dbo.Assets",
                CreateScript = @"
                    CREATE TABLE dbo.Assets (
                        AssetID INT IDENTITY PRIMARY KEY,
                        OwnerUserID INT NOT NULL,
                        Name NVARCHAR(100) NOT NULL,
                        Uri NVARCHAR(2083) NOT NULL UNIQUE,
                        Description NVARCHAR(MAX) NULL,
                        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                        CONSTRAINT FK_Assets_Users FOREIGN KEY (OwnerUserID)
                            REFERENCES dbo.Users(UserID)
                            ON DELETE CASCADE
                    );"
            },
            new TableDefinition
            {
                TableName = "dbo.Tags",
                CreateScript = @"
                    CREATE TABLE dbo.Tags (
                        TagID INT IDENTITY PRIMARY KEY,
                        TagName NVARCHAR(50) NOT NULL UNIQUE
                    );"
            },
            new TableDefinition
            {
                TableName = "dbo.AssetTags",
                CreateScript = @"
                    CREATE TABLE dbo.AssetTags (
                        AssetID INT NOT NULL,
                        TagID INT NOT NULL,
                        CONSTRAINT PK_AssetTags PRIMARY KEY (AssetID, TagID),
                        CONSTRAINT FK_AssetTags_Assets FOREIGN KEY (AssetID)
                            REFERENCES dbo.Assets(AssetID)
                            ON DELETE CASCADE,
                        CONSTRAINT FK_AssetTags_Tags FOREIGN KEY (TagID)
                            REFERENCES dbo.Tags(TagID)
                            ON DELETE CASCADE
                    );"
            },
            new TableDefinition
            {
                TableName = "dbo.Comments",
                CreateScript = @"
                    CREATE TABLE dbo.Comments (
                        CommentID INT IDENTITY PRIMARY KEY,
                        AssetID INT NOT NULL,
                        UserID INT NULL,  -- still stores user reference, but no FK constraint
                        Body NVARCHAR(MAX) NOT NULL,
                        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                        CONSTRAINT FK_Comments_Assets FOREIGN KEY (AssetID)
                            REFERENCES dbo.Assets(AssetID)
                            ON DELETE CASCADE
                        -- No FK on UserID to avoid multiple cascade paths
                    );"
            },
            new TableDefinition
            {
                TableName = "dbo.AssetSelections",
                CreateScript = @"
                    CREATE TABLE dbo.AssetSelections (
                        SelectionID INT IDENTITY PRIMARY KEY,
                        AssetID INT NOT NULL,
                        UserID INT NULL,
                        SelectedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

                        CONSTRAINT FK_Selections_Assets FOREIGN KEY (AssetID)
                            REFERENCES dbo.Assets(AssetID)
                            ON DELETE CASCADE
                        -- FK on UserID removed to avoid multiple cascade paths
                    );

                    CREATE INDEX IX_AssetSelections_Asset_Time
                        ON dbo.AssetSelections (AssetID, SelectedAt);"
            }
        };
    }
}
