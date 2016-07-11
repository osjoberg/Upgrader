using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Column information and modification.
    /// </summary>
    public class ColumnInfo
    {
        private readonly Database database;

        /// <summary>
        /// Gets column SQL data type.
        /// </summary>
        public string DataType => database.GetColumnDataType(TableName, ColumnName);

        /// <summary>
        /// Gets if column is nullable.
        /// </summary>
        public bool Nullable => database.GetColumnNullable(TableName, ColumnName);

        /// <summary>
        /// Gets if column is configured to auto increment.
        /// </summary>
        public bool AutoIncrement => database.GetColumnAutoIncrement(TableName, ColumnName);

        /// <summary>
        /// Gets column name.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets table name.
        /// </summary>
        public string TableName { get; }

        internal ColumnInfo(Database database, string tableName, string columnColumnName)
        {            
            this.database = database;
            TableName = tableName;
            ColumnName = columnColumnName;
        }

        /// <summary>
        /// Change type of column.
        /// </summary>
        /// <param name="type">SQL data type.</param>
        public void ChangeType(string type)
        {
            Validate.IsNotNullAndNotEmpty(type, nameof(type));

            database.ChangeColumn(TableName, ColumnName, type, Nullable);
        }

        /// <summary>
        /// Change type of column.
        /// </summary>
        /// <param name="type">SQL data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        public void ChangeType(string type, bool nullable)
        {
            Validate.IsNotNullAndNotEmpty(type, nameof(type));

            database.ChangeColumn(TableName, ColumnName, type, nullable);
        }

        /// <summary>
        /// Rename column.
        /// </summary>
        /// <param name="newColumnName">New name of column.</param>
        public void Rename(string newColumnName)
        {
            Validate.IsNotNullAndNotEmpty(newColumnName, nameof(newColumnName));

            database.RenameColumn(TableName, ColumnName, newColumnName);
        }
    }
}
