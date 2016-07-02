using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class TableInfoMySqlTest : TableInfoTest
    {
        public TableInfoMySqlTest() : base(new MySqlDatabase("MySql"))
        {
        }
    }
}
