using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Upgrader.Infrastructure;
using Upgrader.Schema;

namespace Upgrader
{
    public abstract class Database : IDisposable
    {
        private readonly DataDefinitionLanguage dataDefinitionLanguage;
        private readonly InformationSchema informationSchema;

        protected Database(IDbConnection connection)
        {
            Connection = connection;
            Dapper = new Infrastructure.Dapper(connection);
            Tables = new TableCollection(this);
            NamingConvention = new NamingConvention(MaxIdentifierLength);
            dataDefinitionLanguage = new DataDefinitionLanguage(this);
            informationSchema = new InformationSchema(this);
        }

        public IDbConnection Connection { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public NamingConvention NamingConvention { get; set; }

        public TableCollection Tables { get; }

        internal abstract string AutoIncrementStatement { get; }

        internal abstract int MaxIdentifierLength { get; }

        internal Infrastructure.Dapper Dapper { get; }

        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "This implementation is enough for now.")]
        public abstract void Dispose();

        internal string[] GetTableNames()
        {
            return informationSchema.GetTableNames();
        }

        internal void AddTable(string tableName, IEnumerable<Column> columns)
        {
            dataDefinitionLanguage.AddTable(tableName, columns);
        }

        internal void RemoveTable(string tableName)
        {
            dataDefinitionLanguage.RemoveTable(tableName);
        }

        internal string[] GetColumnNames(string tableName)
        {
            return informationSchema.GetColumnNames(tableName);
        }

        internal bool GetColumnNullable(string tableName, string columnName)
        {
            return informationSchema.GetColumnNullable(tableName, columnName);
        }

        internal string GetColumnDataType(string tableName, string columnName)
        {
            return informationSchema.GetColumnDataType(tableName, columnName);
        }

        internal void AddColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            dataDefinitionLanguage.AddColumn(tableName, columnName, dataType, nullable);
        }

        internal virtual void ChangeColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            dataDefinitionLanguage.ChangeColumn(tableName, columnName, dataType, nullable);
        }

        internal void RemoveColumn(string tableName, string columnName)
        {
            dataDefinitionLanguage.RemoveColumn(tableName, columnName);
        }

        internal string GetPrimaryKeyName(string tableName)
        {
            return informationSchema.GetPrimaryKeyName(tableName);
        }

        internal string[] GetPrimaryKeyColumnNames(string tableName, string primaryKeyName)
        {
            return informationSchema.GetPrimaryKeyColumnNames(tableName, primaryKeyName);
        }

        internal virtual void AddPrimaryKey(string tableName, string[] columnNames, string primaryKeyName)
        {
            dataDefinitionLanguage.AddPrimaryKey(tableName, columnNames, primaryKeyName);
        }

        internal virtual void RemovePrimaryKey(string tableName, string primaryKeyName)
        {
            dataDefinitionLanguage.RemovePrimaryKey(tableName, primaryKeyName);
        }

        internal string[] GetForeignKeyNames(string tableName)
        {
            return informationSchema.GetForeignKeyNames(tableName);
        }

        internal virtual string GetForeignKeyForeignTableName(string tableName, string foreignKeyName)
        {
            return informationSchema.GetForeignKeyForeignTableName(tableName, foreignKeyName);
        }

        internal string[] GetForeignKeyColumnNames(string tableName, string foreignKeyName)
        {
            return informationSchema.GetForeignKeyColumnNames(tableName, foreignKeyName);
        }

        internal virtual string[] GetForeignKeyForeignColumnNames(string tableName, string foreignKeyName)
        {
            return informationSchema.GetForeignKeyForeignColumnNames(tableName, foreignKeyName);
        }

        internal void AddForeignKey(string tableName, string[] columnNames, string foreignTableName, string[] foreignColumnNames, string foreignKeyName)
        {
            dataDefinitionLanguage.AddForeignKey(tableName, columnNames, foreignTableName, foreignColumnNames, foreignKeyName);
        }

        internal virtual void RemoveForeignKey(string tableName, string foreignKeyName)
        {
            dataDefinitionLanguage.RemoveForeignKey(tableName, foreignKeyName);
        }

        internal abstract string[] GetIndexNames(string tableName);

        internal abstract bool GetIndexType(string tableName, string indexName);

        internal abstract string[] GetIndexColumnNames(string tableName, string indexName);

        internal void AddIndex(string tableName, string[] columnNames, bool unique, string indexName)
        {
            dataDefinitionLanguage.AddIndex(tableName, columnNames, unique, indexName);
        }

        internal abstract void RemoveIndex(string tableName, string indexName);

        protected internal abstract string EscapeIdentifier(string identifier);

        protected internal abstract string GetSchema(string tableName);

        protected internal abstract void RenameColumn(string tableName, string columnName, string newColumnName);

        protected internal abstract void RenameTable(string tableName, string newTableName);

        internal void SetColumnValue(string tableName, string columnName, object value)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedColumnName = EscapeIdentifier(columnName);

            Dapper.Execute($"UPDATE {escapedTableName} SET {escapedColumnName} = @value", new { value });
        }

        internal abstract bool GetColumnAutoIncrement(string tableName, string columnName);
    }
}