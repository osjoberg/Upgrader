using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class PrimaryKeyInfoSqLiteTest : PrimaryKeyInfoTest
    {
        public PrimaryKeyInfoSqLiteTest() : base(new SqLiteDatabase("SqLite"))
        {
        }

        [TestMethod]
        public override void PrimaryKeyIsNamedAccordingToNamingConvention()
        {
            Database.Tables.Add("PrimaryKeyName", new Column<int>("PrimaryKeyNameId", ColumnModifier.PrimaryKey));

            Assert.AreEqual("", Database.Tables["PrimaryKeyName"].GetPrimaryKey().PrimaryKeyName);
        }
    }
}
