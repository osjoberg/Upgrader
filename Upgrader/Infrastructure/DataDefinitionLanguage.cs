using System.Collections.Generic;
using System.Linq;
using Upgrader.Schema;

namespace Upgrader.Infrastructure
{
    internal class DataDefinitionLanguage
    {
        private readonly Database database;

        public DataDefinitionLanguage(Database database)
        {
            this.database = database;
        }

        internal void CreateDatabase(string databaseName)
        {
            var escapedDatabaseName = database.EscapeIdentifier(databaseName);

            database.Dapper.Execute($"CREATE DATABASE {escapedDatabaseName}");
        }

        internal void RemoveDatabase(string databaseName)
        {
            var escapedDatabaseName = database.EscapeIdentifier(databaseName);

            database.Dapper.Execute($"DROP DATABASE {escapedDatabaseName}");
        }

        internal void RenameTable(string tableName, string newTableName)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedNewTableName = database.EscapeIdentifier(newTableName);

            database.Dapper.Execute($"ALTER TABLE {escapedTableName} RENAME TO {escapedNewTableName}");
        }

        internal void AddTable(string tableName, IEnumerable<Column> columns, IEnumerable<ForeignKey> foreignKeys)
        {
            var columnsShallowClone = columns.ToArray();

            var escapedTableName = database.EscapeIdentifier(tableName);

            var columnDefinitions = string.Join(", ", columnsShallowClone.Select(column => $"{database.EscapeIdentifier(column.ColumnName)} {column.GetDataType(database.TypeMappings)} {GetNullableStatement(column.Nullable)} {GetAutoIncrementStatement(column.Modifier == ColumnModifier.AutoIncrementPrimaryKey)}"));

            var sql = $"CREATE TABLE {escapedTableName} ({columnDefinitions}";

            var primaryKeyColumnNames = columnsShallowClone
                .Where(column => column.Modifier == ColumnModifier.PrimaryKey || column.Modifier == ColumnModifier.AutoIncrementPrimaryKey)
                .Select(column => column.ColumnName)
                .ToArray();

            if (primaryKeyColumnNames.Any())
            {     
                var escapedPrimaryKeyColumnNames = primaryKeyColumnNames.Select(identifier => database.EscapeIdentifier(identifier)).ToArray();
                var escapedCommaSeparatedPrimaryKeyColumnNames = string.Join(", ", escapedPrimaryKeyColumnNames);
                var escapedPrimaryKeyConstraintName = database.EscapeIdentifier(database.NamingConvention.GetPrimaryKeyNamingConvention(tableName, primaryKeyColumnNames));

                sql += $", CONSTRAINT {escapedPrimaryKeyConstraintName} PRIMARY KEY ({escapedCommaSeparatedPrimaryKeyColumnNames})";
            }

            foreach (var foreignKey in foreignKeys)
            {
                var escapedForeignKeyName = database.EscapeIdentifier(foreignKey.ForeignKeyName ?? database.NamingConvention.GetForeignKeyNamingConvention(tableName, foreignKey.ColumnNames, foreignKey.ForeignTableName));
                var escapedForeignTableName = database.EscapeIdentifier(foreignKey.ForeignTableName);
                var escapedColumnNames = foreignKey.ColumnNames.Select(columnName => database.EscapeIdentifier(columnName));
                var escapedCommaSeparatedColumnNames = string.Join(", ", escapedColumnNames);
                var escapedForeignColumnNames = foreignKey.ForeignColumnNames.Select(foreignColumnName => database.EscapeIdentifier(foreignColumnName));
                var escapedForeignCommaSeparatedColumnNames = string.Join(", ", escapedForeignColumnNames);

                sql += $", CONSTRAINT {escapedForeignKeyName} FOREIGN KEY ({escapedCommaSeparatedColumnNames}) REFERENCES {escapedForeignTableName} ({escapedForeignCommaSeparatedColumnNames})";
            }

            sql += ")";

            database.Dapper.Execute(sql);
        }

        internal void RemoveTable(string tableName)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);

            database.Dapper.Execute($"DROP TABLE {escapedTableName}");
        }

        internal void AddColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedColumnName = database.EscapeIdentifier(columnName);
            var nullableStatement = GetNullableStatement(nullable);

            database.Dapper.Execute($"ALTER TABLE {escapedTableName} ADD {escapedColumnName} {dataType} {nullableStatement}");
        }

        internal void RemoveColumn(string tableName, string columnName)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedColumnName = database.EscapeIdentifier(columnName);

            database.Dapper.Execute($"ALTER TABLE {escapedTableName} DROP COLUMN {escapedColumnName}");
        }

        internal void ChangeColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedColumnName = database.EscapeIdentifier(columnName);
            var nullableStatement = GetNullableStatement(nullable);

            database.Dapper.Execute($"ALTER TABLE {escapedTableName} ALTER COLUMN {escapedColumnName} {dataType} {nullableStatement}");
        }

        internal void AddPrimaryKey(string tableName, string[] columnNames, string primaryKeyName)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedPrimaryKeyName = database.EscapeIdentifier(primaryKeyName);

            var escapedCommaSeparatedColumnNames = string.Join(", ", columnNames.Select(identifier => database.EscapeIdentifier(identifier)));

            database.Dapper.Execute($"ALTER TABLE {escapedTableName} ADD CONSTRAINT {escapedPrimaryKeyName} PRIMARY KEY ({escapedCommaSeparatedColumnNames})");
        }

        internal void RemovePrimaryKey(string tableName, string primaryKeyName)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedPrimaryKeyName = database.EscapeIdentifier(primaryKeyName);

            database.Dapper.Execute($"ALTER TABLE {escapedTableName} DROP CONSTRAINT {escapedPrimaryKeyName}");
        }

        internal void AddForeignKey(string tableName, string[] columnNames, string foreignTableName, string[] foreignColumnNames, string foreignKeyName)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedPrimaryKeyName = database.EscapeIdentifier(foreignKeyName);
            var escapedForeignTableName = database.EscapeIdentifier(foreignTableName);

            var escapedCommaSeparatedColumnNames = string.Join(", ", columnNames.Select(identifier => database.EscapeIdentifier(identifier)));
            var escapedCommaSeparatedForeignColumnNames = string.Join(", ", foreignColumnNames.Select(identifier1 => database.EscapeIdentifier(identifier1)));

            database.Dapper.Execute($"ALTER TABLE {escapedTableName} ADD CONSTRAINT {escapedPrimaryKeyName} FOREIGN KEY ({escapedCommaSeparatedColumnNames}) REFERENCES {escapedForeignTableName} ({escapedCommaSeparatedForeignColumnNames})");
        }

        internal void RemoveForeignKey(string tableName, string foreignKeyName)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedForeignKeyName = database.EscapeIdentifier(foreignKeyName);

            database.Dapper.Execute($"ALTER TABLE {escapedTableName} DROP CONSTRAINT {escapedForeignKeyName}");
        }

        internal void AddIndex(string tableName, string[] columnNames, bool unique, string indexName, string[] includeColumnNames)
        {
            var uniqueStatement = unique ? "UNIQUE " : "";
            var escapedIndexName = database.EscapeIdentifier(indexName);
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedCommaSeparatedColumnNames = string.Join(", ", columnNames.Select(columnName => database.EscapeIdentifier(columnName)));
            var escapedIncludeColumnNames = string.Join(", ", (includeColumnNames ?? Enumerable.Empty<string>()).Select(includeColumnName => database.EscapeIdentifier(includeColumnName)));
            var includeStatement = includeColumnNames != null ? $"INCLUDE ({escapedIncludeColumnNames})" : "";

            database.Dapper.Execute($"CREATE {uniqueStatement}INDEX {escapedIndexName} ON {escapedTableName} ({escapedCommaSeparatedColumnNames}) {includeStatement}");
        }

        internal void Truncate(string tableName)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);

            database.Dapper.Execute($"TRUNCATE TABLE {escapedTableName}");
        }

        private static string GetNullableStatement(bool nullable)
        {
            return nullable ? "NULL" : "NOT NULL";
        }

        private string GetAutoIncrementStatement(bool autoIncrement)
        {
            return autoIncrement ? database.AutoIncrementStatement : "";
        }
    }
}
