using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class PrimaryKeyInfoSqLiteTest : PrimaryKeyInfoTest
    {
        public PrimaryKeyInfoSqLiteTest() : base(new SqLiteDatabase("Data Source=UpgraderTest.sqlite;Version=3;"))
        {
        }

        [TestMethod]
        public override void PrimaryKeyIsNamedAccordingToNamingConvention()
        {
            Database.Tables.Add("PrimaryKeyName", new Column("PrimaryKeyNameId", "int", ColumnModifier.PrimaryKey));

            Assert.AreEqual("", Database.Tables["PrimaryKeyName"].PrimaryKey.PrimaryKeyName);
        }
    }
}
