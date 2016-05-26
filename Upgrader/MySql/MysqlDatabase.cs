using System.Linq;
using Dapper;
using Upgrader.Infrastructure;

namespace Upgrader.MySql
{
    public class MySqlDatabase : Database
    {
        private static readonly ConnectionFactory connectionFactory = new ConnectionFactory("MySql.Data.dll", "MySql.Data.MySqlClient.MySqlConnection");

        public MySqlDatabase(string connectionString) : base(connectionFactory.CreateConnection(connectionString))
        {
        }
       
        public override sealed void Dispose()
        {
            Connection.Execute("USE information_schema");
            Connection.Dispose();
        }

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

            Validate.IsTrue(isSupportedConstraintName, nameof(constraintName), "MySql only spports primary key constraints named \"PRIMARY\".");

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
                new { tableName, schemaName }).ToArray();
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

        protected internal override string GetSchema(string tableName)
        {
            return Connection.Database;
        }

        protected internal override string EscapeIdentifier(string identifier)
        {
            return "`" + identifier.Replace("`", "``") + "`";
        }

        internal override string AutoIncrementStatement => "AUTO_INCREMENT";
    }
}
