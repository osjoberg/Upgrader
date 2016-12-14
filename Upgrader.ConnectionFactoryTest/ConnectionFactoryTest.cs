using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;

namespace Upgrader.ConnectionFactoryTest
{
    [TestClass]
    public class ConnectionFactoryTest
    {
        [TestMethod]
        public void CanLoad()
        {
            new MySqlDatabase("MySql");
        }
    }
}
