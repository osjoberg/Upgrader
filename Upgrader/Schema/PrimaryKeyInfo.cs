namespace Upgrader.Schema
{
    public class PrimaryKeyInfo
    {
        private readonly Database database;

        internal PrimaryKeyInfo(Database database, string tableName, string primaryKeyName)
        {
            this.database = database;
            TableName = tableName;
            PrimaryKeyName = primaryKeyName;
        }

        public string TableName { get; }

        public string PrimaryKeyName { get; }

        public string[] ColumnNames => database.GetPrimaryKeyColumnNames(TableName, PrimaryKeyName);
    }
}
