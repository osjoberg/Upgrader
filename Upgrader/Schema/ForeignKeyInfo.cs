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
        /// <returns>Column names.</returns>
        public string[] GetColumnNames() => database.GetForeignKeyColumnNames(TableName, ForeignKeyName);

        /// <summary>
        /// Gets foreign table name.
        /// </summary>
        /// <returns>Foreign table name.</returns>
        public string GetForeignTableName() => database.GetForeignKeyForeignTableName(TableName, ForeignKeyName);

        /// <summary>
        /// Gets foreign column names.
        /// </summary>
        /// <returns>Foreign column names.</returns>
        public string[] GetForeignColumnNames() => database.GetForeignKeyForeignColumnNames(TableName, ForeignKeyName);
    }
}