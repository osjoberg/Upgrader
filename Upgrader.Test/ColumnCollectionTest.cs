﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class ColumnCollectionTest
    {
        private readonly Database database;

        protected ColumnCollectionTest(Database database)
        {
            this.database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            database.Dispose();
        }

        [TestMethod]
        public void AddAddsColumn()
        {
            database.Tables.Add("AddColumn", new[] { new Column("AddColumnId", "int") });

            database.Tables["AddColumn"].Columns.Add("AddedColumn", "int");

            Assert.AreEqual(2, database.Tables["AddColumn"].Columns.Count());
        }

        [TestMethod]
        public void RemoveDropsColumn()
        {
            database.Tables.Add("RemoveColumn", new[] { new Column("RemoveColumnId", "int"), new Column("RemovedColumn", "int") });

            database.Tables["RemoveColumn"].Columns.Remove("RemovedColumn");

            Assert.AreEqual(1, database.Tables["RemoveColumn"].Columns.Count());
        }

        [TestMethod]
        public void ColumnsCanBeEnumerated()
        {
            database.Tables.Add("EnumerateColumn", new[] { new Column("EnumerateColumnId", "int") });

            Assert.AreEqual(1, database.Tables["EnumerateColumn"].Columns.Count(column => column.ColumnName == "EnumerateColumnId"));
        }

        [TestMethod]
        public void ColumnsCanBeAccessedByName()
        {
            database.Tables.Add("SpecificColumn", new[] { new Column("SpecificColumnId", "int") });

            Assert.AreEqual("SpecificColumnId", database.Tables["SpecificColumn"].Columns["SpecificColumnId"].ColumnName);
        }

        [TestMethod]
        public void ReturnsNullWhenColumnDoesNotExist()
        {
            database.Tables.Add("DoesNotContainColumn", new[] { new Column("ContainsColumnId", "int") });

            Assert.IsNull(database.Tables["DoesNotContainColumn"].Columns["ContainsColumnId2"]);
        }

        [TestMethod]
        public void ReturnsNotNullTrueWhenColumnDoesExist()
        {
            database.Tables.Add("ContainsColumn", new[] { new Column("ContainsColumnId", "int") });

            Assert.IsNotNull(database.Tables["ContainsColumn"].Columns["ContainsColumnId"]);
        }
    }
}
