using System.Linq;

using Dapper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class TableCollectionTest
    {
        protected TableCollectionTest(Database database)
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
        public void AddCreatesTable()
        {
            Database.Tables.Add("AddTable", new[] { new Column("AddTableId", "int") });

            Assert.IsNotNull(Database.Tables["AddTable"]);
        }

        [TestMethod]
        public void AddWithAutoIncrementCreatesTableWithAutoIncrement()
        {
            Database.Tables.Add("AddAutoIncrementTable", new[] { new Column("AddAutoIncrementTableId", "int", ColumnModifier.AutoIncrementPrimaryKey), new Column("Data", "int") });

            Database.Connection.Execute("INSERT INTO AddAutoIncrementTable (Data) VALUES (12345)");

            Assert.AreEqual(1, Database.Connection.ExecuteScalar("SELECT AddAutoIncrementTableId FROM AddAutoIncrementTable"));
        }

        [TestMethod]
        public void RemoveDropsTable()
        {
            Database.Tables.Add("RemoveTable", new[] { new Column("RemoveTableId", "int") });
            Database.Tables.Remove("RemoveTable");

            Assert.IsNull(Database.Tables["RemoveTable"]);
        }

        [TestMethod]
        public virtual void TablesCanBeEnumerated()
        {
            Database.Tables.Add("EnumerateTable", new[] { new Column("EnumerateTableId", "int") });

            Assert.AreEqual(1, Database.Tables.Count(table => table.TableName == "EnumerateTable"));
        }

        [TestMethod]
        public void TablesCanBeAccessedByName()
        {
            Database.Tables.Add("SpecificTable", new[] { new Column("SpecificTableId", "int") });

            Assert.AreEqual("SpecificTable", Database.Tables["SpecificTable"].TableName);
        }

        [TestMethod]
        public void ReturnsNullWhenTableDoesNotExist()
        {
            Assert.IsNull(Database.Tables["NonExistingTable"]);
        }

        [TestMethod]
        public void ReturnsNotNullWhenTableDoesExist()
        {
            Database.Tables.Add("ExistingTable", new[] { new Column("ExistingTableId", "int") });

            Assert.IsNotNull(Database.Tables["ExistingTable"]);
        }
    }
}
