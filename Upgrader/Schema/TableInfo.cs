using System;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Table information and modification.
    /// </summary>
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

        /// <summary>
        /// Gets collection of columns in table.
        /// </summary>
        public ColumnCollection Columns { get; }

        /// <summary>
        /// Gets collection of foreign keys in table.
        /// </summary>
        public ForeignKeyCollection ForeignKeys { get; }

        /// <summary>
        /// Gets collection of indexes in table.
        /// </summary>
        public IndexCollection Indexes { get; }

        /// <summary>
        /// Gets table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets primary key information. Returns null if no primary key is present.
        /// </summary>
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

        /// <summary>
        /// Add a primary key to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="primaryKeyName">Primary key name. If not name is given, name is set by convention.</param>
        public void AddPrimaryKey(string columnName, string primaryKeyName = null)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);
            Validate.IsNotEmpty(primaryKeyName, nameof(primaryKeyName));
            Validate.MaxLength(primaryKeyName, nameof(primaryKeyName), database.MaxIdentifierLength);

            AddPrimaryKey(new[] { columnName }, primaryKeyName);
        }

        /// <summary>
        /// Add a primary key to the table.
        /// </summary>
        /// <param name="columnNames">Column names.</param>
        /// <param name="primaryKeyName">Primary key name. If not name is given, name is set by convention.</param>
        public void AddPrimaryKey(string[] columnNames, string primaryKeyName = null)
        {
            Validate.IsNotNullAndNotEmptyEnumerable(columnNames, nameof(columnNames));
            Validate.MaxLength(columnNames, nameof(columnNames), database.MaxIdentifierLength);
            Validate.IsNotEmpty(primaryKeyName, nameof(primaryKeyName));
            Validate.MaxLength(primaryKeyName, nameof(primaryKeyName), database.MaxIdentifierLength);

            var constraintName = primaryKeyName ?? database.NamingConvention.GetPrimaryKeyNamingConvention(TableName, columnNames);

            database.AddPrimaryKey(TableName, columnNames, constraintName);
        }

        /// <summary>
        /// Remove the primary key.
        /// </summary>
        public void RemovePrimaryKey()
        {
            var constraintName = database.GetPrimaryKeyName(TableName);
            if (constraintName == null)
            {
                throw new InvalidOperationException($"No primary key was found on table {TableName}.");
            }

            database.RemovePrimaryKey(TableName, constraintName);
        }

        /// <summary>
        /// Rename the table.
        /// </summary>
        /// <param name="newTableName">New table name.</param>
        public void Rename(string newTableName)
        {
            Validate.IsNotNullAndNotEmpty(newTableName, nameof(newTableName));
            Validate.MaxLength(newTableName, nameof(newTableName), database.MaxIdentifierLength);

            database.RenameTable(TableName, newTableName);
        }
    }
}
