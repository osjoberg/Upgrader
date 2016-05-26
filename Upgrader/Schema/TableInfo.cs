using System;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    public class TableInfo
    {
        private readonly Database database;

        internal TableInfo(Database database, string tableName)
        {
            this.database = database;
            TableName = tableName;
            Columns = new ColumnCollection(database, tableName);
            ForeignKeys = new ForeignKeyCollection(database, tableName);
            Indexes = new IndexCollection(database, tableName);
        }

        public ColumnCollection Columns { get; }

        public ForeignKeyCollection ForeignKeys { get; }

        public IndexCollection Indexes { get; }

        public string TableName { get; }

        public PrimaryKeyInfo PrimaryKey
        {
            get
            {
                var constraintName = database.GetPrimaryKeyName(TableName);
                if (constraintName == null)
                {
                    return null;
                }

                return new PrimaryKeyInfo(database, TableName, constraintName);
            }
        }

        public void AddPrimaryKey(string columnName, string primaryKeyName = null)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));

            AddPrimaryKey(new[] { columnName }, primaryKeyName);
        }

        public void AddPrimaryKey(string[] columnNames, string primaryKeyName = null)
        {
            Validate.IsNotNullAndNotEmptyEnumerable(columnNames, nameof(columnNames));

            var constraintName = primaryKeyName ?? database.NamingConvention.GetPrimaryKeyNamingConvention(TableName, columnNames);

            database.AddPrimaryKey(TableName, columnNames, constraintName);
        }

        public void RemovePrimaryKey()
        {
            var constraintName = database.GetPrimaryKeyName(TableName);
            if (constraintName == null)
            {
                throw new InvalidOperationException($"No primary key was found on table {TableName}.");
            }

            database.RemovePrimaryKey(TableName, constraintName);
        }
    }
}
