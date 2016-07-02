using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class IndexCollectionSqlServerTest : IndexCollectionTest
    {
        public IndexCollectionSqlServerTest() : base(new SqlServerDatabase("SqlServer"))
        {            
        }
    }
}
