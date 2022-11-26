using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class RowCollectionTest
    {
        protected readonly Database Database;

        protected RowCollectionTest(Database database)
        {
            this.Database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Dispose();
        }

        [TestMethod]
        public void AddInsertsRowIntoTable()
        {
            Database.Tables.Add("InsertRowIntoTable", new Column<int>("InsertRowIntoTableId"));

            Database.Tables["InsertRowIntoTable"].Rows.Add(new { InsertRowIntoTableId = 5 });

            Assert.AreEqual(5, Database.Tables["InsertRowIntoTable"].Rows.Query().Single().InsertRowIntoTableId);
        }

        [TestMethod]
        public void AddSetsIdentity()
        {
            Database.Tables.Add("InsertRowIntoTableSetIdentity", new Column<int>("InsertRowIntoTableSetIdentityId", ColumnModifier.AutoIncrementPrimaryKey), new Column<int>("Value"));

            var data = new InsertRowIntoTableSetIdentity { InsertRowIntoTableSetIdentityId = 0, Value = 5 };
            Database.Tables["InsertRowIntoTableSetIdentity"].Rows.Add(data);

            Assert.AreEqual(1, data.InsertRowIntoTableSetIdentityId);
        }

        [TestMethod]
        public void UpdateUpdatesRow()
        {
            Database.Tables.Add("UpdateUpdatesRow", new Column<int>("UpdateUpdatesRowId", ColumnModifier.AutoIncrementPrimaryKey), new Column<int>("Value"));

            Database.Tables["UpdateUpdatesRow"].Rows.Add(new { Value = 1 });
            Database.Tables["UpdateUpdatesRow"].Rows.Update(new { UpdateUpdatesRowId = 1, Value = 2 });

            Assert.AreEqual(2, Database.Tables["UpdateUpdatesRow"].Rows.Query().Single().Value);
        }

        [TestMethod]
        public void RemoveDeletesRow()
        {
            Database.Tables.Add("RemoveDeletesRow", new Column<int>("RemoveDeletesRowId", ColumnModifier.PrimaryKey), new Column<int>("Value"));

            Database.Tables["RemoveDeletesRow"].Rows.Add(new { RemoveDeletesRowId = 1, Value = 1 });
            Database.Tables["RemoveDeletesRow"].Rows.Remove(new { RemoveDeletesRowId = 1 });

            Assert.AreEqual(0, Database.Tables["RemoveDeletesRow"].Rows.Query().Count());
        }

        [TestMethod]
        public void RemoveWithoutWhereDeletesAllRows()
        {
            Database.Tables.Add("RemoveDeletesAllRows", new Column<int>("RemoveDeletesAllRowsId", ColumnModifier.PrimaryKey), new Column<int>("Value"));

            Database.Tables["RemoveDeletesAllRows"].Rows.Add(new { RemoveDeletesAllRowsId = 1, Value = 1 });
            Database.Tables["RemoveDeletesAllRows"].Rows.Remove();

            Assert.AreEqual(0, Database.Tables["RemoveDeletesAllRows"].Rows.Query().Count());
        }

        [TestMethod]
        public virtual void RemoveWhereDeletesSpecificRows()
        {
            Database.Tables.Add("RemoveDeletesSpecificRows", new Column<int>("RemoveDeletesSpecificRowsId", ColumnModifier.PrimaryKey), new Column<int>("Value"));

            Database.Tables["RemoveDeletesSpecificRows"].Rows.Add(new { RemoveDeletesSpecificRowsId = 1, Value = 1 });
            Database.Tables["RemoveDeletesSpecificRows"].Rows.Add(new { RemoveDeletesSpecificRowsId = 2, Value = 2 });
            Database.Tables["RemoveDeletesSpecificRows"].Rows.Remove("Value = 1");

            Assert.AreEqual(1, Database.Tables["RemoveDeletesSpecificRows"].Rows.Query().Count());
        }

        private class InsertRowIntoTableSetIdentity
        {
            public int InsertRowIntoTableSetIdentityId { get; set; }

            public int Value { get; set; }
        }
    }
}
