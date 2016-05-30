using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    public class ColumnInfo
    {
        private readonly Database database;

        public string DataType => database.GetColumnDataType(TableName, ColumnName);

        public bool Nullable => database.GetColumnNullable(TableName, ColumnName);

        public bool AutoIncrement => database.GetColumnAutoIncrement(TableName, ColumnName);

        public string ColumnName { get; }

        public string TableName { get; }

        internal ColumnInfo(Database database, string tableName, string columnColumnName)
        {            
            this.database = database;
            TableName = tableName;
            ColumnName = columnColumnName;
        }

        public void ChangeType(string type)
        {
            Validate.IsNotNullAndNotEmpty(type, nameof(type));

            database.ChangeColumn(TableName, ColumnName, type, Nullable);
        }

        public void ChangeType(string type, bool nullable)
        {
            Validate.IsNotNullAndNotEmpty(type, nameof(type));

            database.ChangeColumn(TableName, ColumnName, type, nullable);
        }
           
        public void Rename(string newColumnName)
        {
            Validate.IsNotNullAndNotEmpty(newColumnName, nameof(newColumnName));

            database.RenameColumn(TableName, ColumnName, newColumnName);
        }
    }
}
