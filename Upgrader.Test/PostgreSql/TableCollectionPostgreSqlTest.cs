using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class TableCollectionPostgreSqlTest : TableCollectionTest
    {
        public TableCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddWithComputedColumnCreatesTable()
        {
            base.AddWithComputedColumnCreatesTable();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddWithNullableComputedColumnCreatesTable()
        {
            base.AddWithNullableComputedColumnCreatesTable();
        }
    }
}
