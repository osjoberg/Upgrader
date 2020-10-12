using System.Configuration;
using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MySql.Data.MySqlClient;

using Upgrader.MySql;
using Upgrader.SqlServer;

namespace Upgrader.ConnectionFactoryTest
{
   

    [TestClass]
    public class ConnectionFactoryTest
    {
        // Reference to a MySqlConnection type so that the the assembly will be copied to the test bin directory.
        private IDbConnection[] references = { new MySqlConnection(), new Microsoft.Data.SqlClient.SqlConnection(), new System.Data.SqlClient.SqlConnection() };
        private readonly ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.OpenExeConfiguration("Upgrader.ConnectionFactoryTest.dll").ConnectionStrings.ConnectionStrings;

        [TestMethod]
        public void CanLoad()
        {
            new MySqlDatabase(connectionStrings["MySql"].ConnectionString);
        }

        [TestMethod]
        public void MicrosoftDataSqlClientSqlConnectionIsPreferred()
        {
            Assert.AreEqual(typeof(Microsoft.Data.SqlClient.SqlConnection), new SqlServerDatabase(connectionStrings["SqlServer"].ConnectionString).Connection.GetType());
        }
    }
}
