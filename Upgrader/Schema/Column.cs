using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    public class Column
    {
        public Column(string columnName, string dataType, ColumnModifier modifiers) : this(columnName, dataType, false, modifiers)
        {
        }

        public Column(string columnName, string dataType, bool nullable = false) : this(columnName, dataType, nullable, ColumnModifier.None)
        {
        }

        private Column(string columnName, string dataType, bool nullable, ColumnModifier columnModifier)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            ColumnName = columnName;
            DataType = dataType;
            Nullable = nullable;
            Modifier = columnModifier;
        }

        public string ColumnName { get; }

        public string DataType { get; }

        public bool Nullable { get; }

        public ColumnModifier Modifier { get; }
    }
}
