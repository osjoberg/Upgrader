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
        public string[] ColumnNames => database.GetIndexColumnNames(TableName, IndexName);

        /// <summary>
        /// Gets if index is unique.
        /// </summary>
        public bool Unique => database.GetIndexType(TableName, IndexName);
    }
}