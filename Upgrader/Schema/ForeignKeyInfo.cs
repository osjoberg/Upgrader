namespace Upgrader.Schema
{
    /// <summary>
    /// Foreign key information.
    /// </summary>
    public class ForeignKeyInfo
    {
        private readonly Database database;

        internal ForeignKeyInfo(Database database, string tableName, string foreignKeyName)
        {
            this.database = database;
            TableName  = tableName;
            ForeignKeyName = foreignKeyName;
        }

        /// <summary>
        /// Gets table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets foreign key name.
        /// </summary>
        public string ForeignKeyName { get; }

        /// <summary>
        /// Gets column names.
        /// </summary>
        public string[] ColumnNames => database.GetForeignKeyColumnNames(TableName, ForeignKeyName);

        /// <summary>
        /// Gets foreign tabbe name name.
        /// </summary>
        public string ForeignTableName => database.GetForeignKeyForeignTableName(TableName, ForeignKeyName);

        /// <summary>
        /// Gets foreign column names.
        /// </summary>
        public string[] ForeignColumnNames => database.GetForeignKeyForeignColumnNames(TableName, ForeignKeyName);
    }
}