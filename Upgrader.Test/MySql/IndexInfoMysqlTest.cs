using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class IndexInfoMysqlTest : IndexInfoTest
    {
        public IndexInfoMysqlTest() : base(new MySqlDatabase("Server=localhost;Database=UpgraderTest;Uid=root;Pwd=;"))
        {            
        }
    }
}
