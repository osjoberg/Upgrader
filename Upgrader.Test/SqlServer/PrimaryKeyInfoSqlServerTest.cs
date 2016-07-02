using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class PrimaryKeyInfoSqlServerTest : PrimaryKeyInfoTest
    {
        public PrimaryKeyInfoSqlServerTest() : base(new SqlServerDatabase("SqlServer"))
        {
        }
    }
}
