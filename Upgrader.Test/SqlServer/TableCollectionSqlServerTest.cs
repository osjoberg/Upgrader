using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class TableCollectionSqlServerTest : TableCollectionTest
    {
        public TableCollectionSqlServerTest() : base(new SqlServerDatabase("SqlServer"))
        {
        }
    }
}
