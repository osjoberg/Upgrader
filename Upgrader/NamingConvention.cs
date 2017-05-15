namespace Upgrader
{
    public class NamingConvention
    {
        private readonly int maxIdentifierLength;

        /// <summary>
        /// Create a new instance of the naming convention.
        /// </summary>
        /// <param name="maxIdentifierLength">Maximum character length of identifiers in the underlying database engine.</param>
        public NamingConvention(int maxIdentifierLength)
        {
            this.maxIdentifierLength = maxIdentifierLength;
        }

        /// <summary>
        /// Generates a foreign key name based on naming convention. 
        /// </summary>
        /// <param name="tableName">Table name of parent side of the foreign-key relationship.</param>
        /// <param name="columnNames">Column names of the parent side of the foreign-key relationship.</param>
        /// <param name="foreignTableName">Table name of the child side of the foreign-key relationship.</param>
        /// <returns>Returns a foreign key name with format FK_tableName_column1_columnN_foreignTableName.</returns>
        public virtual string GetForeignKeyNamingConvention(string tableName, string[] columnNames, string foreignTableName)
        {
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return StringHelper.Truncate($"FK_{tableName}_{underscoreSepratedColumnNames}_{foreignTableName}", maxIdentifierLength);
        }

        /// <summary>
        /// Generates a primary key name based on naming convention.
        /// </summary>
        /// <param name="tableName">Table name defining the primary key.</param>
        /// <param name="columnNames">Columns defining the primary key.</param>
        /// <returns>Returns a primary key name with format PK_tableName_column1_columnN.</returns>
        public virtual string GetPrimaryKeyNamingConvention(string tableName, string[] columnNames)
        {
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return StringHelper.Truncate($"PK_{tableName}_{underscoreSepratedColumnNames}", maxIdentifierLength);
        }

        /// <summary>
        /// Generates a index name based on naming convention.
        /// </summary>
        /// <param name="tableName">Table name defining the index.</param>
        /// <param name="columnNames">Column names defining the index.</param>
        /// <param name="unique">True if the defining index is unique.</param>
        /// <returns>
        /// Returns a index name with format IX_tableName_column1_columnN for non-unique indexes. 
        /// If the index is unique the format is UX_tableName_column1_columnN.
        /// </returns>
        public virtual string GetIndexNamingConvention(string tableName, string[] columnNames, bool unique)
        {
            var prefix = unique ? "UX" : "IX";
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return StringHelper.Truncate($"{prefix}_{tableName}_{underscoreSepratedColumnNames}", maxIdentifierLength);
        }
    }
}