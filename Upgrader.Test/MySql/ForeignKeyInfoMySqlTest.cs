using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class ForeignKeyInfoMySqlTest : ForeignKeyInfoTest
    {
        public ForeignKeyInfoMySqlTest() : base(new MySqlDatabase("MySql"))
        {            
        }
    }
}
