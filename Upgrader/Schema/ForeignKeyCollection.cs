using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    public class ForeignKeyCollection : IEnumerable<ForeignKeyInfo>
    {
        private readonly Database database;
        private readonly string tableName;

        internal ForeignKeyCollection(Database database, string tableName)
        {
            this.database = database;
            this.tableName = tableName;
        }

        public ForeignKeyInfo this[string foreignKeyName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(foreignKeyName, nameof(foreignKeyName));
                Validate.MaxLength(foreignKeyName, nameof(foreignKeyName), database.MaxIdentifierLength);

                return database.GetForeignKeyForeignTableName(tableName, foreignKeyName) != null ? new ForeignKeyInfo(database, tableName, foreignKeyName) : null;
            }
        }

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

        public void Add(string[] columnNames, string foreignTableName, string[] foreignColumnNames = null, string foreignKeyName = null)
        {
            Validate.IsNotNullAndNotEmptyEnumerable(columnNames, nameof(columnNames));
            Validate.MaxLength(columnNames, nameof(columnNames), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(foreignTableName, nameof(foreignTableName));
            Validate.MaxLength(foreignTableName, nameof(foreignTableName), database.MaxIdentifierLength);
            Validate.MaxLength(foreignColumnNames, nameof(foreignColumnNames), database.MaxIdentifierLength);
            Validate.IsNotEmpty(foreignKeyName, nameof(foreignKeyName));
            Validate.MaxLength(foreignKeyName, nameof(foreignKeyName), database.MaxIdentifierLength);

            var constaintName = foreignKeyName ?? database.NamingConvention.GetForeignKeyNamingConvention(tableName, columnNames, foreignTableName);

            database.AddForeignKey(tableName, columnNames, foreignTableName, foreignColumnNames ?? columnNames, constaintName);
        }

        public void Remove(string foreignKeyName)
        {
            Validate.IsNotNullAndNotEmpty(foreignKeyName, nameof(foreignKeyName));
            Validate.MaxLength(foreignKeyName, nameof(foreignKeyName), database.MaxIdentifierLength);

            database.RemoveForeignKey(tableName, foreignKeyName);
        }

        public void RemoveAll()
        {
            foreach (var foreignKey in this)
            {
                Remove(foreignKey.ForeignKeyName);
            }
        }

        public IEnumerator<ForeignKeyInfo> GetEnumerator()
        {
            return database
                .GetForeignKeyNames(tableName)
                .Select(constraintName => new ForeignKeyInfo(database, tableName, constraintName))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}