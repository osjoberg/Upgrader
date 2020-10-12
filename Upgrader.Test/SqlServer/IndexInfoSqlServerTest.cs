using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class IndexInfoSqlServerTest : IndexInfoTest
    {
        public IndexInfoSqlServerTest() : base(new SqlServerDatabase(AssemblyInitialize.SqlServerConnectionString))
        {            
        }
    }
}
