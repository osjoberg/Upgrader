using System;
using System.Linq;

using Upgrader.Infrastructure;

namespace Upgrader.MySql
{
    public class MySqlDatabase : Database
    {
        private static readonly Lazy<ConnectionFactory> ConnectionFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory("MySql.Data.dll", "MySql.Data.MySqlClient.MySqlConnection"));
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        public MySqlDatabase(string connectionString) : base(ConnectionFactory.Value.CreateConnection(connectionString), GetMasterConnectionString(connectionString, "Database", "mysql"))
        {
            this.connectionString = connectionString;

            TypeMappings.Add<bool>("bit");
            TypeMappings.Add<byte>("tinyint unsigned");
            TypeMappings.Add<char>("char(1)");
            TypeMappings.Add<DateTime>("datetime");
            TypeMappings.Add<decimal>("decimal(19,5)");
            TypeMappings.Add<double>("double");
            TypeMappings.Add<float>("double");
            TypeMappings.Add<Guid>("char(36)");
            TypeMappings.Add<int>("int");
            TypeMappings.Add<long>("bigint");
            TypeMappings.Add<short>("smallint");
            TypeMappings.Add<string>("varchar(50)");
            TypeMappings.Add<TimeSpan>("time(3)");
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

        internal override string[] GeneratedTypes => new string[] { };

        internal override int MaxIdentifierLength => 64;

        internal override string AutoIncrementStatement => "AUTO_INCREMENT";

        internal override void SupportsTransactionalDataDescriptionLanguage()
        {
            throw new NotSupportedException("Transactional data definition language statements are not supported by MySql.");
        }

        internal override string GetColumnDataType(string tableName, string columnName)
        {
            var schemaName = GetSchema(tableName);

            var columnInformation = Dapper.Query<InformationSchema.Column>(
                @"
                    SELECT 
                        DATA_TYPE, 
                        CHARACTER_MAXIMUM_LENGTH, 
                        NUMERIC_PRECISION, 
                        NUMERIC_SCALE,
                        COLUMN_TYPE,
                        DATETIME_PRECISION
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE 
                        COLUMN_NAME = @columnName AND
                        TABLE_NAME = @tableName AND 
                        TABLE_SCHEMA = @schemaName
                ",
                new { tableName, schemaName, columnName }).SingleOrDefault();

            if (columnInformation == null)
            {
                return null;
            }

            var unsigned = columnInformation.column_type.EndsWith("unsigned") ? " unsigned" : "";
            var includePrecisionOnTypes = new[] { "varchar", "char", "integer", "time", "decimal" };
            if (includePrecisionOnTypes.Contains(columnInformation.data_type) == false)
            {
                return columnInformation.data_type + unsigned;
            }

            var parameters = new[] { columnInformation.datetime_precision, columnInformation.character_maximum_length, columnInformation.numeric_precision, columnInformation.numeric_scale };

            var usedParameters = parameters.Where(parameter => parameter.HasValue).Select(parameter => parameter.Value.ToString()).ToArray();

            return columnInformation.data_type + (usedParameters.Any() ? "(" + string.Join(",", usedParameters) + ")" : "") + unsigned;
        }

        internal override string GetCreateComputedStatement(string dataType, bool nullable, string expression, bool persisted)
        {
            var persistedExpression = persisted ? " STORED" : "";

            return $"{dataType} GENERATED ALWAYS AS ({expression}){persistedExpression}";
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

        internal override void AddIndex(string tableName, string[] columnNames, bool unique, string indexName, string[] includeColumnNames)
        {
            if (includeColumnNames != null)
            {
                throw new NotSupportedException("Including columns in an index is not supported by MySql.");
            }

            this.DataDefinitionLanguage.AddIndex(tableName, columnNames, unique, indexName, null);
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

            Dapper.Execute($"ALTER TABLE {escapedTableName} RENAME COLUMN {escapedColumnName} TO {escapedNewColumnName}");
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

        internal override Database Clone()
        {
            return new MySqlDatabase(connectionString);           
        }
    }
}
