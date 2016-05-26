using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

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
        }
    }
}
