using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    public class TableCollection : IEnumerable<TableInfo>
    {
        private readonly Database database;

        internal TableCollection(Database database)
        {
            this.database = database;
        }

        public TableInfo this[string tableName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(tableName, nameof(tableName));
                Validate.MaxLength(tableName, nameof(tableName), database.MaxIdentifierLength);

                return database.GetColumnNames(tableName).Any() ? new TableInfo(database, tableName) : null;
            }
        }

        public IEnumerator<TableInfo> GetEnumerator()
        {
            return database
                .GetTableNames()
                .Select(tableName => new TableInfo(database, tableName))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string tableName, Column[] columns, ForeignKey[] foreignKeys)
        {
            Validate.IsNotNullAndNotEmpty(tableName, nameof(tableName));
            Validate.MaxLength(tableName, nameof(tableName), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(columns, nameof(columns));
            Validate.MaxLength(columns.Select(column => column.ColumnName), nameof(columns), database.MaxIdentifierLength);
            Validate.IsNotNull(foreignKeys, nameof(foreignKeys));
            Validate.MaxLength(foreignKeys.Select(foreignKey => foreignKey.ForeignKeyName), nameof(columns), database.MaxIdentifierLength);

            database.AddTable(tableName, columns, foreignKeys);
        }

        public void Add(string tableName, params Column[] columns)
        {
            Add(tableName, columns, new ForeignKey[] { });
        }

        public void Remove(string tableName)
        {
            Validate.IsNotNullAndNotEmpty(tableName, nameof(tableName));
            Validate.MaxLength(tableName, nameof(tableName), database.MaxIdentifierLength);

            database.RemoveTable(tableName);
        }
    }
}
