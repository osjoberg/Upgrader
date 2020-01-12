using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Collection of all indexes in the specified table.
    /// </summary>
    public class IndexCollection : IEnumerable<IndexInfo>
    {
        private readonly Database database;
        private readonly string tableName;

        internal IndexCollection(Database database, string tableName)
        {
            this.database = database;
            this.tableName = tableName;
        }

        /// <summary>
        /// Gets index information for the specified index. Returns null if the specified index name does not exist.
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <returns>Index information.</returns>
        public IndexInfo this[string indexName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(indexName, nameof(indexName));
                Validate.MaxLength(indexName, nameof(indexName), database.MaxIdentifierLength);

                return database.GetIndexColumnNames(tableName, indexName).Any() ? new IndexInfo(database, tableName, indexName) : null;
            }
        }

        /// <summary>
        /// Gets an IEnumerator of all indexes.
        /// </summary>
        /// <returns>IEnumerator of all indexes.</returns>
        public IEnumerator<IndexInfo> GetEnumerator()
        {
            return database
                .GetIndexNames(tableName)
                .Select(indexName => new IndexInfo(database, tableName, indexName))
                .GetEnumerator();
        }

        /// <summary>
        /// Gets an IEnumerator of all indexes.
        /// </summary>
        /// <returns>IEnumerator of all indexes.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Checks if an index exists for the table.
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <returns>True, if the index exists or False if it does not exist.</returns>
        public bool Exists(string indexName)
        {
            return database.GetIndexColumnNames(tableName, indexName).Any();
        }

        /// <summary>
        /// Adds an index to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="unique">True to create a unique index.</param>
        /// <param name="indexName">Index name. If not name is given, name is set by convention.</param>
        /// <param name="includeColumnNames">Include additional data columns in index. If no include column names are given, index refers to the entire row.</param>
        public void Add(string columnName, bool unique = false, string indexName = null, string[] includeColumnNames = null)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));

            Add(new[] { columnName }, unique, indexName, includeColumnNames);
        }

        /// <summary>
        /// Adds an index to the table.
        /// </summary>
        /// <param name="columnNames">Column names.</param>
        /// <param name="unique">True to create a unique index.</param>
        /// <param name="indexName">Index name. If not name is given, name is set by convention.</param>
        /// <param name="includeColumnNames">Include additional data columns in index. If no include column names are given, index refers to the entire row.</param>
        public void Add(string[] columnNames, bool unique = false, string indexName = null, string[] includeColumnNames = null)
        {
            Validate.IsNotNullAndNotEmptyEnumerable(columnNames, nameof(columnNames));
            Validate.MaxLength(columnNames, nameof(columnNames), database.MaxIdentifierLength);
            Validate.IsNotEmpty(indexName, nameof(indexName));
            Validate.MaxLength(indexName, nameof(indexName), database.MaxIdentifierLength);

            indexName = indexName ?? database.NamingConvention.GetIndexNamingConvention(tableName, columnNames, unique);
            database.AddIndex(tableName, columnNames, unique, indexName, includeColumnNames);
        }

        /// <summary>
        /// Removes an index from the table.
        /// </summary>
        /// <param name="indexName">Index name.</param>
        public void Remove(string indexName)
        {
            Validate.IsNotNullAndNotEmpty(indexName, nameof(indexName));
            Validate.MaxLength(indexName, nameof(indexName), database.MaxIdentifierLength);

            database.RemoveIndex(tableName, indexName);
        }

        /// <summary>
        /// Removes all indexes from the table.
        /// </summary>
        public void RemoveAll()
        {
            foreach (var index in this)
            {
                Remove(index.IndexName);
            }
        }
    }
}