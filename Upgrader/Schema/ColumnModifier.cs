namespace Upgrader.Schema
{
    /// <summary>
    /// Column modifiers.
    /// </summary>
    public enum ColumnModifier
    {
        /// <summary>
        /// No column modifier.
        /// </summary>
        None = 0, 

        /// <summary>
        /// Column is primary key.
        /// </summary>
        PrimaryKey = 1, 

        /// <summary>
        /// Column is an auto incrementing primary key.
        /// </summary>
        AutoIncrementPrimaryKey = 2,

        /// <summary>
        /// Collum is defined as nullable.
        /// </summary>
        Nullable = 3
    }
}
