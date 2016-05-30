using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    public class ColumnCollection : IEnumerable<ColumnInfo>
    {
        private readonly Database database;
        private readonly string tableName;

        internal ColumnCollection(Database database, string tableName)
        {
            this.database = database;
            this.tableName = tableName;
        }

        public ColumnInfo this[string columnName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
                Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);

                return database.GetColumnDataType(tableName, columnName) != null ? new ColumnInfo(database, tableName, columnName) : null;
            }
        }

        public IEnumerator<ColumnInfo> GetEnumerator()
        {
            return database
                .GetColumnNames(tableName)
                .Select(columnName => new ColumnInfo(database, tableName, columnName))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddNullable(string columnName, string dataType)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);

            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            database.AddColumn(tableName, columnName, dataType, true);
        }

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

        public void Remove(string columnName)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);

            database.RemoveColumn(tableName, columnName);
        }
    }
}
