using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class TableCollectionSqLiteTest : TableCollectionTest
    {
        public TableCollectionSqLiteTest() : base(new SqLiteDatabase(AssemblyInitialize.SqLiteConnectionString))
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

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddWithPersistedComputedColumnCreatesTable()
        {
            base.AddWithPersistedComputedColumnCreatesTable();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddWithPersistedNullableComputedColumnCreatesTable()
        {
            base.AddWithPersistedNullableComputedColumnCreatesTable();
        }
    }
}
