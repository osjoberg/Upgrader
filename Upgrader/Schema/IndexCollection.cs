using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    public class IndexCollection : IEnumerable<IndexInfo>
    {
        private readonly Database database;
        private readonly string tableName;

        internal IndexCollection(Database database, string tableName)
        {
            this.database = database;
            this.tableName = tableName;
        }

        public IndexInfo this[string indexName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(indexName, nameof(indexName));
                Validate.MaxLength(indexName, nameof(indexName), database.MaxIdentifierLength);

                return database.GetIndexColumnNames(tableName, indexName).Any() ? new IndexInfo(database, tableName, indexName) : null;
            }
        }

        public void Add(string columnName, string indexName = null)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));

            Add(new[] { columnName }, false, indexName);
        }

        public void Add(string[] columnNames, string indexName = null)
        {
            Add(columnNames, false, indexName);
        }

        public void AddUnique(string columnName, string indexName = null)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));

            Add(new[] { columnName }, true, indexName);
        }

        public void AddUnique(string[] columnNames, string indexName = null)
        {
            Add(columnNames, true, indexName);
        }

        private void Add(string[] columnNames, bool unique, string indexName = null)
        {
            Validate.IsNotNullAndNotEmptyEnumerable(columnNames, nameof(columnNames));
            Validate.MaxLength(columnNames, nameof(columnNames), database.MaxIdentifierLength);
            Validate.IsNotEmpty(indexName, nameof(indexName));
            Validate.MaxLength(indexName, nameof(indexName), database.MaxIdentifierLength);

            indexName = indexName ?? database.NamingConvention.GetIndexNamingConvention(tableName, columnNames, unique);
            database.AddIndex(tableName, columnNames, unique, indexName);
        }

        public void Remove(string indexName)
        {
            Validate.IsNotNullAndNotEmpty(indexName, nameof(indexName));
            Validate.MaxLength(indexName, nameof(indexName), database.MaxIdentifierLength);

            database.RemoveIndex(tableName, indexName);
        }

        public IEnumerator<IndexInfo> GetEnumerator()
        {
            return database
                .GetIndexNames(tableName)
                .Select(indexName => new IndexInfo(database, tableName, indexName))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}