using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class PrimaryKeyInfoTest
    {
        protected Database Database { get; }

        protected PrimaryKeyInfoTest(Database database)
        {
            this.Database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Dispose();
        }

        [TestMethod]
        public void TableNameReturnsTableName()
        {
            Database.Tables.Add("PrimaryKeyTableName", new Column("PrimaryKeyTableNameId", "int", ColumnModifier.PrimaryKey));
            
            Assert.AreEqual("PrimaryKeyTableName", Database.Tables["PrimaryKeyTableName"].PrimaryKey.TableName);
        }


        [TestMethod]
        public virtual void PrimaryKeyIsNamedAccordingToNamingConvention()
        {
            Database.Tables.Add("PrimaryKeyName", new Column("PrimaryKeyNameId", "int"));
            Database.Tables["PrimaryKeyName"].AddPrimaryKey("PrimaryKeyNameId");

            Assert.AreEqual("PK_PrimaryKeyName_PrimaryKeyNameId", Database.Tables["PrimaryKeyName"].PrimaryKey.PrimaryKeyName);
        }
    }
}
