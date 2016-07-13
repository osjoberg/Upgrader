using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Represents a new column in the database.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class as a non-null column with a column name, data type and with a modifier.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="modifier">Column modifiers.</param>
        public Column(string columnName, string dataType, ColumnModifier modifier) : this(columnName, dataType, false, modifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        public Column(string columnName, string dataType, bool nullable = false) : this(columnName, dataType, nullable, ColumnModifier.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the<see cref="Column"/> class with a column name, data type, nullable and with a modifier.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        /// <param name="modifier">Column modifiers.</param>
        private Column(string columnName, string dataType, bool nullable, ColumnModifier modifier)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            ColumnName = columnName;
            DataType = dataType;
            Nullable = nullable;
            Modifier = modifier;
        }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the column data type.
        /// </summary>
        public string DataType { get; }

        /// <summary>
        /// Gets if the column is nullable or not.
        /// </summary>
        public bool Nullable { get; }

        /// <summary>
        /// Gets the column modifier.
        /// </summary>
        public ColumnModifier Modifier { get; }
    }
}
