using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    using System.Linq;

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
        /// Gets column modifier.
        /// </summary>
        public ColumnModifier Modifier
        {
            get
            {
                if (database.GetColumnNullable(TableName, ColumnName))
                {
                    return ColumnModifier.Nullable;
                }

                var primaryKeyName = database.GetPrimaryKeyName(TableName);
                if (primaryKeyName != null && database.GetPrimaryKeyColumnNames(TableName, primaryKeyName).Contains(ColumnName))
                {
                    return database.GetColumnAutoIncrement(TableName, ColumnName) ? ColumnModifier.AutoIncrementPrimaryKey : ColumnModifier.PrimaryKey;
                }

                return ColumnModifier.None;
            }
        }

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
        /// <param name="newDataType">New SQL data type.</param>
        public void ChangeType(string newDataType)
        {
            ChangeType(newDataType, Modifier);
        }

        /// <summary>
        /// Change type of column.
        /// </summary>
        /// <param name="modifier">New modifier. Only ColumnModifier.None and ColumnModifier.Nullable are valid.</param>
        public void ChangeType(ColumnModifier modifier)
        {
            ChangeType(DataType, modifier);
        }

        /// <summary>
        /// Change type of column.
        /// </summary>
        /// <param name="newDataType">New SQL data type.</param>
        /// <param name="modifier">New modifier. Only ColumnModifier.None and ColumnModifier.Nullable are valid.</param>
        public void ChangeType(string newDataType, ColumnModifier modifier)
        {
            Validate.IsNotNullAndNotEmpty(newDataType, nameof(newDataType));
            Validate.IsTrue(modifier == ColumnModifier.None || modifier == ColumnModifier.Nullable, nameof(modifier), "Only ColumnModifier.None and ColumnModifier.Nullable is allowed.");

            database.ChangeColumn(TableName, ColumnName, newDataType, modifier == ColumnModifier.Nullable);
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
