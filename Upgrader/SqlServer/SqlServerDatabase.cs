using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Upgrader.SqlServer
{
    public class SqlServerDatabase : Database
    {
        public SqlServerDatabase(string connectionString) : base(new SqlConnection(connectionString))
        {
        }

        public override sealed void Dispose()
        {
            Connection.Execute("USE master");
            Connection.Dispose();
        }

        protected internal override string GetSchema(string tableName)
        {
            if (tableName == null || tableName.Contains('.') == false)
            {
                return Dapper.ExecuteScalar<string>("SELECT SCHEMA_NAME()");
            }

            return tableName.Split('.').Last();
        }

        protected internal override void RenameColumn(string tableName, string columnName, string newColumnName)
        {
            Dapper.Execute($"sp_RENAME '{tableName}.{columnName}', '{newColumnName}', 'COLUMN'");
        }

        protected internal override void RenameTable(string tableName, string newTableName)
        {
            Dapper.Execute($"sp_RENAME '{tableName}', '{newTableName}'");
        }

        protected internal override string EscapeIdentifier(string identifier)
        {
            return "[" + identifier.Replace("]", "]]") + "]";
        }

        internal override string AutoIncrementStatement => "IDENTITY";

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
