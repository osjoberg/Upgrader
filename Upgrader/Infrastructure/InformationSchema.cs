using System.Linq;

namespace Upgrader.Infrastructure
{
    internal sealed class InformationSchema
    {
        private readonly Database database;

        public InformationSchema(Database database)
        {
            this.database = database;
        }

        internal string[] GetTableNames()
        {
            var schemaName = database.GetSchema(null);
            var tableCatalog = database.GetCatalog();

            return database.Dapper.Query<string>(
                @"
                    SELECT
                        TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE 
                        TABLE_SCHEMA = @schemaName AND
                        TABLE_CATALOG = @tableCatalog
                ", 
                new { schemaName, tableCatalog }).ToArray();
        }

        internal string[] GetColumnNames(string tableName)
        {
            var schemaName = database.GetSchema(tableName);

            return database.Dapper.Query<string>(
                @"
                    SELECT 
                        COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE 
                        TABLE_NAME = @tableName AND 
                        TABLE_SCHEMA = @schemaName
                    ORDER BY ORDINAL_POSITION
                ", 
                new { tableName, schemaName }).ToArray();
        }

        internal string GetColumnDataType(string tableName, string columnName, params string[] includePrecisionOnTypes)
        {
            var schemaName = database.GetSchema(tableName);

            var columnInformation = database.Dapper.Query<Column>(
                @"
                    SELECT 
                        DATA_TYPE, 
                        CHARACTER_MAXIMUM_LENGTH, 
                        NUMERIC_PRECISION, 
                        NUMERIC_SCALE,
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

            if (includePrecisionOnTypes.Contains(columnInformation.data_type) == false)
            {
                return columnInformation.data_type;
            }

            var parameters = new[] { columnInformation.datetime_precision, columnInformation.character_maximum_length, columnInformation.numeric_precision, columnInformation.numeric_scale };

            var usedParameters = parameters.Where(parameter => parameter.HasValue).Select(parameter => parameter.Value.ToString()).ToArray();
            
            return columnInformation.data_type + (usedParameters.Any() ? "(" + string.Join(",", usedParameters) + ")" : "");
        }

        internal bool GetColumnNullable(string tableName, string columnName)
        {
            var schemaName = database.GetSchema(tableName);

            return database.Dapper.Query<string>(
                @"
                    SELECT 
                        IS_NULLABLE
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE 
                        COLUMN_NAME = @columnName AND
                        TABLE_NAME = @tableName AND 
                        TABLE_SCHEMA = @schemaName
                ", 
                new { tableName, schemaName, columnName }).Single() == "YES";
        }

        internal string[] GetForeignKeyColumnNames(string tableName, string foreignKeyName)
        {
            return GetConstraintColumnNames(tableName, foreignKeyName);
        }

        internal string[] GetPrimaryKeyColumnNames(string tableName, string primaryKeyName)
        {
            return GetConstraintColumnNames(tableName, primaryKeyName);
        }

        internal string[] GetForeignKeyNames(string tableName)
        {
            return GetConstraintNames(tableName, "FOREIGN KEY");
        }

        internal string GetPrimaryKeyName(string tableName)
        {
            return GetConstraintNames(tableName, "PRIMARY KEY").SingleOrDefault();
        }

        internal string[] GetForeignKeyForeignColumnNames(string tableName, string foreignKeyName)
        {
            var schemaName = database.GetSchema(tableName);

            return database.Dapper.Query<string>(
                @"
	                SELECT  
                        KCU2.COLUMN_NAME
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
	                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1 ON
		                KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME AND
		                KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG AND
		                KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA
                    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 ON 
                        KCU2.CONSTRAINT_NAME = UNIQUE_CONSTRAINT_NAME AND
		                KCU2.CONSTRAINT_CATALOG = UNIQUE_CONSTRAINT_CATALOG AND
                        KCU2.CONSTRAINT_SCHEMA = UNIQUE_CONSTRAINT_SCHEMA
	                WHERE 
		                RC.CONSTRAINT_NAME = @foreignKeyName AND 
		                RC.CONSTRAINT_SCHEMA = @schemaName AND 
		                KCU1.TABLE_NAME = @tableName
	                ORDER BY 
		                KCU2.ORDINAL_POSITION
                ", 
                new { foreignKeyName, tableName, schemaName }).ToArray();
        }

        internal string GetForeignKeyForeignTableName(string tableName, string foreignKeyName)
        {
            var schemaName = database.GetSchema(tableName);

            return database.Dapper.ExecuteScalar<string>(
            @"
	            SELECT  
                    KCU2.TABLE_NAME
                FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
	            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1 ON
		            KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME AND
		            KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG AND
		            KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 ON 
                    KCU2.CONSTRAINT_NAME = UNIQUE_CONSTRAINT_NAME AND
		            KCU2.CONSTRAINT_CATALOG = UNIQUE_CONSTRAINT_CATALOG AND
                    KCU2.CONSTRAINT_SCHEMA = UNIQUE_CONSTRAINT_SCHEMA
	            WHERE 
		            RC.CONSTRAINT_NAME = @foreignKeyName AND 
		            RC.CONSTRAINT_SCHEMA = @schemaName AND 
		            KCU1.TABLE_NAME = @tableName
            ", 
            new { tableName, foreignKeyName, schemaName });
        }

        private string[] GetConstraintNames(string tableName, string constraintType)
        {
            var schemaName = database.GetSchema(tableName);

            return database.Dapper.Query<string>(
                @"
                    SELECT 
                  	    CONSTRAINT_NAME
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE
				        CONSTRAINT_TYPE = @constraintType AND
                        TABLE_NAME = @tableName AND 
                        TABLE_SCHEMA = @schemaName
                ", 
                new { tableName, schemaName, constraintType }).ToArray();
        }

        private string[] GetConstraintColumnNames(string tableName, string constraintName)
        {
            var schemaName = database.GetSchema(tableName);

            return database.Dapper.Query<string>(
                @"
                    SELECT 
                        COLUMN_NAME
                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                    WHERE
				        CONSTRAINT_NAME = @constraintName AND
                        TABLE_NAME = @tableName AND 
                        TABLE_SCHEMA = @schemaName
                ", 
                new { tableName, schemaName, constraintName }).ToArray();
        }

        internal class Column
        {
            public string data_type { get; set; }

            public int? character_maximum_length { get; set; }

            public int? numeric_precision { get; set; }

            public int? numeric_scale { get; set; }

            public string column_type { get; set; }

            public int? datetime_precision { get; set; }
        }
    }
}