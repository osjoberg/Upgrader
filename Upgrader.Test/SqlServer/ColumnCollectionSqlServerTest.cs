using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class ColumnCollectionSqlServerTest : ColumnCollectionTest
    {
        public ColumnCollectionSqlServerTest() : base(new SqlServerDatabase("SqlServer"))
        {            
        }
    }
}
