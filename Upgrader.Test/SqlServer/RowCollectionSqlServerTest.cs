using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class RowCollectionSqlServerTest : RowCollectionTest
    {
        public RowCollectionSqlServerTest() : base(new SqlServerDatabase(AssemblyInitialize.SqlServerConnectionString))
        {
        }
    }
}
