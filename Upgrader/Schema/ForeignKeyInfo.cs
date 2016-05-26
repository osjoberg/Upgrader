namespace Upgrader.Schema
{
    public class ForeignKeyInfo
    {
        private readonly Database database;

        internal ForeignKeyInfo(Database database, string tableName, string foreignKeyName)
        {
            this.database = database;
            TableName  = tableName;
            ForeignKeyName = foreignKeyName;
        }

        public string TableName { get; }

        public string ForeignKeyName { get; }

        public string[] ColumnNames => database.GetForeignKeyColumnNames(TableName, ForeignKeyName);

        public string ForeignTable => database.GetForeignKeyForeignTableName(TableName, ForeignKeyName);

        public string[] ForeignColumnNames => database.GetForeignKeyForeignColumnNames(TableName, ForeignKeyName);
    }
}