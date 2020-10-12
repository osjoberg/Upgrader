using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MySql.Data.MySqlClient;

using Npgsql;

using Upgrader.MySql;
using Upgrader.PostgreSql;
using Upgrader.SqLite;
using Upgrader.SqlServer;

namespace Upgrader.Test
{
    [TestClass]
    public static class AssemblyInitialize
    {
        public static readonly string SqlServerConnectionString;
        public static readonly string MySqlConnectionString;
        public static readonly string PostgreSqlConnectionString;
        public static readonly string SqLiteConnectionString;
        public static readonly string PostgreSqlEnlist;

        static AssemblyInitialize()
        {
            var connectionStrings = ConfigurationManager.OpenExeConfiguration("Upgrader.Test.dll").ConnectionStrings.ConnectionStrings;

            SqlServerConnectionString = connectionStrings["SqlServer"].ConnectionString;
            MySqlConnectionString = connectionStrings["MySql"].ConnectionString;
            PostgreSqlConnectionString = connectionStrings["PostgreSql"].ConnectionString;
            SqLiteConnectionString = connectionStrings["SqLite"].ConnectionString;
            PostgreSqlEnlist = connectionStrings["PostgreSqlEnlist"].ConnectionString;
        }

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            // References to all connection types so that the assemblies will be copied to the test bin directory.
            var references = new IDbConnection[] { new MySqlConnection(), new NpgsqlConnection(), new SQLiteConnection(), new SqlConnection() };

            var databases = new Database[]
            {
                new SqlServerDatabase(SqlServerConnectionString),
                new MySqlDatabase(MySqlConnectionString),
                new PostgreSqlDatabase(PostgreSqlConnectionString),
                new SqLiteDatabase(SqLiteConnectionString)
            };

            foreach (var database in databases)
            {
                try
                {
                    if (database.Exists)
                    {
                        database.Remove();
                    }

                    database.Create();
                }
                finally
                {
                    database.Dispose();
                }
            }
        }
    }
}
