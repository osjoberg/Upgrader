﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Upgrader.Infrastructure;

namespace Upgrader.SqLite
{
    public class SqLiteDatabase : Database
    {
        private static readonly ConnectionFactory ConnectionFactory = new ConnectionFactory("System.Data.SQLite.dll", "System.Data.SQLite.SQLiteConnection");

        private static readonly Regex CreateTableSqlParser = new Regex(@"CONSTRAINT[\s]+([^ ]+)[\s]+FOREIGN[\s]+KEY[\s]*\(([^)]+)\)[\s]*REFERENCES[\s]+([^ ]+)[\s]*\(([^)]+)\)", RegexOptions.IgnoreCase);

        public SqLiteDatabase(string connectionString) : base(ConnectionFactory.CreateConnection(connectionString))
        {
        }

        internal override string AutoIncrementStatement => "";

        internal override int MaxIdentifierLength => 64;

        protected internal override string EscapeIdentifier(string identifier)
        {
            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
        }

        protected internal override string GetSchema(string tableName)
        {
            if (tableName == null || tableName.Contains(".") == false)
            {
                return "main";
            }

            return tableName.Split('.').First();
        }

        internal override string[] GetTableNames()
        {
            return Dapper
                .Query<string>("SELECT name FROM sqlite_master WHERE type = 'table'")
                .ToArray();
        }

        protected internal override void RenameTable(string tableName, string newTableName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedNewTableName = EscapeIdentifier(newTableName);
            
            Dapper.Execute($"ALTER TABLE {escapedTableName} RENAME TO {escapedNewTableName}");
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
            if (string.Equals(dataType, "INTEGER", StringComparison.InvariantCultureIgnoreCase) == false)
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
                throw new NotSupportedException("Adding non-nullable columns is not supported in SQLite.");
            }

            base.AddColumn(tableName, columnName, dataType, true);
        }

        internal override void ChangeColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            throw new NotSupportedException("Modifying column properties is not supported in SQLite.");
        }

        protected internal override void RenameColumn(string tableName, string columnName, string newColumnName)
        {
            throw new NotSupportedException("Renaming columns is not supported in SQLite.");
        }

        internal override void RemoveColumn(string tableName, string columnName)
        {
            throw new NotSupportedException("Removing columns is not supported in SQLite.");
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
            throw new NotSupportedException("Adding primary key to an already created table is not supported in SQLite.");
        }

        internal override void RemovePrimaryKey(string tableName, string primaryKeyName)
        {
            throw new NotSupportedException("Removing the primary key is not supported in SQLite.");
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

        internal override void RemoveForeignKey(string tableName, string foreignKeyName)
        {
            throw new NotSupportedException("Removing foreign key is not supported in SQLite");
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

        private string UnescapeIdentifier(string identifier)
        {
            if (identifier.StartsWith("\"") == false)
            {
                return identifier;
            }

            return identifier.Substring(1, identifier.Length - 2).Replace("\"\"", "\"");            
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class TableInfo
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public bool NotNull { get; set; }

            public bool Pk { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class IndexList
        {
            public string Name { get; set; }

            public bool Unique { get; set; }
        }
    }
}