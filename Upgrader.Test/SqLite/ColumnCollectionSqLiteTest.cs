using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class ColumnCollectionSqLiteTest : ColumnCollectionTest
    {
        public ColumnCollectionSqLiteTest() : base(new SqLiteDatabase("SqLite"))
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
