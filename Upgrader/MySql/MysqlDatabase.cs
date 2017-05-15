using System;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.MySql
{
    public class MySqlDatabase : Database
    {
        private static readonly Lazy<ConnectionFactory> ConnectionFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory("MySql.Data.dll", "MySql.Data.MySqlClient.MySqlConnection"));

        /// <summary>
        /// Creates an instance of the MySqlDatabase.
        /// </summary>
        /// <param name="connectionStringOrName">Connection string or name of the connection string to use as defined in App/Web.config.</param>
        public MySqlDatabase(string connectionStringOrName) : base(ConnectionFactory.Value.CreateConnection(GetConnectionString(connectionStringOrName)), GetMasterConnectionString(connectionStringOrName, "Database", "mysql"))
        {
        }

        public override bool Exists
        {
            get
            {
                UseMainDatabase();
                var exists = Dapper.ExecuteScalar<bool>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @databaseName", new { databaseName = DatabaseName });
                UseConnectedDatabase();
                return exists;
            }
        }

        internal override int MaxIdentifierLength => 64;

        internal override string AutoIncrementStatement => "AUTO_INCREMENT";

        internal override void ChangeColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedColumnName = EscapeIdentifier(columnName);
            var nullableStatement = nullable ? "NULL" : "NOT NULL";

            Dapper.Execute($"ALTER TABLE {escapedTableName} CHANGE COLUMN {escapedColumnName} {escapedColumnName} {dataType} {nullableStatement}");
        }

        internal override void AddPrimaryKey(string tableName, string[] columnNames, string constraintName)
        {
            var isSupportedConstraintName = constraintName == "PRIMARY" || constraintName == NamingConvention.GetPrimaryKeyNamingConvention(tableName, columnNames);

            Validate.IsTrue(isSupportedConstraintName, nameof(constraintName), "MySql only supports primary key constraints named \"PRIMARY\".");

            base.AddPrimaryKey(tableName, columnNames, constraintName);
        }

        internal override void RemovePrimaryKey(string tableName, string primaryKeyName)
        {
            var escapedTableName = EscapeIdentifier(tableName);

            Dapper.Execute($"ALTER TABLE {escapedTableName} DROP PRIMARY KEY");
        }

        internal override string GetForeignKeyForeignTableName(string tableName, string constraintName)
        {
            var schemaName = GetSchema(tableName);

            return Dapper.ExecuteScalar<string>(
                @"
                SELECT  
                    KCU2.TABLE_NAME
                FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1 ON                				
					KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME AND
					KCU1.TABLE_NAME = RC.TABLE_NAME AND
					KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG AND
					KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA                
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 ON 
                    KCU2.CONSTRAINT_NAME = UNIQUE_CONSTRAINT_NAME AND
                    KCU2.TABLE_NAME = RC.REFERENCED_TABLE_NAME AND
					KCU2.CONSTRAINT_CATALOG = UNIQUE_CONSTRAINT_CATALOG AND
                    KCU2.CONSTRAINT_SCHEMA = UNIQUE_CONSTRAINT_SCHEMA
				WHERE 
                    RC.CONSTRAINT_NAME = @constraintName AND 
                    RC.CONSTRAINT_SCHEMA = @schemaName AND 
                    KCU1.TABLE_NAME = @tableName
				ORDER BY 
                    KCU2.ORDINAL_POSITION
                ", 
                new { constraintName, tableName, schemaName });
        }

        internal override string[] GetForeignKeyForeignColumnNames(string tableName, string constraintName)
        {
            var schemaName = GetSchema(tableName);

            return Dapper.Query<string>(
                @"
                SELECT  
                    KCU2.COLUMN_NAME
                FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1 ON                				
					KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME AND
					KCU1.TABLE_NAME = RC.TABLE_NAME AND
					KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG AND
					KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA                
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 ON 
                    KCU2.CONSTRAINT_NAME = UNIQUE_CONSTRAINT_NAME AND
                    KCU2.TABLE_NAME = RC.REFERENCED_TABLE_NAME AND
					KCU2.CONSTRAINT_CATALOG = UNIQUE_CONSTRAINT_CATALOG AND
                    KCU2.CONSTRAINT_SCHEMA = UNIQUE_CONSTRAINT_SCHEMA
				WHERE 
                    RC.CONSTRAINT_NAME = @constraintName AND 
                    RC.CONSTRAINT_SCHEMA = @schemaName AND 
                    KCU1.TABLE_NAME = @tableName
				ORDER BY 
                    KCU2.ORDINAL_POSITION
                ", 
                new { constraintName, tableName, schemaName }).ToArray();
        }

        internal override void RemoveForeignKey(string tableName, string foreignKeyName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedForeignKeyName = EscapeIdentifier(foreignKeyName);

            Dapper.Execute($"ALTER TABLE {escapedTableName} DROP FOREIGN KEY {escapedForeignKeyName}");
        }

        internal override string[] GetIndexNames(string tableName)
        {
            var schemaName = GetSchema(tableName);

            return Dapper.Query<string>(
                @"
                SELECT DISTINCT
                    INDEX_NAME
                FROM INFORMATION_SCHEMA.STATISTICS 
                WHERE
                    TABLE_NAME = @tableName AND
                    TABLE_SCHEMA = @schemaName AND
                    INDEX_NAME <> 'PRIMARY'
                ", 
                new { tableName, schemaName })
                .Except(GetForeignKeyNames(tableName))
                .ToArray();
        }

        internal override bool GetIndexType(string tableName, string indexName)
        {
            var schemaName = GetSchema(tableName);

            return Dapper.ExecuteScalar<bool>(
                @"
                SELECT DISTINCT
                    1 - NON_UNIQUE AS IS_UNIQUE
                FROM INFORMATION_SCHEMA.STATISTICS WHERE
                    INDEX_NAME = @indexName AND
                    TABLE_NAME = @tableName AND
                    TABLE_SCHEMA = @schemaName
                ", 
                new { indexName, tableName, schemaName });
        }

        internal override string[] GetIndexColumnNames(string tableName, string indexName)
        {
            var schemaName = GetSchema(tableName);

            return Dapper.Query<string>(
                @"
                SELECT 
	                COLUMN_NAME
                FROM INFORMATION_SCHEMA.STATISTICS WHERE 
	                INDEX_NAME = @indexName AND
	                TABLE_NAME = @tableName AND
	                TABLE_SCHEMA = @schemaName
                ", 
                new { indexName, tableName, schemaName }).ToArray();
        }

        internal override void RemoveIndex(string tableName, string indexName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedIndexName = EscapeIdentifier(indexName);

            Dapper.Execute($"ALTER TABLE {escapedTableName} DROP INDEX {escapedIndexName}");
        }

        internal override string GetSchema(string tableName)
        {
            return Connection.Database;
        }

        internal override string GetCatalog()
        {
            return "def";
        }

        internal override void RenameColumn(string tableName, string columnName, string newColumnName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedColumnName = EscapeIdentifier(columnName);
            var escapedNewColumnName = EscapeIdentifier(newColumnName);

            var dataType = GetColumnDataType(tableName, columnName);
            var nullable = GetColumnNullable(tableName, columnName);
            var nullableStatement = nullable ? "NULL" : "NOT NULL";
            var autoIncrementStatement = GetColumnAutoIncrement(tableName, columnName) ? "AUTO_INCREMENT" : "";

            Dapper.Execute($"ALTER TABLE {escapedTableName} CHANGE COLUMN {escapedColumnName} {escapedNewColumnName} {dataType} {nullableStatement} {autoIncrementStatement}");
        }

        internal override void RenameTable(string tableName, string newTableName)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedNewTableName = EscapeIdentifier(newTableName);

            Dapper.Execute($"RENAME TABLE {escapedTableName} TO {escapedNewTableName}");
        }

        internal override bool GetColumnAutoIncrement(string tableName, string columnName)
        {
            return Dapper.ExecuteScalar<bool>(
                @"
                SELECT 
                    EXTRA LIKE '%auto_increment%'
                FROM INFORMATION_SCHEMA.COLUMNS WHERE
                    TABLE_NAME = @tableName AND 
                    COLUMN_NAME = @columnName
                ", 
                new { tableName, columnName });
        }

        internal override string EscapeIdentifier(string identifier)
        {
            return "`" + identifier.Replace("`", "``") + "`";
        }

        internal override string GetLastInsertedAutoIncrementedPrimaryKeyIdentity(string columnName)
        {
            return ";SELECT LAST_INSERT_ID()";
        }
    }
}
