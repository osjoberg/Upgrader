using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class PrimaryKeyInfoTest
    {
        protected PrimaryKeyInfoTest(Database database)
        {
            this.Database = database;
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
            Database.Tables.Add("PrimaryKeyTableName", new Column("PrimaryKeyTableNameId", "int", ColumnModifier.PrimaryKey));
            
            Assert.AreEqual("PrimaryKeyTableName", Database.Tables["PrimaryKeyTableName"].PrimaryKey.TableName);
        }

        [TestMethod]
        public virtual void PrimaryKeyIsNamedAccordingToNamingConvention()
        {
            Database.Tables.Add("PrimaryKeyName", new Column("PrimaryKeyNameId", "int", ColumnModifier.PrimaryKey));

            Assert.AreEqual("PK_PrimaryKeyName_PrimaryKeyNameId", Database.Tables["PrimaryKeyName"].PrimaryKey.PrimaryKeyName);
        }
    }
}
