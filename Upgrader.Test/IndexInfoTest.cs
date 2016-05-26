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
            database.Tables.Add("IndexInfoTableName", new [] { new Column("IndexInfoTableNameId",  "int") });
            database.Tables["IndexInfoTableName"].Indexes.Add("IndexInfoTableNameId", false);            

            Assert.AreEqual("IndexInfoTableName", database.Tables["IndexInfoTableName"].Indexes.Single().TableName);
        }

        [TestMethod]
        public void IndexNameMatchesTableName()
        {
            database.Tables.Add("IndexInfoIndexName", new[] { new Column("IndexInfoIndexNameId", "int") });
            database.Tables["IndexInfoIndexName"].Indexes.Add("IndexInfoIndexNameId", false);

            Assert.AreEqual("IX_IndexInfoIndexName_IndexInfoIndexNameId", database.Tables["IndexInfoIndexName"].Indexes.Single().IndexName);
        }

        [TestMethod]
        public void ColumnNamesMatchesColumnNames()
        {
            database.Tables.Add("IndexInfoColumnNames", new[] { new Column("IndexInfoColumnNamesId", "int") });
            database.Tables["IndexInfoColumnNames"].Indexes.Add("IndexInfoColumnNamesId", false);

            Assert.AreEqual("IndexInfoColumnNamesId", database.Tables["IndexInfoColumnNames"].Indexes.Single().ColumnNames.Single());
        }

        [TestMethod]
        public void UniqueIsTrueForUniqueIndexes()
        {
            database.Tables.Add("IndexInfoUniqueIndex", new[] { new Column("IndexInfoUniqueIndexId", "int") });
            database.Tables["IndexInfoUniqueIndex"].Indexes.Add("IndexInfoUniqueIndexId", true);

            Assert.IsTrue(database.Tables["IndexInfoUniqueIndex"].Indexes.Single().Unique);
        }

        [TestMethod]
        public void UniqueIsFalseForNonUniqueIndexes()
        {
            database.Tables.Add("IndexInfoNonUniqueIndex", new[] { new Column("IndexInfoNonUniqueIndexId", "int") });
            database.Tables["IndexInfoNonUniqueIndex"].Indexes.Add("IndexInfoNonUniqueIndexId", false);

            Assert.IsFalse(database.Tables["IndexInfoNonUniqueIndex"].Indexes.Single().Unique);
        }
    }
}
