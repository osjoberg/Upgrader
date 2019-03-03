using Microsoft.VisualStudio.TestTools.UnitTesting;

using MySql.Data.MySqlClient;

using Upgrader.MySql;

namespace Upgrader.ConnectionFactoryTest
{
    [TestClass]
    public class ConnectionFactoryTest
    {
        // Reference to a MySqlConnection type so that the the assembly will be copied to the test bin directory.
        private MySqlConnection reference = new MySqlConnection();

        [TestMethod]
        public void CanLoad()
        {
            new MySqlDatabase("MySql");
        }
    }
}
