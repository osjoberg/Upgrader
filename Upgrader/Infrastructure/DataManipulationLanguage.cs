using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Upgrader.Schema;

namespace Upgrader.Infrastructure
{
    internal class DataManipulationLanguage
    {
        private readonly Database database;

        public DataManipulationLanguage(Database database)
        {
            this.database = database;
        }

        internal void Insert<T>(string tableName, IEnumerable<T> rows)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);

            var columns = database.Tables[tableName].Columns.ToArray();

            var autoIncrementedColumn = columns.SingleOrDefault(column => column.IsAutoIncrement());

            var escapedColumnNames = columns.Where(column => column != autoIncrementedColumn).Select(column => database.EscapeIdentifier(column.ColumnName));
            var escapedCommaSeparatedColumnNames = string.Join(", ", escapedColumnNames);
            var parameterNames = columns.Where(column => column != autoIncrementedColumn).Select(column => "@" + column.ColumnName);
            var commaSeparatedParameterNames = string.Join(", ", parameterNames);

            var sql = $"INSERT INTO {escapedTableName} ({escapedCommaSeparatedColumnNames}) VALUES ({commaSeparatedParameterNames})";

            var writableAutoIncrementProperty = GetWritableAutoIncrementProperty(typeof(T), autoIncrementedColumn);
            if (writableAutoIncrementProperty != null)
            {
                sql += database.GetLastInsertedAutoIncrementedPrimaryKeyIdentity(writableAutoIncrementProperty.Name);
            }

            foreach (var row in rows)
            {
                if (writableAutoIncrementProperty != null)
                {
                    var newIdentity = database.Dapper.ExecuteScalar(sql, row);
                    var newIdentityTypeChanged = Convert.ChangeType(newIdentity, writableAutoIncrementProperty.PropertyType);
                    writableAutoIncrementProperty.SetValue(row, newIdentityTypeChanged, null);
                }
                else
                {
                    database.Dapper.Execute(sql, row);
                }
            }
        }

        internal void SetColumnValue(string tableName, string columnName, object value)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var escapedColumnName = database.EscapeIdentifier(columnName);

            database.Dapper.Execute($"UPDATE {escapedTableName} SET {escapedColumnName} = @value", new { value });
        }

        internal void Update<T>(string tableName, IEnumerable<T> rows)
        {
            var primaryKeyColumnNames = database.Tables[tableName].GetPrimaryKey()?.GetColumnNames();           
            if (primaryKeyColumnNames == null)
            {
                throw new ArgumentException("");                
            }

            var escapedTableName = database.EscapeIdentifier(tableName);

            var columns = database.Tables[tableName].Columns;

            var columnNames = columns
                .Where(column => column.IsAutoIncrement() == false)
                .Select(column => column.ColumnName)
                .Except(primaryKeyColumnNames);

            var setStatements = columnNames.Select(columnName => $"{database.EscapeIdentifier(columnName)} = @{columnName}");
            var setStatement = string.Join(", ", setStatements);

            var whereStatements = primaryKeyColumnNames.Select(columnName => $"{database.EscapeIdentifier(columnName)} = @{columnName}");
            var whereStatement = string.Join(", ", whereStatements);

            var sql = $"UPDATE {escapedTableName} SET {setStatement} WHERE {whereStatement}";

            foreach (var row in rows)
            {
                database.Dapper.Execute(sql, row);
            }
        }

        internal void Delete<T>(string tableName, IEnumerable<T> rows)
        {
            var primaryKeyColumnNames = database.Tables[tableName].GetPrimaryKey().GetColumnNames();
            if (primaryKeyColumnNames == null)
            {
                throw new ArgumentException("");
            }

            var escapedTableName = database.EscapeIdentifier(tableName);
            var whereStatements = primaryKeyColumnNames.Select(columnName => $"{database.EscapeIdentifier(columnName)} = @{columnName}");
            var whereStatement = string.Join(", ", whereStatements);

            var sql = $"DELETE FROM {escapedTableName} WHERE {whereStatement}";

            foreach (var row in rows)
            {
                database.Dapper.Execute(sql, row);
            }
        }

        internal void Delete(string tableName, string where)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var whereStatement = string.IsNullOrEmpty(where) ? "" : $" WHERE {where}";

            var sql = $"DELETE FROM {escapedTableName} {whereStatement}";

            database.Dapper.Execute(sql);
        }

        internal void Update<T>(string tableName, Func<T, T> updateFunc)
        {
            var escapedTableName = database.EscapeIdentifier(tableName);
            var sql = $"SELECT * FROM {escapedTableName}";
            var rows = database.Dapper.Query<T>(sql);

            Update(tableName, rows.Select(updateFunc));
        }

        private static PropertyInfo GetWritableAutoIncrementProperty(Type type, ColumnInfo autoIncrementColumn)
        {
            if (autoIncrementColumn == null)
            {
                return null;
            }
            
            var propertyInfo = type.GetProperty(autoIncrementColumn.ColumnName);
            if (propertyInfo == null || propertyInfo.CanWrite == false)
            {
                return null;
            }

            return propertyInfo;
        }
    }
}
