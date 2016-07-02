namespace Upgrader
{
    public class NamingConvention
    {
        private readonly int maxIdentifierLength;

        public NamingConvention(int maxIdentifierLength)
        {
            this.maxIdentifierLength = maxIdentifierLength;
        }

        internal string GetForeignKeyNamingConvention(string tableName, string[] columnNames, string foreignTableName)
        {
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return Truncate($"FK_{tableName}_{underscoreSepratedColumnNames}_{foreignTableName}");
        }

        internal string GetPrimaryKeyNamingConvention(string tableName, string[] columnNames)
        {
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return Truncate($"PK_{tableName}_{underscoreSepratedColumnNames}");
        }

        internal string GetIndexNamingConvention(string tableName, string[] columnNames, bool unique)
        {
            var prefix = unique ? "UX" : "IX";
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return Truncate($"{prefix}_{tableName}_{underscoreSepratedColumnNames}");
        }

        private string Truncate(string identifier)
        {
            return identifier.Length >= maxIdentifierLength ? identifier.Substring(0, maxIdentifierLength) : identifier;
        }
    }
}