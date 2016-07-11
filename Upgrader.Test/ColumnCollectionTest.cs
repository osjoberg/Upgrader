using System.Linq;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class ColumnCollectionTest
    {
        protected readonly Database Database;

        protected ColumnCollectionTest(Database database)
        {
            this.Database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Dispose();
        }

        [TestMethod]
        public virtual void AddAddsColumn()
        {
            Database.Tables.Add("AddColumn", new Column("AddColumnId", "int"));

            Database.Tables["AddColumn"].Columns.Add("AddedColumn", "int");

            Assert.AreEqual(2, Database.Tables["AddColumn"].Columns.Count());
        }

        [TestMethod]
        public virtual void RemoveDropsColumn()
        {
            Database.Tables.Add("RemoveColumn", new Column("RemoveColumnId", "int"), new Column("RemovedColumn", "int"));

            Database.Tables["RemoveColumn"].Columns.Remove("RemovedColumn");

            Assert.AreEqual(1, Database.Tables["RemoveColumn"].Columns.Count());
        }

        [TestMethod]
        public void ColumnsCanBeEnumerated()
        {
            Database.Tables.Add("EnumerateColumn", new Column("EnumerateColumnId", "int"));

            Assert.AreEqual(1, Database.Tables["EnumerateColumn"].Columns.Count(column => column.ColumnName == "EnumerateColumnId"));
        }

        [TestMethod]
        public void ColumnsCanBeAccessedByName()
        {
            Database.Tables.Add("SpecificColumn", new Column("SpecificColumnId", "int"));

            Assert.AreEqual("SpecificColumnId", Database.Tables["SpecificColumn"].Columns["SpecificColumnId"].ColumnName);
        }

        [TestMethod]
        public void ReturnsNullWhenColumnDoesNotExist()
        {
            Database.Tables.Add("DoesNotContainColumn", new Column("ContainsColumnId", "int"));

            Assert.IsNull(Database.Tables["DoesNotContainColumn"].Columns["ContainsColumnId2"]);
        }

        [TestMethod]
        public void ReturnsNotNullTrueWhenColumnDoesExist()
        {
            Database.Tables.Add("ContainsColumn", new Column("ContainsColumnId", "int"));

            Assert.IsNotNull(Database.Tables["ContainsColumn"].Columns["ContainsColumnId"]);
        }

        [TestMethod]
        public virtual void CanAddNonNullColumn()
        {
            Database.Tables.Add("CanAddNotNullColumn", new Column("CanAddNotNullColumnId", "int"));
            Database.Connection.Execute("INSERT INTO CanAddNotNullColumn VALUES (1)");
            Database.Tables["CanAddNotNullColumn"].Columns.Add("NewNotNullColumn", "int", 5);

            Assert.AreEqual(ColumnModifier.None, Database.Tables["CanAddNotNullColumn"].Columns["NewNotNullColumn"].Modifier);
        }

        [TestMethod]
        public void AddNullableAddsNullableColumn()
        {
            Database.Tables.Add("CanAddNullableColumn", new Column("CanAddNullableColumnId", "int"));
            Database.Tables["CanAddNullableColumn"].Columns.AddNullable("NullableColumn", "int");

            Assert.AreEqual(ColumnModifier.Nullable, Database.Tables["CanAddNullableColumn"].Columns["NullableColumn"].Modifier);
        }
    }
}
