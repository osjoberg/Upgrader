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

        public void Add(string columnName, string type, bool nullable = false, bool autoIncrement = false)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsNotNullAndNotEmpty(type, nameof(type));

            database.AddColumn(tableName, columnName, type, nullable, autoIncrement);            
        }

        public void Remove(string columnName)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));

            database.RemoveColumn(tableName, columnName);
        }
    }
}
