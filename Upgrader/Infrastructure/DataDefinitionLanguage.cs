using System.Collections.Generic;
using System.Linq;
using Upgrader.Schema;

namespace Upgrader.Infrastructure
{
    internal class DataDefinitionLanguage
    {
        private readonly Database Database;

        public DataDefinitionLanguage(Database database)
        {
            Database = database;
        }

        internal void AddTable(string tableName, IEnumerable<Column> columns)
        {
            var columnsShallowClone = columns.ToArray();

            var escapedTableName = Database.EscapeIdentifier(tableName);

            var columnDefinitions = string.Join(", ", columnsShallowClone
                .Select(column => $"{Database.EscapeIdentifier(column.ColumnName)} {column.DataType} {GetNullableStatement(column.Nullable)} {GetAutoIncrementStatement(column.Modifier == ColumnModifier.AutoIncrementPrimaryKey)}"));

            var primaryKeyColumnNames = columnsShallowClone
                .Where(column => column.Modifier == ColumnModifier.PrimaryKey || column.Modifier == ColumnModifier.AutoIncrementPrimaryKey)
                .Select(column => column.ColumnName)
                .ToArray();

            if (primaryKeyColumnNames.Any() == false)
            {
                Database.Dapper.Execute($"CREATE TABLE {escapedTableName} ({columnDefinitions})");
                return;
            }

            var escapedPrimaryKeyColumnNames = primaryKeyColumnNames.Select(identifier => Database.EscapeIdentifier(identifier)).ToArray();
            var escapedCommaSeparatedPrimaryKeyColumnNames = string.Join(", ", escapedPrimaryKeyColumnNames);
            var escapedPrimaryKeyConstraintName = Database.EscapeIdentifier(Database.NamingConvention.GetPrimaryKeyNamingConvention(tableName, primaryKeyColumnNames));

            Database.Dapper.Execute($"CREATE TABLE {escapedTableName} ({columnDefinitions}, CONSTRAINT {escapedPrimaryKeyConstraintName} PRIMARY KEY ({escapedCommaSeparatedPrimaryKeyColumnNames}))");
        }

        internal void RemoveTable(string tableName)
        {
            var escapedTableName = Database.EscapeIdentifier(tableName);

            Database.Dapper.Execute($"DROP TABLE {escapedTableName}");
        }

        internal void AddColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            var escapedTableName = Database.EscapeIdentifier(tableName);
            var escapedColumnName = Database.EscapeIdentifier(columnName);
            var nullableStatement = GetNullableStatement(nullable);

            Database.Dapper.Execute($"ALTER TABLE {escapedTableName} ADD {escapedColumnName} {dataType} {nullableStatement}");
        }

        internal void RemoveColumn(string tableName, string columnName)
        {
            var escapedTableName = Database.EscapeIdentifier(tableName);
            var escapedColumnName = Database.EscapeIdentifier(columnName);

            Database.Dapper.Execute($"ALTER TABLE {escapedTableName} DROP COLUMN {escapedColumnName}");
        }

        internal void ChangeColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            var escapedTableName = Database.EscapeIdentifier(tableName);
            var escapedColumnName = Database.EscapeIdentifier(columnName);
            var nullableStatement = GetNullableStatement(nullable);

            Database.Dapper.Execute($"ALTER TABLE {escapedTableName} ALTER COLUMN {escapedColumnName} {dataType} {nullableStatement}");
        }

        internal void AddPrimaryKey(string tableName, string[] columnNames, string primaryKeyName)
        {
            var escapedTableName = Database.EscapeIdentifier(tableName);
            var escapedPrimaryKeyName = Database.EscapeIdentifier(primaryKeyName);

            var escapedCommaSeparatedColumnNames = string.Join(", ", columnNames.Select(identifier => Database.EscapeIdentifier(identifier)));

            Database.Dapper.Execute($"ALTER TABLE {escapedTableName} ADD CONSTRAINT {escapedPrimaryKeyName} PRIMARY KEY ({escapedCommaSeparatedColumnNames})");
        }

        internal void RemovePrimaryKey(string tableName, string primaryKeyName)
        {
            var escapedTableName = Database.EscapeIdentifier(tableName);
            var escapedPrimaryKeyName = Database.EscapeIdentifier(primaryKeyName);

            Database.Dapper.Execute($"ALTER TABLE {escapedTableName} DROP CONSTRAINT {escapedPrimaryKeyName}");
        }

        internal void AddForeignKey(string tableName, string[] columnNames, string foreignTableName, string[] foreignColumnNames, string foreignKeyName)
        {
            var escapedTableName = Database.EscapeIdentifier(tableName);
            var escapedPrimaryKeyName = Database.EscapeIdentifier(foreignKeyName);
            var escapedForeignTableName = Database.EscapeIdentifier(foreignTableName);

            var escapedCommaSeparatedColumnNames = string.Join(", ", columnNames.Select(identifier => Database.EscapeIdentifier(identifier)));
            var escapedCommaSeparatedForeignColumnNames = string.Join(", ", foreignColumnNames.Select(identifier1 => Database.EscapeIdentifier(identifier1)));

            Database.Dapper.Execute($"ALTER TABLE {escapedTableName} ADD CONSTRAINT {escapedPrimaryKeyName} FOREIGN KEY ({escapedCommaSeparatedColumnNames}) REFERENCES {escapedForeignTableName} ({escapedCommaSeparatedForeignColumnNames})");
        }

        internal void RemoveForeignKey(string tableName, string foreignKeyName)
        {
            var escapedTableName = Database.EscapeIdentifier(tableName);
            var escapedForeignKeyName = Database.EscapeIdentifier(foreignKeyName);

            Database.Dapper.Execute($"ALTER TABLE {escapedTableName} DROP CONSTRAINT {escapedForeignKeyName}");
        }

        internal void AddIndex(string tableName, string[] columnNames, bool unique, string indexName)
        {
            var uniqueStatement = unique ? "UNIQUE " : "";
            var escapedIndexName = Database.EscapeIdentifier(indexName);
            var escapedTableName = Database.EscapeIdentifier(tableName);
            var escapedCommaSeparatedColumnNames = string.Join(", ", columnNames.Select(identifier => Database.EscapeIdentifier(identifier)));

            Database.Dapper.Execute($"CREATE {uniqueStatement}INDEX {escapedIndexName} ON {escapedTableName} ({escapedCommaSeparatedColumnNames})");
        }

        private static string GetNullableStatement(bool nullable)
        {
            return nullable ? "NULL" : "NOT NULL";
        }

        private string GetAutoIncrementStatement(bool autoIncrement)
        {
            return autoIncrement ? Database.AutoIncrementStatement : "";
        }
    }
}
