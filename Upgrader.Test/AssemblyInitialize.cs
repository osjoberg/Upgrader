using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Upgrader.Test
{
    [TestClass]
    public static class AssemblyInitialize
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            try
            {
                Cleanup();
            }
            catch (DbException)
            {
            }

            using (var connection = new MySqlConnection("Server=localhost;Uid=root;Pwd=;"))
            {
                connection.Open();
                connection.Execute("CREATE DATABASE UpgraderTest");
            }

            using (var connection = new SqlConnection("Server=(local);Integrated Security=true;"))
            {
                connection.Open();
                connection.Execute("CREATE DATABASE UpgraderTest");
            }

            SQLiteConnection.CreateFile("UpgraderTest.sqlite");

            using (var connection = new NpgsqlConnection("User ID=postgres;Password=postgres;Host=localhost;Port=5432;"))
            {
                connection.Open();
                connection.Execute("CREATE DATABASE \"UpgraderTest\"");
            }
        }

        /*[AssemblyCleanup]*/
        public static void Cleanup()
        {
            using (var connection = new MySqlConnection("Server=localhost;Uid=root;Pwd=;"))
            {
                connection.Open();
                connection.Execute("USE information_schema");
                connection.Execute("DROP DATABASE UpgraderTest");
            }

            using (var connection = new SqlConnection("Server=(local);Integrated Security=true;"))
            {
                connection.Open();
                connection.Execute("USE master");
                connection.Execute("DROP DATABASE UpgraderTest");
            }

            if (File.Exists("UpgraderTest.sqlite") == false)
            {
                File.Delete("UpgraderTest.sqlite");
            }

            using (var connection = new NpgsqlConnection("User ID=postgres;Password=postgres;Host=localhost;Port=5432"))
            {
                connection.Open();
                connection.Execute("DROP DATABASE \"UpgraderTest\"");
            }
        }
    }
}
