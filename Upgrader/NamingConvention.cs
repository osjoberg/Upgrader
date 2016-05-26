namespace Upgrader
{
    public class NamingConvention
    {
        protected internal string GetForeignKeyNamingConvention(string tableName, string[] columnNames, string foreignTableName)
        {
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return $"FK_{tableName}_{underscoreSepratedColumnNames}_{foreignTableName}";
        }

        protected internal string GetPrimaryKeyNamingConvention(string tableName, string[] columnNames)
        {
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return $"PK_{tableName}_{underscoreSepratedColumnNames}";
        }

        protected internal string GetIndexNamingConvention(string tableName, string[] columnNames, bool unique)
        {
            var prefix = unique ? "UX" : "IX";
            var underscoreSepratedColumnNames = string.Join("_", columnNames);
            return $"{prefix}_{tableName}_{underscoreSepratedColumnNames}";
        }
    }
}
