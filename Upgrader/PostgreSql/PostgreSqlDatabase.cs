using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using Upgrader.Infrastructure;
using Upgrader.Schema;

namespace Upgrader.PostgreSql
{
    public class PostgreSqlDatabase : Database
    {
        private static readonly Lazy<ConnectionFactory> ConnectionFactory = new Lazy<ConnectionFactory>(() => new ConnectionFactory("Npgsql.dll", "Npgsql.NpgsqlConnection"));
        private readonly string connectionStringOrName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDatabase"/> class.
        /// </summary>
        /// <param name="connectionStringOrName">Connection string or name of the connection string to use as defined in App/Web.config.</param>
        public PostgreSqlDatabase(string connectionStringOrName) : base(ConnectionFactory.Value.CreateConnection(GetConnectionString(connectionStringOrName)), GetMasterConnectionString(connectionStringOrName, "Database", "postgres"))
        {
            this.connectionStringOrName = connectionStringOrName;

            // No type mapping for byte.
            TypeMappings.Add<bool>("boolean");
            TypeMappings.Add<char>("character(1)");
            TypeMappings.Add<DateTime>("timestamp without time zone");
            TypeMappings.Add<decimal>("numeric(19,5)");
            TypeMappings.Add<double>("double precision");
            TypeMappings.Add<float>("real");
            TypeMappings.Add<Guid>("uuid");
            TypeMappings.Add<int>("integer");
            TypeMappings.Add<long>("bigint");
            TypeMappings.Add<short>("smallint");
            TypeMappings.Add<string>("character varying(50)");
            TypeMappings.Add<TimeSpan>("interval(6)");
        }

        public override bool Exists
        {
            get
            {
                UseMainDatabase();
                var exists = Dapper.ExecuteScalar<bool>("SELECT COUNT(*) FROM pg_database WHERE datistemplate = false AND datname = @databaseName", new { databaseName = DatabaseName });
                UseConnectedDatabase();
                return exists;
            }
        }

        internal override string AutoIncrementStatement => "";

        internal override int MaxIdentifierLength => 63;

        internal override string GetColumnDataType(string tableName, string columnName)
        {
            return InformationSchema.GetColumnDataType(tableName, columnName, "character varying", "character", "numeric", "interval");
        }

        internal override void SupportsTransactionalDataDescriptionLanguage()
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = GetConnectionString(connectionStringOrName)
            };

            if (builder.ContainsKey("enlist") == false || string.Equals(builder["enlist"].ToString(), "true", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new NotSupportedException("Transactional upgrades requires \"enlist=true;\" in the connection string on PostgreSql.");
            }
        }

        internal override void ClearPool(IDbConnection connection)
        {
            var connectionType = connection.GetType();
            var methodInfo = connectionType.GetMethod("ClearPool", BindingFlags.Public | BindingFlags.Instance);
            methodInfo.Invoke(connection, new object[] { });
        }

        internal override void AddTable(string tableName, IEnumerable<Column> columns, IEnumerable<ForeignKey> foreignKeys)
        {
            var modifiedColumns = new List<Column>();
            foreach (var column in columns)
            {
                if (column.Modifier == ColumnModifier.AutoIncrementPrimaryKey)
                {
                    if (column.GetDataType(TypeMappings).Equals("integer", StringComparison.InvariantCultureIgnoreCase) || column.GetDataType(TypeMappings).Equals("int", StringComparison.CurrentCultureIgnoreCase))
                    {
                        modifiedColumns.Add(new Column(column.ColumnName, "serial", column.Nullable, ColumnModifier.PrimaryKey));
                    }
                    else if (column.GetDataType(TypeMappings).Equals("bigint", StringComparison.InvariantCultureIgnoreCase))
                    {
                        modifiedColumns.Add(new Column(column.ColumnName, "bigserial", column.Nullable, ColumnModifier.PrimaryKey));
                    }
                    else
                    {
                        throw new NotSupportedException("Only columns of type \"int\" or \"bigint\" may be qualified for usage as auto incrementing primary key.");
                    }
                }
                else
                {
                    modifiedColumns.Add(column);
                }
            }

            base.AddTable(tableName, modifiedColumns, foreignKeys);
        }

        internal override string[] GetIndexNames(string tableName)
        {
            var schemaName = GetSchema(tableName);

            return Dapper.Query<string>(
                @"
                    SELECT 
                        indexname 
                    FROM pg_indexes WHERE 
                        schemaname = 'public' AND 
                        tablename = @tableName 
                    EXCEPT

                    SELECT 
                        constraint_name 
                    FROM information_schema.table_constraints
                    WHERE
				        constraint_type = 'PRIMARY KEY' AND
                        table_name = @tableName AND 
                        table_schema = @schemaName
                ", 
                new { tableName, schemaName })
                .ToArray();
        }

        internal override bool GetIndexType(string tableName, string indexName)
        {
            return Dapper.ExecuteScalar<bool>(
                @"
                    SELECT
                          ix.indisunique
                    FROM 
                         pg_class ic,
                         pg_index ix     
                    WHERE
                         ic.relname = @indexName AND
                         ic.oid = ix.indexrelid
                ", 
                new { indexName });
        }



        internal override string[] GetIndexColumnNames(string tableName, string indexName)
        {
            return Dapper.Query<string>(
                @"
                    SELECT
                          a.attname
                    FROM 
                         pg_class ic,
                         pg_attribute a,
                         pg_class tc,
                         pg_index ix     
                    WHERE
                         ic.relname = @indexName AND
                         ic.oid = ix.indexrelid AND
                         tc.oid = ix.indrelid AND
                         a.attrelid = tc.oid AND
                         a.attnum = ANY(ix.indkey) AND
                         tc.relkind = 'r'
                ", 
                new { indexName })
                .ToArray();
        }

        internal override void RemoveIndex(string tableName, string indexName)
        {
            var escapedIndexName = EscapeIdentifier(indexName);

            Dapper.Execute($"DROP INDEX {escapedIndexName}");
        }

        internal override string EscapeIdentifier(string identifier)
        {
            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
        }

        internal override string GetSchema(string tableName)
        {
            return "public";
        }

        internal override void RenameColumn(string tableName, string columnName, string newColumnName)
        {
            var isColumnAutoIncrement = GetColumnAutoIncrement(tableName, columnName);
            if (isColumnAutoIncrement)
            {
                throw new NotSupportedException("Renaming a serial column is not supported in PostgreSql.");
            }

            var escapedTableName = EscapeIdentifier(tableName);
            var escapedColumnName = EscapeIdentifier(columnName);
            var escapedNewColumnName = EscapeIdentifier(newColumnName);

            Dapper.Execute($"ALTER TABLE {escapedTableName} RENAME {escapedColumnName} TO {escapedNewColumnName}");
        }

        internal override void ChangeColumn(string tableName, string columnName, string dataType, bool nullable)
        {
            var escapedTableName = EscapeIdentifier(tableName);
            var escapedColumnName = EscapeIdentifier(columnName);
            var nullableStatement = nullable ? "DROP NOT NULL" : "SET NOT NULL";

            Dapper.Execute($"ALTER TABLE {escapedTableName} ALTER COLUMN {escapedColumnName} TYPE  {dataType}, ALTER COLUMN {escapedColumnName} {nullableStatement}");
        }

        internal override bool GetColumnAutoIncrement(string tableName, string columnName)
        {
            const string Suffix = "_seq";
            var maxLength = MaxIdentifierLength - Suffix.Length;
            var sequenceName = $"{StringHelper.Truncate($"{tableName}_{columnName}", maxLength)}{Suffix}";

            return Dapper.ExecuteScalar<int>("SELECT COUNT(*) FROM pg_class WHERE relkind = 'S' AND relname = @sequenceName", new { sequenceName }) == 1;
        }

        internal override string GetLastInsertedAutoIncrementedPrimaryKeyIdentity(string columnName)
        {
            var escapedColumnName = EscapeIdentifier(columnName);
            return $"RETURNING {escapedColumnName}";
        }

        internal override Database Clone()
        {
            return new PostgreSqlDatabase(connectionStringOrName);
        }
    }
}
