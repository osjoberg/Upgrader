using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class ColumnCollectionSqLiteTest : ColumnCollectionTest
    {
        public ColumnCollectionSqLiteTest() : base(new SqLiteDatabase(AssemblyInitialize.SqLiteConnectionString))
        {            
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddAddsColumn()
        {
            base.AddAddsColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddComputedAddsStoredComputedColumn()
        {
            base.AddComputedAddsStoredComputedColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddComputedAddsVirtualComputedColumn()
        {
            base.AddComputedAddsVirtualComputedColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddNullableComputedAddsStoredComputedColumn()
        {
            base.AddNullableComputedAddsStoredComputedColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddNullableComputedAddsVirtualComputedColumn()
        {
            base.AddNullableComputedAddsVirtualComputedColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RenameRenamesColumn()
        {
            base.RenameRenamesColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void CanAddNonNullColumn()
        {
            base.CanAddNonNullColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RemoveDropsColumn()
        {
            base.RemoveDropsColumn();
        }
    }
}
