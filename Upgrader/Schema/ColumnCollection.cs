using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Collection of all columns in the specified table.
    /// </summary>
    public class ColumnCollection : IEnumerable<ColumnInfo>
    {
        private readonly Database database;
        private readonly string tableName;

        internal ColumnCollection(Database database, string tableName)
        {
            this.database = database;
            this.tableName = tableName;
        }

        /// <summary>
        /// Gets column information for the specified column. Returns null if the specified column name does not exist.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Column information.</returns>
        public ColumnInfo this[string columnName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
                Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);

                return database.GetColumnDataType(tableName, columnName) != null ? new ColumnInfo(database, tableName, columnName) : null;
            }
        }

        /// <summary>
        /// Gets an IEnumerator of all columns.
        /// </summary>
        /// <returns>IEnumerator of all columns.</returns>
        public IEnumerator<ColumnInfo> GetEnumerator()
        {
            return database
                .GetColumnNames(tableName)
                .Select(columnName => new ColumnInfo(database, tableName, columnName))
                .GetEnumerator();
        }

        /// <summary>
        /// Gets an IEnumerator of all columns.
        /// </summary>
        /// <returns>IEnumerator of all columns.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a nullable column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="dataType">SQL data type.</param>
        public void AddNullable(string columnName, string dataType)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);

            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            database.AddColumn(tableName, columnName, dataType, true);
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="initialValue">Initial value to set to all existing rows.</param>
        public void Add(string columnName, string dataType, object initialValue = null)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            if (initialValue == null)
            {
                database.AddColumn(tableName, columnName, dataType, false);
                return;
            }

            database.AddColumn(tableName, columnName, dataType, true);
            database.SetColumnValue(tableName, columnName, initialValue);
            database.ChangeColumn(tableName, columnName, dataType, false);
        }

        /// <summary>
        /// Removes a column from the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        public void Remove(string columnName)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);

            database.RemoveColumn(tableName, columnName);
        }
    }
}
