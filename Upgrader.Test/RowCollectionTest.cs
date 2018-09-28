using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class RowCollectionTest
    {
        private readonly Database database;

        protected RowCollectionTest(Database database)
        {
            this.database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.database.Dispose();
        }

        [TestMethod]
        public void AddInsertsRowIntoTable()
        {
            database.Tables.Add("InsertRowIntoTable", new Column<int>("InsertRowIntoTableId"));

            database.Tables["InsertRowIntoTable"].Rows.Add(new { InsertRowIntoTableId = 5 });

            Assert.AreEqual(5, database.Tables["InsertRowIntoTable"].Rows.Query().Single().InsertRowIntoTableId);
        }

        [TestMethod]
        public void AddSetsIdentity()
        {
            database.Tables.Add("InsertRowIntoTableSetIdentity", new Column<int>("InsertRowIntoTableSetIdentityId", ColumnModifier.AutoIncrementPrimaryKey), new Column<int>("Value"));

            var data = new InsertRowIntoTableSetIdentity { InsertRowIntoTableSetIdentityId = 0, Value = 5 };
            database.Tables["InsertRowIntoTableSetIdentity"].Rows.Add(data);

            Assert.AreEqual(1, data.InsertRowIntoTableSetIdentityId);
        }

        [TestMethod]
        public void UpdateUpdatesRow()
        {
            database.Tables.Add("UpdateUpdatesRow", new Column<int>("UpdateUpdatesRowId", ColumnModifier.AutoIncrementPrimaryKey), new Column<int>("Value"));

            database.Tables["UpdateUpdatesRow"].Rows.Add(new { Value = 1 });
            database.Tables["UpdateUpdatesRow"].Rows.Update(new { UpdateUpdatesRowId = 1, Value = 2 });

            Assert.AreEqual(2, database.Tables["UpdateUpdatesRow"].Rows.Query().Single().Value);
        }

        [TestMethod]
        public void RemoveDeletesRow()
        {
            database.Tables.Add("RemoveDeletesRow", new Column<int>("RemoveDeletesRowId", ColumnModifier.PrimaryKey), new Column<int>("Value"));

            database.Tables["RemoveDeletesRow"].Rows.Add(new { RemoveDeletesRowId = 1, Value = 1 });
            database.Tables["RemoveDeletesRow"].Rows.Remove(new { RemoveDeletesRowId = 1 });

            Assert.AreEqual(0, database.Tables["RemoveDeletesRow"].Rows.Query().Count());
        }

        private class InsertRowIntoTableSetIdentity
        {
            public int InsertRowIntoTableSetIdentityId { get; set; }

            public int Value { get; set; }
        }
    }
}
