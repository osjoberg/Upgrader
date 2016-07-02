using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class ColumnInfoMySqlTest : ColumnInfoTest
    {
        public ColumnInfoMySqlTest() : base(new MySqlDatabase("MySql"))
        {
        }
    }
}
