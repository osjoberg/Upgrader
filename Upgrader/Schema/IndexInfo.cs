namespace Upgrader.Schema
{
    public class IndexInfo
    {
        private readonly Database database;

        public IndexInfo(Database database, string tableName, string indexName)
        {
            this.database = database;
            TableName = tableName;
            IndexName = indexName;
        }

        public string IndexName { get; }

        public string TableName { get; }

        public string[] ColumnNames => database.GetIndexColumnNames(TableName, IndexName);

        public bool Unique => database.GetIndexType(TableName, IndexName);
    }
}