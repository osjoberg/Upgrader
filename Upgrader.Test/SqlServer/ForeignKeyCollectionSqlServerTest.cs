using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class ForeignKeyCollectionSqlServerTest : ForeignKeyCollectionTest
    {
        public ForeignKeyCollectionSqlServerTest() : base(new SqlServerDatabase("SqlServer"))
        {            
        }
    }
}
