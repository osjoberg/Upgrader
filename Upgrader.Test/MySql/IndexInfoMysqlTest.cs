using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class IndexInfoMySqlTest : IndexInfoTest
    {
        public IndexInfoMySqlTest() : base(new MySqlDatabase("Server=localhost;Database=UpgraderTest;Uid=root;Pwd=;"))
        {            
        }
    }
}
