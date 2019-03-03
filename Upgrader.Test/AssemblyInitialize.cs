using System.Data;
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
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            // References to all connection types so that the assemblies will be copied to the test bin directory.
            var references = new IDbConnection[] { new MySqlConnection(), new NpgsqlConnection(), new SQLiteConnection() };

            var databases = new Database[]
            {
                new SqlServerDatabase("SqlServer"),
                new MySqlDatabase("MySql"), 
                new PostgreSqlDatabase("PostgreSql"),
                new SqLiteDatabase("SqLite") 
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
