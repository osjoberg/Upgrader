using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class ColumnInfoSqlServerTest : ColumnInfoTest
    {
        public ColumnInfoSqlServerTest() : base(new SqlServerDatabase("SqlServer"))
        {            
        }
    }
}
