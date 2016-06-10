namespace Upgrader.Schema
{
    public class ForeignKey
    {
        public ForeignKey(string columnName, string foreignTableName) : this(new[] { columnName }, foreignTableName, new[] { columnName })
        {            
        }

        public ForeignKey(string[] columnNames, string foreignTableName, string foreignConstraintName = null) : this(columnNames, foreignTableName, columnNames, foreignConstraintName)
        {            
        }

        public ForeignKey(string columnName, string foreignTableName, string foreignColumnName, string foreignKeyName = null) : this(new[] { columnName }, foreignTableName, new[] { foreignColumnName }, foreignKeyName)
        {            
        }

        public ForeignKey(string[] columnNames, string foreignTableName, string[] foreignColumnNames, string foreignKeyName = null)
        {
            ForeignKeyName = foreignKeyName;
            ForeignTableName = foreignTableName;
            ColumnNames = columnNames;
            ForeignColumnNames = foreignColumnNames;
        }

        public string ForeignKeyName { get; }

        public string ForeignTableName { get; }

        public string[] ColumnNames { get; }

        public string[] ForeignColumnNames { get; }
    }
}
