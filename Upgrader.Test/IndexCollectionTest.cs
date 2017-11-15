using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class IndexCollectionTest
    {
        private readonly Database database;

        protected IndexCollectionTest(Database database)
        {
            this.database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            database.Dispose();
        }

        [TestMethod]
        public void AddAddsIndex()
        {
            database.Tables.Add("AddIndex", new Column("AddIndexId", "int"));
            database.Tables["AddIndex"].Indexes.Add("AddIndexId");

            Assert.IsNotNull(database.Tables["AddIndex"].Indexes.Single());
        }

        [TestMethod]
        public void AddAddsIndexWithIncludeColumn()
        {
            database.Tables.Add("AddIndexWithIncludeColumn", new Column("AddIndexId", "int"), new Column("IncludeColumn", "int"));
            database.Tables["AddIndexWithIncludeColumn"].Indexes.Add("AddIndexId", includeColumnNames: new[] { "IncludeColumn" });

            Assert.IsNotNull(database.Tables["AddIndexWithIncludeColumn"].Indexes.Single());
        }

        [TestMethod]
        public void AddAddsIndexWithMultipleColumns()
        {
            database.Tables.Add("AddIndexMultiple", new Column("AddIndexMultipleId", "int"), new Column("Multiple", "int"));
            database.Tables["AddIndexMultiple"].Indexes.Add(new[] { "AddIndexMultipleId", "Multiple" });

            CollectionAssert.AreEqual(new[] { "AddIndexMultipleId", "Multiple" }, database.Tables["AddIndexMultiple"].Indexes.Single().ColumnNames);
        }

        [TestMethod]
        public void RemoveRemovesIndex()
        {
            database.Tables.Add("RemoveIndex", new Column("RemoveIndexId", "int"));
            database.Tables["RemoveIndex"].Indexes.Add("RemoveIndexId");

            database.Tables["RemoveIndex"].Indexes.Remove("IX_RemoveIndex_RemoveIndexId");
            Assert.IsNull(database.Tables["RemoveIndex"].Indexes.SingleOrDefault());
        }

        [TestMethod]
        public void IndexesCanBeEnumerated()
        {
            database.Tables.Add("EnumerateIndexParent", new Column("EnumerateIndexParentId", "int", ColumnModifier.AutoIncrementPrimaryKey));
            database.Tables["EnumerateIndexParent"].Indexes.Add("EnumerateIndexParentId", true);

            database.Tables.Add(
                "EnumerateIndex", 
                new[] { new Column("EnumerateIndexId", "int"), new Column("EnumerateIndexParentId", "int") },
                new[] { new ForeignKey("EnumerateIndexParentId", "EnumerateIndexParent") });

            database.Tables["EnumerateIndex"].Indexes.Add("EnumerateIndexId");

            Assert.AreEqual("IX_EnumerateIndex_EnumerateIndexId", database.Tables["EnumerateIndex"].Indexes.Single().IndexName);
        }

        [TestMethod]
        public void IndexesCanBeAccessedByName()
        {
            database.Tables.Add("AccessIndex", new Column("AccessIndexId", "int"));
            database.Tables["AccessIndex"].Indexes.Add("AccessIndexId");

            Assert.AreEqual("IX_AccessIndex_AccessIndexId", database.Tables["AccessIndex"].Indexes["IX_AccessIndex_AccessIndexId"].IndexName);
        }
    }
}
