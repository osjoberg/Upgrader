using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class ForeignKeyInfoTest
    {
        private readonly Database database;

        protected ForeignKeyInfoTest(Database database)
        {
            this.database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            database.Dispose();
        }

        [TestInitialize]
        public void Initialize()
        {
            if (database.Tables["ForeignTable2"] == null)
            {
                database.Tables.Add("ForeignTable2", new Column("ForeignTable2Id", "int", ColumnModifier.PrimaryKey));
                database.Tables.Add(
                    "ForeignTable2Child", 
                    new[] { new Column("ForeignTable2ChildId", "int", ColumnModifier.PrimaryKey) }, 
                    new[] { new ForeignKey("ForeignTable2ChildId", "ForeignTable2", "ForeignTable2Id") });
            }
        }

        [TestMethod]
        public void NameContainsTheForeignKeyConstraintName()
        {
            Assert.AreEqual(
                "FK_ForeignTable2Child_ForeignTable2ChildId_ForeignTable2", 
                database.Tables["ForeignTable2Child"].ForeignKeys.Single().ForeignKeyName);
        }

        [TestMethod]
        public void TableContainsTheTableName()
        {
            Assert.AreEqual("ForeignTable2Child", database.Tables["ForeignTable2Child"].ForeignKeys.Single().TableName);
        }

        [TestMethod]
        public void ForeignTableContainsTheForeignTableName()
        {
            Assert.AreEqual("ForeignTable2", database.Tables["ForeignTable2Child"].ForeignKeys.Single().ForeignTable, true);
        }

        [TestMethod]
        public void ColumnsContainsTheTableColumns()
        {
            Assert.AreEqual("ForeignTable2ChildId", database.Tables["ForeignTable2Child"].ForeignKeys.Single().ColumnNames.Single());
        }

        [TestMethod]
        public void ForeignColumnsContainsTheForeignTableColumns()
        {
            Assert.AreEqual("ForeignTable2Id", database.Tables["ForeignTable2Child"].ForeignKeys.Single().ForeignColumnNames.Single());
        }
    }
}
