using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;
using Upgrader.Schema;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class PrimaryKeyInfoMysqlTest : PrimaryKeyInfoTest
    {
        public PrimaryKeyInfoMysqlTest() : base(new MySqlDatabase("Server=localhost;Database=UpgraderTest;Uid=root;Pwd=;"))
        {
        }

        [TestMethod]
        public override void PrimaryKeyIsNamedAccordingToNamingConvention()
        {
            Database.Tables.Add("PrimaryKeyName", new Column("PrimaryKeyNameId", "int"));
            Database.Tables["PrimaryKeyName"].AddPrimaryKey("PrimaryKeyNameId");

            Assert.AreEqual("PRIMARY", Database.Tables["PrimaryKeyName"].PrimaryKey.PrimaryKeyName);
        }
    }
}
