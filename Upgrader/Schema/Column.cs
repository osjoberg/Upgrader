using System.Linq;

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
        internal Column(string columnName, string dataType, bool nullable, ColumnModifier modifier)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            ColumnName = columnName;
            DataType = dataType;
            Nullable = nullable;
            Modifier = modifier;
        }

        /// <summary>
        /// Initializes a new instance of the<see cref="Column"/> class with a column name, data type, nullable and with a modifier.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        /// <param name="modifier">Column modifiers.</param>
        internal Column(string columnName, bool nullable, ColumnModifier modifier)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));

            ColumnName = columnName;
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
        /// Gets a value indicating whether the column is nullable or not.
        /// </summary>
        public bool Nullable { get; }

        /// <summary>
        /// Gets the column modifier.
        /// </summary>
        public ColumnModifier Modifier { get; }

        internal virtual string GetDataType(TypeMappingCollection typeMappings)
        {
            return DataType;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Represents a new column in the database.
    /// </summary>
    public class Column<TType> : Column
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="Column{TType}"/> class as a non-null column with a column name, data type and with a modifier.
        /// </summary>
        /// <param name="columnName">
        /// Name of the new column.
        /// </param>
        public Column(string columnName) : this(columnName, ColumnModifier.None)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="Column{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="modifier">Column modifiers.</param>
        public Column(string columnName, ColumnModifier modifier) : base(columnName, System.Nullable.GetUnderlyingType(typeof(TType)) != null, modifier)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="Column{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="length">Length of data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        public Column(string columnName, int length, bool nullable = false) : this(columnName, length, nullable, ColumnModifier.None)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="Column{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="length">Length of data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        /// <param name="modifier">Column modifiers.</param>
        public Column(string columnName, int length, bool nullable, ColumnModifier modifier) : base(columnName, nullable, modifier)
        {
            Validate.IsEqualOrGreaterThan(length, 1, nameof(length));

            Length = length;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="Column{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="scale">Scale of data type.</param>
        /// <param name="precision">Precision of data type.</param>
        public Column(string columnName, int scale, int precision) : this(columnName, scale, precision, ColumnModifier.None)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="Column{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="scale">Scale of data type.</param>
        /// <param name="precision">Precision of data type.</param>
        /// <param name="modifier">Column modifiers.</param>
        public Column(string columnName, int scale, int precision, ColumnModifier modifier) : base(columnName, System.Nullable.GetUnderlyingType(typeof(TType)) != null, modifier)
        {
            Validate.IsEqualOrGreaterThan(scale, 1, nameof(scale));
            Validate.IsEqualOrGreaterThan(precision, 0, nameof(precision));

            Scale = scale;
            Precision = precision;
        }

        /// <summary>
        /// Gets length of data type.
        /// </summary>
        public int? Length { get;  }

        /// <summary>
        /// Gets scale of data type.
        /// </summary>
        public int? Scale { get;  }

        /// <summary>
        /// Gets precision of data type.
        /// </summary>
        public int? Precision { get; }

        internal override string GetDataType(TypeMappingCollection typeMappings)
        {
            var parameters = new[] { Length, Scale, Precision }.Where(parameter => parameter != null).Select(parameter => parameter.Value).ToArray();

            return typeMappings.GetDataType(typeof(TType), parameters);
        }
    }
}
