using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class RowCollectionMySqlTest : RowCollectionTest
    {
        public RowCollectionMySqlTest() : base(new MySqlDatabase(AssemblyInitialize.MySqlConnectionString))
        {
        }
    }
}
