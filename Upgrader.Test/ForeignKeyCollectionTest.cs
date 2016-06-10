using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class ForeignKeyCollectionTest
    {
        private readonly Database database;

        protected ForeignKeyCollectionTest(Database database)
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
            if (database.Tables["ForeignTable"] == null)
            { 
                database.Tables.Add("ForeignTable", new Column("ForeignTableId", "int", ColumnModifier.PrimaryKey));
                database.Tables.Add(
                    "ParentTable", 
                    new[] { new Column("ParentTableId", "int", ColumnModifier.PrimaryKey), new Column("ForeignTableId", "int") }, 
                    new[] { new ForeignKey("ForeignTableId", "ForeignTable") });
            }
        }

        [TestMethod]
        public void AddAddsForeignKey()
        {
            Assert.AreEqual("ForeignTableId", database.Tables["ParentTable"].ForeignKeys["FK_ParentTable_ForeignTableId_ForeignTable"].ColumnNames.Single());
        }

        [TestMethod]
        public void CanEnumerateForeignKeys()
        {
            Assert.AreEqual("FK_ParentTable_ForeignTableId_ForeignTable", database.Tables["ParentTable"].ForeignKeys.Single().ForeignKeyName);
        }

        [TestMethod]
        public void ReturnsNotNullForExistingForeignKey()
        {
            Assert.IsNotNull(database.Tables["ParentTable"].ForeignKeys["FK_ParentTable_ForeignTableId_ForeignTable"]);
        }

        [TestMethod]
        public void ReturnsNullForNonExistingForeignKey()
        {
            Assert.IsNull(database.Tables["ParentTable"].ForeignKeys["FK_ParentTable_ForeignTableId_ForeignTable_ForeignTableId2"]);
        }

        [TestMethod]
        public virtual void RemoveRemovesForeignKey()
        {
            database.Tables.Add(
                "RemoveForeignKeyTable",
                new[] { new Column("RemoveForeignKeyTableId", "int", ColumnModifier.PrimaryKey), new Column("ForeignTableId", "int") },
                new[] { new ForeignKey("ForeignTableId", "ForeignTable", "ForeignTableId", "DuplicateFK") });

            database.Tables["RemoveForeignKeyTable"].ForeignKeys.Remove("DuplicateFK");

            Assert.IsNull(database.Tables["RemoveForeignKeyTable"].ForeignKeys["DuplicateFK"]);
        }
    }
}
