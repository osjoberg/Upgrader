using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Collection of all foreign keys in the specified table.
    /// </summary>
    public class ForeignKeyCollection : IEnumerable<ForeignKeyInfo>
    {
        private readonly Database database;
        private readonly string tableName;

        internal ForeignKeyCollection(Database database, string tableName)
        {
            this.database = database;
            this.tableName = tableName;
        }

        /// <summary>
        /// Gets foreign key information for the specified foreign key. Returns null if the specified foreign key does not exist.
        /// </summary>
        /// <param name="foreignKeyName">Foreign key name.</param>
        /// <returns>Foreign key information.</returns>
        public ForeignKeyInfo this[string foreignKeyName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(foreignKeyName, nameof(foreignKeyName));
                Validate.MaxLength(foreignKeyName, nameof(foreignKeyName), database.MaxIdentifierLength);

                return new ForeignKeyInfo(database, tableName, foreignKeyName);
            }
        }

        /// <summary>
        /// Gets an IEnumerator of all foreign keys.
        /// </summary>
        /// <returns>IEnumerator of all foreign keys.</returns>
        public IEnumerator<ForeignKeyInfo> GetEnumerator()
        {
            return database
                .GetForeignKeyNames(tableName)
                .Select(constraintName => new ForeignKeyInfo(database, tableName, constraintName))
                .GetEnumerator();
        }

        /// <summary>
        /// Gets an IEnumerator of all foreign keys.
        /// </summary>
        /// <returns>IEnumerator of all foreign keys.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Checks if a foreign key exists.
        /// </summary>
        /// <param name="foreignKeyName">Foreign key name.</param>
        /// <returns>True, if the foreign key exists or False if it does not exist.</returns>
        public bool Exists(string foreignKeyName)
        {
            return database.GetForeignKeyForeignTableName(tableName, foreignKeyName) != null;
        }

        /// <summary>
        /// Adds a foreign key to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="foreignTableName">Foreign table name.</param>
        /// <param name="foreignColumnName">Foreign column name. If no foreign column name is set, the <see cref="columnName"/> will be used.</param>
        /// <param name="foreignKeyName">Foreign key name. If not name is given, name is set by convention.</param>
        public void Add(string columnName, string foreignTableName, string foreignColumnName = null, string foreignKeyName = null)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(foreignTableName, nameof(foreignTableName));
            Validate.MaxLength(foreignTableName, nameof(foreignTableName), database.MaxIdentifierLength);
            Validate.IsNotEmpty(foreignColumnName, nameof(foreignColumnName));
            Validate.MaxLength(foreignColumnName, nameof(foreignColumnName), database.MaxIdentifierLength);
            Validate.IsNotEmpty(foreignKeyName, nameof(foreignKeyName));
            Validate.MaxLength(foreignKeyName, nameof(foreignKeyName), database.MaxIdentifierLength);

            Add(new[] { columnName }, foreignTableName, new[] { foreignColumnName ?? columnName }, foreignKeyName);
        }

        /// <summary>
        /// Adds a foreign key to the table.
        /// </summary>
        /// <param name="columnNames">Column names.</param>
        /// <param name="foreignTableName">Foreign table name.</param>
        /// <param name="foreignColumnNames">Foreign column names. If no foreign column names is set, the <see cref="columnNames"/> will be used.</param>
        /// <param name="foreignKeyName">Foreign key name. If not name is given, name is set by convention.</param>
        public void Add(string[] columnNames, string foreignTableName, string[] foreignColumnNames = null, string foreignKeyName = null)
        {
            Validate.IsNotNullAndNotEmptyEnumerable(columnNames, nameof(columnNames));
            Validate.MaxLength(columnNames, nameof(columnNames), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(foreignTableName, nameof(foreignTableName));
            Validate.MaxLength(foreignTableName, nameof(foreignTableName), database.MaxIdentifierLength);
            Validate.MaxLength(foreignColumnNames, nameof(foreignColumnNames), database.MaxIdentifierLength);
            Validate.IsNotEmpty(foreignKeyName, nameof(foreignKeyName));
            Validate.MaxLength(foreignKeyName, nameof(foreignKeyName), database.MaxIdentifierLength);

            var constraintName = foreignKeyName ?? database.NamingConvention.GetForeignKeyNamingConvention(tableName, columnNames, foreignTableName);

            database.AddForeignKey(tableName, columnNames, foreignTableName, foreignColumnNames ?? columnNames, constraintName);
        }

        /// <summary>
        /// Removes a foreign key from the table.
        /// </summary>
        /// <param name="foreignKeyName">Foreign key name.</param>
        public void Remove(string foreignKeyName)
        {
            Validate.IsNotNullAndNotEmpty(foreignKeyName, nameof(foreignKeyName));
            Validate.MaxLength(foreignKeyName, nameof(foreignKeyName), database.MaxIdentifierLength);

            database.RemoveForeignKey(tableName, foreignKeyName);
        }

        /// <summary>
        /// Removes all foreign keys from the table.
        /// </summary>
        public void RemoveAll()
        {
            foreach (var foreignKey in this)
            {
                Remove(foreignKey.ForeignKeyName);
            }
        }
    }
}