namespace Upgrader.Schema
{
    /// <summary>
    /// Primary key information.
    /// </summary>
    public class PrimaryKeyInfo
    {
        private readonly Database database;

        internal PrimaryKeyInfo(Database database, string tableName, string primaryKeyName)
        {
            this.database = database;
            TableName = tableName;
            PrimaryKeyName = primaryKeyName;
        }

        /// <summary>
        /// Gets table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets primary key name.
        /// </summary>
        public string PrimaryKeyName { get; }

        /// <summary>
        /// Gets column names.
        /// </summary>
        public string[] ColumnNames => database.GetPrimaryKeyColumnNames(TableName, PrimaryKeyName);
    }
}
