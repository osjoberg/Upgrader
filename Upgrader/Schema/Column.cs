using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Represents a new column in the database.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class with a column name and a data type.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="dataType">SQL data type.</param>
        public Column(string columnName, string dataType) : this(columnName, dataType, ColumnModifier.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the<see cref="Column"/> class with a column name, data type and a modifier.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="modifier">Column modifier.</param>
        public Column(string columnName, string dataType, ColumnModifier modifier)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            ColumnName = columnName;
            DataType = dataType;
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
        /// Gets the column modifier.
        /// </summary>
        public ColumnModifier Modifier { get; }
    }
}
