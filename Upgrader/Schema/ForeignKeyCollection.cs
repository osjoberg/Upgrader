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

                return database.GetForeignKeyForeignTableName(tableName, foreignKeyName) != null ? new ForeignKeyInfo(database, tableName, foreignKeyName) : null;
            }
        }

        public void Add(string columnName, string foreignTableName, string foreignColumn = null, string foreignKeyName = null)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsNotEmpty(foreignColumn, nameof(foreignColumn));

            Add(new[] { columnName }, foreignTableName, new[] { foreignColumn ?? columnName }, foreignKeyName);
        }

        public void Add(string[] columnNames, string foreignTableName, string[] foreignColumnNames = null, string foreignKeyName = null)
        {
            Validate.IsNotNullAndNotEmptyEnumerable(columnNames, nameof(columnNames));
            Validate.IsNotNullAndNotEmpty(foreignTableName, nameof(tableName));

            var constaintName = foreignKeyName ?? database.NamingConvention.GetForeignKeyNamingConvention(tableName, columnNames, foreignTableName);

            database.AddForeignKey(tableName, columnNames, foreignTableName, foreignColumnNames ?? columnNames, constaintName);
        }

        public void Remove(string foreignKeyName)
        {
            Validate.IsNotNullAndNotEmpty(foreignKeyName, nameof(foreignKeyName));

            database.RemoveForeignKey(tableName, foreignKeyName);
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