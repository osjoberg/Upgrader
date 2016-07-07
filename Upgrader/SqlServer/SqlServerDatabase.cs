using System.Data.SqlClient;
using System.Linq;
using System.Data;
using Dapper;

namespace Upgrader.SqlServer
{

    public class SqlServerDatabase : Database
    {
        public SqlServerDatabase(string connectionStringOrName) : base(
            new SqlConnection(GetConnectionString(connectionStringOrName)),
            GetMasterConnectionString(connectionStringOrName, "Initial Catalog", "master"))
        {
        }

        public override bool Exists
        {
            get
            {
                UseMainDatabase();
                var exists = Dapper.ExecuteScalar<bool>("SELECT COUNT(*) FROM sysdatabases WHERE name = @databaseName", new { databaseName });

                UseConnectedDatabase();

                if (exists)
                {
                    Connection.Open();
                }

                return exists;
            }
        }

        internal override void UseMainDatabase()
        {
            if (Connection.State == ConnectionState.Open)
            {
               Connection.Execute("USE master");
            }
            
            base.UseMainDatabase();
        }

        internal override string GetSchema(string tableName)
        {
            if (tableName == null || tableName.Contains('.') == false)
            {
                return Dapper.ExecuteScalar<string>("SELECT SCHEMA_NAME()");
            }

            return tableName.Split('.').Last();
        }

        internal override void RenameColumn(string tableName, string columnName, string newColumnName)
        {
            Dapper.Execute($"sp_RENAME '{tableName}.{columnName}', '{newColumnName}', 'COLUMN'");
        }

        internal override void RenameTable(string tableName, string newTableName)
        {
            Dapper.Execute($"sp_RENAME '{tableName}', '{newTableName}'");
        }

        internal override bool GetColumnAutoIncrement(string tableName, string columnName)
        {
            return Dapper.ExecuteScalar<bool>(
                @"
                SELECT 
                    is_identity
                FROM sys.columns WHERE
                    object_id = OBJECT_ID(@tableName) AND 
                    name = @columnName
                ", 
                new { tableName, columnName });
        }

        internal override string EscapeIdentifier(string identifier)
        {
            return "[" + identifier.Replace("]", "]]") + "]";
        }

        internal override string AutoIncrementStatement => "IDENTITY";

        internal override int MaxIdentifierLength => 128;

        internal override string[] GetIndexNames(string tableName)
        {
            return Dapper.Query<string>(
                @"
                SELECT 
                    name
                FROM sys.indexes 
                WHERE 
                    object_id = OBJECT_ID(@tableName) AND 
                    [type] = 2
                ", 
                new { tableName }).ToArray();
        }

        internal override bool GetIndexType(string tableName, string indexName)
        {
            return Dapper.ExecuteScalar<bool>(
                @"
                SELECT 
                    is_unique
                FROM sys.indexes 
                WHERE 
                    name = @indexName AND
                    object_id = OBJECT_ID(@tableName) AND        
                    [type] = 2
                ", 
                new { indexName, tableName });
        }

        internal override string[] GetIndexColumnNames(string tableName, string indexName)
        {
            return Dapper.Query<string>(
                @"
                SELECT 
                    sys.columns.Name
                FROM sys.indexes
                INNER JOIN sys.index_columns ON 
	                sys.index_columns .object_id = sys.indexes.object_id AND 
	                sys.index_columns .index_id = sys.indexes.index_id
                INNER JOIN sys.columns ON 
	                sys.columns.object_id = sys.index_columns .object_id AND 
	                sys.columns.column_id = sys.index_columns .column_id
                WHERE
	                sys.columns.object_id = OBJECT_ID(@tableName) AND
	                sys.indexes.name = @indexName AND
                    sys.indexes.[type] = 2
                ", 
                new { indexName, tableName }).ToArray();
        }

        internal override void RemoveIndex(string tableName, string indexName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedIndexName = EscapeIdentifier(indexName);

            Dapper.Execute($"DROP INDEX {escapedTableName}.{escapedIndexName}");
        }
    }
}
