using System.Collections.Generic;

namespace Upgrader.Infrastructure
{
    internal class StructuredQueryLanguage
    {
        private readonly Database database;

        public StructuredQueryLanguage(Database database)
        {
            this.database = database;
        }

        public IEnumerable<dynamic> Select(string tableName, string where)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var whereClause = string.IsNullOrEmpty(where) ? "" : $" WHERE {where}";

            return database.Dapper.Query($"SELECT * FROM {escapedTableName}{whereClause}");
        }

        public IEnumerable<T> Select<T>(string tableName, string where)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var whereClause = string.IsNullOrEmpty(where) ? "" : $" WHERE {where}";

            return database.Dapper.Query<T>($"SELECT * FROM {escapedTableName}{whereClause}");
        }
    }
}
