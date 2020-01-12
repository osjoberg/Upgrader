using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class IndexInfoTest
    {
        private readonly Database database;

        protected IndexInfoTest(Database database)
        {
            this.database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            database.Dispose();
        }

        [TestMethod]
        public void TableNameMatchesTableName()
        {
            database.Tables.Add("IndexInfoTableName", new Column<int>("IndexInfoTableNameId"));
            database.Tables["IndexInfoTableName"].Indexes.Add("IndexInfoTableNameId");            

            Assert.AreEqual("IndexInfoTableName", database.Tables["IndexInfoTableName"].Indexes.Single().TableName);
        }

        [TestMethod]
        public void IndexNameMatchesTableName()
        {
            database.Tables.Add("IndexInfoIndexName", new Column<int>("IndexInfoIndexNameId"));
            database.Tables["IndexInfoIndexName"].Indexes.Add("IndexInfoIndexNameId");

            Assert.AreEqual("IX_IndexInfoIndexName_IndexInfoIndexNameId", database.Tables["IndexInfoIndexName"].Indexes.Single().IndexName);
        }

        [TestMethod]
        public void ColumnNamesMatchesColumnNames()
        {
            database.Tables.Add("IndexInfoColumnNames", new Column<int>("IndexInfoColumnNamesId"));
            database.Tables["IndexInfoColumnNames"].Indexes.Add("IndexInfoColumnNamesId");

            Assert.AreEqual("IndexInfoColumnNamesId", database.Tables["IndexInfoColumnNames"].Indexes.Single().GetColumnNames().Single());
        }

        [TestMethod]
        public void UniqueIsTrueForUniqueIndexes()
        {
            database.Tables.Add("IndexInfoUniqueIndex", new Column<int>("IndexInfoUniqueIndexId"));
            database.Tables["IndexInfoUniqueIndex"].Indexes.Add("IndexInfoUniqueIndexId", true);

            Assert.IsTrue(database.Tables["IndexInfoUniqueIndex"].Indexes.Single().IsUnique());
        }

        [TestMethod]
        public void UniqueIsFalseForNonUniqueIndexes()
        {
            database.Tables.Add("IndexInfoNonUniqueIndex", new Column<int>("IndexInfoNonUniqueIndexId"));
            database.Tables["IndexInfoNonUniqueIndex"].Indexes.Add("IndexInfoNonUniqueIndexId");

            Assert.IsFalse(database.Tables["IndexInfoNonUniqueIndex"].Indexes.Single().IsUnique());
        }
    }
}
