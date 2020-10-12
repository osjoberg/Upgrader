using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Upgrader.Infrastructure;

namespace Upgrader.SqLite
{    
    public class SqLiteDatabase : Database
    {
        private static readonly Lazy<ConnectionFactory> ConnectionFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory("System.Data.SQLite.dll", "System.Data.SQLite.SQLiteConnection"));
        private static readonly Regex CreateTableSqlParser = new Regex(@"CONSTRAINT[\s]+([^ ]+)[\s]+FOREIGN[\s]+KEY[\s]*\(([^)]+)\)[\s]*REFERENCES[\s]+([^ ]+)[\s]*\(([^)]+)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqLiteDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public SqLiteDatabase(string connectionString) : base(ConnectionFactory.Value.CreateConnection(connectionString), null, (string)new DbConnectionStringBuilder { ConnectionString = connectionString }["Data Source"])
        {
            this.connectionString = connectionString;

            TypeMappings.Add<bool>("boolean");
            TypeMappings.Add<byte>("tinyint");
            TypeMappings.Add<char>("character(1)");
            TypeMappings.Add<DateTime>("datetime");
            TypeMappings.Add<decimal>("decimal(19,5)");
            TypeMappings.Add<double>("real");
            TypeMappings.Add<float>("real");
            TypeMappings.Add<Guid>("uniqueidentifier");
            TypeMappings.Add<int>("integer");
            TypeMappings.Add<long>("bigint");
            TypeMappings.Add<short>("smallint");
            TypeMappings.Add<string>("varchar(50)");
            
            // No type mapping for TimesSpan.
        }

        public override bool Exists => File.Exists(DatabaseName);

        internal override string AutoIncrementStatement => "";

        internal override int MaxIdentifierLength => 64;

        public override void Create()
        {
            File.WriteAllBytes(DatabaseName, new byte[0]);
        }

        public override void Remove()
        {
            File.Delete(DatabaseName);
        }

        internal override string EscapeIdentifier(string identifier)
        {
            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
        }

        internal override string GetSchema(string tableName)
        {
            if (tableName == null || tableName.Contains(".") == false)
            {
                return "main";
            }

            return tableName.Split('.').First();
        }

        internal override bool GetTableExists(string tableName)
        {
            return Dapper
                .Query<string>("SELECT name FROM sqlite_master WHERE type = 'table' AND name = @tableName", new { tableName })
                .Any();
        }

        internal override string[] GetTableNames()
        {
            return Dapper
                .Query<string>("SELECT name FROM sqlite_master WHERE type = 'table'")
                .ToArray();
        }

        internal override void Truncate(string tableName)
        {
            throw new NotSupportedException("Truncating tables is not supported by SQLite.");
        }

        internal override string[] GetColumnNames(string tableName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = schemaName;

            return Dapper
                .Query<TableInfo>($"PRAGMA {escapedSchemaName}.table_info({escapedTableName})")
                .Select(tableInfo => tableInfo.Name)
                .ToArray();
        }

        internal override string GetColumnDataType(string tableName, string columnName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = schemaName;

            return Dapper
                .Query<TableInfo>($"PRAGMA {escapedSchemaName}.table_info({escapedTableName})")
                .SingleOrDefault(tableInfo => tableInfo.Name == columnName)
                ?.Type;
        }

        internal override bool GetColumnNullable(string tableName, string columnName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = schemaName;

            return Dapper
                       .Query<TableInfo>($"PRAGMA {escapedSchemaName}.table_info({escapedTableName})")
                       .Single(tableInfo => tableInfo.Name == columnName)
                       .NotNull == false;
        }

        internal override bool GetColumnAutoIncrement(string tableName, string columnName)
        {
            var dataType = GetColumnDataType(tableName, columnName);
            if (string.Equals(dataType, "INTEGER", StringComparison.OrdinalIgnoreCase) == false && string.Equals(dataType, "INT", StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }

            var primaryKeyColumnsNames = GetPrimaryKeyColumnNames(tableName, "");
            var isSinglePrimaryKeyColumn = primaryKeyColumnsNames.Length == 1 && string.Equals(primaryKeyColumnsNames.Single(), columnName, StringComparison.CurrentCultureIgnoreCase);
            return isSinglePrimaryKeyColumn;
        }

        internal override void AddColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            if (nullable == false)
            {
                throw new NotSupportedException("Adding non-nullable columns is not supported by SQLite.");
            }

            base.AddColumn(tableName, columnName, dataType, true);
        }

        internal override string GetCreateComputedStatement(string dataType, bool nullable, string expression, bool persisted)
        {
            throw new NotSupportedException("Computed columns is not supported by SQLite.");
        }

        internal override void ChangeColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            throw new NotSupportedException("Modifying column properties is not supported by SQLite.");
        }

        internal override void RenameColumn(string tableName, string columnName, string newColumnName)
        {
            throw new NotSupportedException("Renaming columns is not supported by SQLite.");
        }

        internal override void RemoveColumn(string tableName, string columnName)
        {
            throw new NotSupportedException("Removing columns is not supported by SQLite.");
        }

        internal override string GetPrimaryKeyName(string tableName)
        {
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = schemaName;
            var escapedTableName = EscapeIdentifier(tableName);

            return Dapper
                .Query<TableInfo>($"PRAGMA {escapedSchemaName}.table_info({escapedTableName})")
                .Any(tableInfo => tableInfo.Pk) ? "" : null;
        }

        internal override string[] GetPrimaryKeyColumnNames(string tableName, string primaryKeyName)
        {
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = schemaName;
            var escapedTableName = EscapeIdentifier(tableName);

            return Dapper
                .Query<TableInfo>($"PRAGMA {escapedSchemaName}.table_info({escapedTableName})")
                .Where(tableInfo => tableInfo.Pk)
                .Select(tableInfo => tableInfo.Name)
                .ToArray();
        }

        internal override void AddPrimaryKey(string tableName, string[] columnNames, string primaryKeyName)
        {
            throw new NotSupportedException("Adding primary key to an already created table is not supported by SQLite.");
        }

        internal override void RemovePrimaryKey(string tableName, string primaryKeyName)
        {
            throw new NotSupportedException("Removing the primary key is not supported by SQLite.");
        }

        internal override string[] GetForeignKeyNames(string tableName)
        {
            var createTableSqlStatement = Dapper.ExecuteScalar<string>("SELECT sql FROM sqlite_master WHERE tbl_name = @tableName", new { tableName });
            
            var matches = CreateTableSqlParser.Matches(createTableSqlStatement);
            return matches
                .Cast<Match>()
                .Select(match => UnescapeIdentifier(match.Groups[1].Value))
                .ToArray();
        }

        internal override string[] GetForeignKeyColumnNames(string tableName, string foreignKeyName)
        {
            var createTableSqlStatement = Dapper.ExecuteScalar<string>("SELECT sql FROM sqlite_master WHERE tbl_name = @tableName", new { tableName });

            var matches = CreateTableSqlParser.Matches(createTableSqlStatement);
            return matches
                .Cast<Match>()
                .Where(match => UnescapeIdentifier(match.Groups[1].Value) == foreignKeyName)
                .SelectMany(match => match.Groups[2].Value.Split(',').Select(columnName => UnescapeIdentifier(columnName.Trim())))
                .ToArray();
        }

        internal override string GetForeignKeyForeignTableName(string tableName, string foreignKeyName)
        {
            var createTableSqlStatement = Dapper.ExecuteScalar<string>("SELECT sql FROM sqlite_master WHERE tbl_name = @tableName", new { tableName });

            var matches = CreateTableSqlParser.Matches(createTableSqlStatement);
            return matches
                .Cast<Match>()
                .Where(match => UnescapeIdentifier(match.Groups[1].Value) == foreignKeyName)
                .Select(match => UnescapeIdentifier(match.Groups[3].Value))
                .SingleOrDefault();
        }

        internal override string[] GetForeignKeyForeignColumnNames(string tableName, string foreignKeyName)
        {
            var createTableSqlStatement = Dapper.ExecuteScalar<string>("SELECT sql FROM sqlite_master WHERE tbl_name = @tableName", new { tableName });

            var matches = CreateTableSqlParser.Matches(createTableSqlStatement);
            return matches
                .Cast<Match>()
                .Where(match => UnescapeIdentifier(match.Groups[1].Value) == foreignKeyName)
                .SelectMany(match => match.Groups[4].Value.Split(',').Select(columnName => UnescapeIdentifier(columnName.Trim())))
                .ToArray();
        }

        internal override void AddForeignKey(string tableName, string[] columnNames, string foreignTableName, string[] foreignColumnNames, string foreignKeyName)
        {
            throw new NotSupportedException("Adding foreign key after table creation is not supported bySQLite");
        }

        internal override void RemoveForeignKey(string tableName, string foreignKeyName)
        {
            throw new NotSupportedException("Removing foreign key is not supported bySQLite");
        }

        internal override string[] GetIndexNames(string tableName)
        {
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = schemaName;
            var escapedTableName = EscapeIdentifier(tableName);

            return Dapper
                .Query<IndexList>($"PRAGMA {escapedSchemaName}.index_list({escapedTableName})")
                .Select(tableInfo => tableInfo.Name)
                .ToArray();
        }

        internal override string[] GetIndexColumnNames(string tableName, string indexName)
        {
            var escapedIndexName = EscapeIdentifier(indexName);
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = schemaName;

            return Dapper
                .Query<IndexList>($"PRAGMA {escapedSchemaName}.index_info({escapedIndexName})")
                .Select(tableInfo => tableInfo.Name)
                .ToArray();
        }

        internal override void AddIndex(string tableName, string[] columnNames, bool unique, string indexName, string[] includeColumnNames)
        {
            if (includeColumnNames != null)
            {
                throw new NotSupportedException("Including columns in an index is not supported by SQLite.");
            }

            this.DataDefinitionLanguage.AddIndex(tableName, columnNames, unique, indexName, null);
        }

        internal override bool GetIndexType(string tableName, string indexName)
        {
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = schemaName;
            var escapedTableName = EscapeIdentifier(tableName);

            return Dapper
                .Query<IndexList>($"PRAGMA {escapedSchemaName}.index_list({escapedTableName})")
                .Single(indexList => indexList.Name == indexName)
                .Unique;
        }

        internal override void RemoveIndex(string tableName, string indexName)
        {
            var escapedIndexName = EscapeIdentifier(indexName);
            var schemaName = GetSchema(tableName);
            var escapedSchemaName = EscapeIdentifier(schemaName);

            Dapper.Execute($"DROP INDEX {escapedSchemaName}.{escapedIndexName}");
        }

        internal override string GetLastInsertedAutoIncrementedPrimaryKeyIdentity(string columnName)
        {
            return ";SELECT last_insert_rowid()";
        }

        internal override Database Clone()
        {
            return new SqLiteDatabase(connectionString);
        }

        private static string UnescapeIdentifier(string identifier)
        {
            if (identifier.StartsWith("\"") == false)
            {
                return identifier;
            }

            return identifier.Substring(1, identifier.Length - 2).Replace("\"\"", "\"");            
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Internal class")]
        private class TableInfo
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public bool NotNull { get; set; }

            public bool Pk { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Internal class")]
        private class IndexList
        {
            public string Name { get; set; }

            public bool Unique { get; set; }
        }
    }
}