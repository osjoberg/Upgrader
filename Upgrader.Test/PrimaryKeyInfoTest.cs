using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class PrimaryKeyInfoTest
    {
        protected PrimaryKeyInfoTest(Database database)
        {
            Database = database;
        }

        protected Database Database { get; }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Dispose();
        }

        [TestMethod]
        public void TableNameReturnsTableName()
        {
            Database.Tables.Add("PrimaryKeyTableName", new Column<int>("PrimaryKeyTableNameId", ColumnModifier.PrimaryKey));
            
            Assert.AreEqual("PrimaryKeyTableName", Database.Tables["PrimaryKeyTableName"].GetPrimaryKey().TableName);
        }

        [TestMethod]
        public virtual void PrimaryKeyIsNamedAccordingToNamingConvention()
        {
            Database.Tables.Add("PrimaryKeyName", new Column<int>("PrimaryKeyNameId", ColumnModifier.PrimaryKey));

            Assert.AreEqual("PK_PrimaryKeyName_PrimaryKeyNameId", Database.Tables["PrimaryKeyName"].GetPrimaryKey().PrimaryKeyName);
        }
    }
}
