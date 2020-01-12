using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <inheritdoc />
    /// <summary>
    /// Collection of all tables in the connected database.
    /// </summary>
    public class TableCollection : IEnumerable<TableInfo>
    {
        private readonly Database database;

        internal TableCollection(Database database)
        {
            this.database = database;
        }

        /// <summary>
        /// Gets table information for the specified table. Returns null if the specified table does not exist.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <returns>Table information.</returns>
        public TableInfo this[string tableName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(tableName, nameof(tableName));
                Validate.MaxLength(tableName, nameof(tableName), database.MaxIdentifierLength);

                return new TableInfo(database, tableName);
            }
        }

        /// <summary>
        /// Gets an IEnumerator of all tables.
        /// </summary>
        /// <returns>IEnumerator of all tables.</returns>
        public IEnumerator<TableInfo> GetEnumerator()
        {
            return database
                .GetTableNames()
                .Select(tableName => new TableInfo(database, tableName))
                .GetEnumerator();
        }

        /// <summary>
        /// Gets an IEnumerator of all tables.
        /// </summary>
        /// <returns>IEnumerator of all tables.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Checks if a table exists.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <returns>True, if the table exists or False if it does not exist.</returns>
        public bool Exists(string tableName)
        {
            return database.GetColumnNames(tableName).Any();
        }

        /// <summary>
        /// Adds a table to the database.
        /// </summary>
        /// <param name="tableName">Table names.</param>
        /// <param name="columns">Column definitions. At least one column definition needs to be specified.</param>
        /// <param name="foreignKeys">Foreign key definitions.</param>
        public void Add(string tableName, Column[] columns, ForeignKey[] foreignKeys)
        {
            Validate.IsNotNullAndNotEmpty(tableName, nameof(tableName));
            Validate.MaxLength(tableName, nameof(tableName), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(columns, nameof(columns));
            Validate.MaxLength(columns.Select(column => column.ColumnName), nameof(columns), database.MaxIdentifierLength);
            Validate.IsNotNull(foreignKeys, nameof(foreignKeys));
            Validate.MaxLength(foreignKeys.Select(foreignKey => foreignKey.ForeignKeyName), nameof(columns), database.MaxIdentifierLength);

            var firstInvalidColumn = columns.FirstOrDefault(column => column.GetDataType(database.TypeMappings) == null);
            if (firstInvalidColumn != null)
            {
                var type = firstInvalidColumn.GetType().GetGenericArguments().Single();

                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(columns));
            }

            database.AddTable(tableName, columns, foreignKeys);
        }

        /// <summary>
        /// Rename table.
        /// </summary>
        /// <param name="currentTableName">Current table name.</param>
        /// <param name="newTableName">New table name.</param>
        public void Rename(string currentTableName, string newTableName)
        {
            Validate.IsNotNullAndNotEmpty(newTableName, nameof(newTableName));
            Validate.MaxLength(newTableName, nameof(newTableName), database.MaxIdentifierLength);

            database.RenameTable(currentTableName, newTableName);
        }

        /// <summary>
        /// Adds a table to the database.
        /// </summary>
        /// <param name="tableName">Table names.</param>
        /// <param name="columns">Column definitions. At least one column definition needs to be specified.</param>
        public void Add(string tableName, params Column[] columns)
        {
            Add(tableName, columns, new ForeignKey[] { });
        }

        /// <summary>
        /// Removes a table from the database.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        public void Remove(string tableName)
        {
            Validate.IsNotNullAndNotEmpty(tableName, nameof(tableName));
            Validate.MaxLength(tableName, nameof(tableName), database.MaxIdentifierLength);

            database.RemoveTable(tableName);
        }

        /// <summary>
        /// Removes all tables from the database.
        /// </summary>
        public void RemoveAll()
        {
            foreach (var table in this)
            {
                table.ForeignKeys.RemoveAll();
            }

            foreach (var table in this)
            {
                Remove(table.TableName);
            }
        }
    }
}
