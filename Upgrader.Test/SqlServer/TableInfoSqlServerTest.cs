using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class TableInfoSqlServerTest : TableInfoTest
    {
        public TableInfoSqlServerTest() : base(new SqlServerDatabase(AssemblyInitialize.SqlServerConnectionString))
        {
        }
    }
}
