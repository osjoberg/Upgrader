using System;
using System.Linq;

using Upgrader.Infrastructure;

namespace Upgrader.SqlServer
{
    public class SqlServerDatabase : Database
    {
        private static readonly Lazy<ConnectionFactory> ConnectionFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory(new AdoProvider("Microsoft.Data.SqlClient.dll", "Microsoft.Data.SqlClient.SqlConnection"), new AdoProvider("System.Data.SqlClient.dll", "System.Data.SqlClient.SqlConnection"), new AdoProvider("System.Data.dll", "System.Data.SqlClient.SqlConnection")));
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public SqlServerDatabase(string connectionString) : base(ConnectionFactory.Value.CreateConnection(connectionString), GetMasterConnectionString(connectionString, "Initial Catalog", "master"))
        {
            this.connectionString = connectionString;

            TypeMappings.Add<bool>("bit");
            TypeMappings.Add<byte>("tinyint");
            TypeMappings.Add<char>("nchar(1)");
            TypeMappings.Add<DateTime>("datetime");
            TypeMappings.Add<decimal>("decimal(19,5)");
            TypeMappings.Add<double>("float");
            TypeMappings.Add<float>("real");
            TypeMappings.Add<Guid>("uniqueidentifier");
            TypeMappings.Add<int>("int");
            TypeMappings.Add<long>("bigint");
            TypeMappings.Add<short>("smallint");
            TypeMappings.Add<string>("nvarchar(50)");
        }

        public override bool Exists
        {
            get
            {
                UseMainDatabase();

                var exists = Dapper.ExecuteScalar<bool>(
                    "SELECT COUNT(*) FROM sysdatabases WHERE name = @databaseName",
                    new { databaseName = DatabaseName });

                UseConnectedDatabase();

                return exists;
            }
        }

        internal override string AutoIncrementStatement => "IDENTITY";

        internal override int MaxIdentifierLength => 128;

        internal override string GetSchema(string tableName)
        {
            if (tableName == null || tableName.Contains('.') == false)
            {
                return Dapper.ExecuteScalar<string>("SELECT SCHEMA_NAME()");
            }

            return tableName.Split('.').Last();
        }

        internal override string GetColumnDataType(string tableName, string columnName)
        {
            return InformationSchema.GetColumnDataType(tableName, columnName, "decimal", "nvarchar", "nchar", "varchar", "char", "time");
        }

        internal override string GetCreateComputedStatement(string dataType, bool nullable, string expression, bool persisted)
        {
            var persistedStatement = persisted ? " PERSISTED" : "";

            return $"AS ({expression}){persistedStatement}";
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

        internal override string GetLastInsertedAutoIncrementedPrimaryKeyIdentity(string columnName)
        {
            return "SELECT SCOPE_IDENTITY()";
        }

        internal override Database Clone()
        {
            return new SqlServerDatabase(connectionString);
        }
    }
}