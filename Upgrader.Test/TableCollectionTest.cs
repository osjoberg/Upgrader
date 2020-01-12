using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class TableCollectionTest
    {
        protected TableCollectionTest(Database database)
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
        public void AddCreatesTable()
        {
            Database.Tables.Add("AddTable", new Column<int>("AddTableId"));

            Assert.IsNotNull(Database.Tables["AddTable"]);
        }

        [TestMethod]
        public void AddWithPrecisionCreatesTable()
        {
            Database.Tables.Add("AddDecimalTable", new Column<decimal?>("AddDecimalTableId", 4, 2));

            Assert.IsTrue(Database.Tables["AddDecimalTable"].Columns["AddDecimalTableId"].GetDataType().EndsWith("(4,2)"));
            Assert.IsTrue(Database.Tables["AddDecimalTable"].Columns["AddDecimalTableId"].IsNullable());
        }

        [TestMethod]
        public void AddWithAutoIncrementCreatesTableWithAutoIncrement()
        {
            Database.Tables.Add("AddAutoIncrementTable", new Column<int>("AddAutoIncrementTableId", ColumnModifier.AutoIncrementPrimaryKey), new Column<int>("Data"));

            Database.Tables["AddAutoIncrementTable"].Rows.Add(new { Data = 12345 });

            Assert.AreEqual(1, Database.Tables["AddAutoIncrementTable"].Rows.Query().Single().AddAutoIncrementTableId);
            Assert.IsFalse(Database.Tables["AddAutoIncrementTable"].Columns["AddAutoIncrementTableId"].IsNullable());
        }

        [TestMethod]
        public void RemoveDropsTable()
        {
            Database.Tables.Add("RemoveTable", new Column<int>("RemoveTableId"));
            Database.Tables.Remove("RemoveTable");

            Assert.IsFalse(Database.Tables.Exists("RemoveTable"));
        }

        [TestMethod]
        public virtual void TablesCanBeEnumerated()
        {
            Database.Tables.Add("EnumerateTable", new Column<int>("EnumerateTableId"));

            Assert.AreEqual(1, Database.Tables.Count(table => table.TableName == "EnumerateTable"));
        }

        [TestMethod]
        public void TablesCanBeAccessedByName()
        {
            Database.Tables.Add("SpecificTable", new Column<int>("SpecificTableId"));

            Assert.AreEqual("SpecificTable", Database.Tables["SpecificTable"].TableName);
        }

        [TestMethod]
        public void ExistsReturnsFalseWhenTableDoesNotExist()
        {
            Assert.IsFalse(Database.Tables.Exists("NonExistingTable"));
        }

        [TestMethod]
        public void ExistsReturnsTrueWhenTableDoesExist()
        {
            Database.Tables.Add("ExistingTable", new Column<int>("ExistingTableId"));

            Assert.IsTrue(Database.Tables.Exists("ExistingTable"));
        }
    }
}
