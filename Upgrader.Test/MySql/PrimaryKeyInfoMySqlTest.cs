using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;
using Upgrader.Schema;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class PrimaryKeyInfoMySqlTest : PrimaryKeyInfoTest
    {
        public PrimaryKeyInfoMySqlTest() : base(new MySqlDatabase("MySql"))
        {
        }

        [TestMethod]
        public override void PrimaryKeyIsNamedAccordingToNamingConvention()
        {
            Database.Tables.Add("PrimaryKeyName", new Column<int>("PrimaryKeyNameId"));
            Database.Tables["PrimaryKeyName"].AddPrimaryKey("PrimaryKeyNameId");

            Assert.AreEqual("PRIMARY", Database.Tables["PrimaryKeyName"].GetPrimaryKey().PrimaryKeyName);
        }
    }
}
