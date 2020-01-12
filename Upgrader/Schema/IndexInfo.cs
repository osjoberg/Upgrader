namespace Upgrader.Schema
{
    /// <summary>
    /// Index information.
    /// </summary>
    public class IndexInfo
    {
        private readonly Database database;

        public IndexInfo(Database database, string tableName, string indexName)
        {
            this.database = database;
            TableName = tableName;
            IndexName = indexName;
        }

        /// <summary>
        /// Gets index name.
        /// </summary>
        public string IndexName { get; }

        /// <summary>
        /// Gets table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets column names.
        /// </summary>
        /// <returns>Column names.</returns>
        public string[] GetColumnNames() => database.GetIndexColumnNames(TableName, IndexName);

        /// <summary>
        /// Gets if index is unique.
        /// </summary>
        /// <returns>True if the index is unique, otherwise False.</returns>
        public bool IsUnique() => database.GetIndexType(TableName, IndexName);
    }
}