using System.Linq;

using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Represents a new computed column in the database.
    /// </summary>
    public class ComputedColumn : Column
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedColumn"/> class as a non-null column with a column name, data type and a non-persisted expression.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="expression">Expression representing the computed computed value of the column.</param>
        /// <param name="persisted">True to store the computed value in the table, False to re-compute the value whenever the value is read.</param>
        public ComputedColumn(string columnName, string dataType, string expression, bool persisted = false) : this(columnName, dataType, false, expression, persisted)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedColumn"/> class with a column name, data type, nullable and a non-persisted expression.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        /// <param name="expression">Expression representing the computed computed value of the column.</param>
        /// <param name="persisted">True to store the computed value in the table, False to re-compute the value whenever the value is read.</param>
        public ComputedColumn(string columnName, string dataType, bool nullable, string expression, bool persisted = false) : base(columnName, dataType, nullable)
        {
            Expression = expression;
            Persisted = persisted;
        }

        internal ComputedColumn(string columnName, bool nullable, string expression, bool persisted) : base(columnName, nullable, ColumnModifier.None)
        {
            Expression = expression;
            Persisted = persisted;
        }

        /// <summary>
        /// Gets the column modifier.
        /// </summary>
        public string Expression { get; }

        public bool Persisted { get; }

        internal override string GetExpression()
        {
            return Expression;
        }

        internal override bool GetPersisted()
        {
            return Persisted;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Represents a new computed column in the database.
    /// </summary>
    public class ComputedColumn<TType> : ComputedColumn
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedColumn{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="expression">Expression representing the computed computed value of the column.</param>
        /// <param name="persisted">True to store the computed value in the table, False to re-compute the value whenever the value is read.</param>
        public ComputedColumn(string columnName, string expression, bool persisted = false) : base(columnName, System.Nullable.GetUnderlyingType(typeof(TType)) != null, expression, persisted)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedColumn{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="length">Length of data type.</param>
        /// <param name="expression">Expression representing the computed computed value of the column.</param>
        /// <param name="persisted">True to store the computed value in the table, False to re-compute the value whenever the value is read.</param>
        public ComputedColumn(string columnName, int length, string expression, bool persisted = false) : this(columnName, length, false, expression, persisted)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedColumn{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="length">Length of data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        /// <param name="expression">Expression representing the computed computed value of the column.</param>
        /// <param name="persisted">True to store the computed value in the table, False to re-compute the value whenever the value is read.</param>
        public ComputedColumn(string columnName, int length, bool nullable, string expression, bool persisted = false) : base(columnName, nullable, expression, persisted)
        {
            Validate.IsEqualOrGreaterThan(length, 1, nameof(length));

            Length = length;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedColumn{TType}"/> class with a column name, data type and with nullable.
        /// </summary>
        /// <param name="columnName">Name of the new column.</param>
        /// <param name="scale">Scale of data type.</param>
        /// <param name="precision">Precision of data type.</param>
        /// <param name="expression">Expression representing the computed computed value of the column.</param>
        /// <param name="persisted">True to store the computed value in the table, False to re-compute the value whenever the value is read.</param>
        public ComputedColumn(string columnName, int scale, int precision, string expression, bool persisted = false) : base(columnName, System.Nullable.GetUnderlyingType(typeof(TType)) != null, expression, persisted)
        {
            Validate.IsEqualOrGreaterThan(scale, 1, nameof(scale));
            Validate.IsEqualOrGreaterThan(precision, 0, nameof(precision));

            Scale = scale;
            Precision = precision;
        }

        /// <summary>
        /// Gets length of data type.
        /// </summary>
        public int? Length { get; }

        /// <summary>
        /// Gets scale of data type.
        /// </summary>
        public int? Scale { get; }

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
