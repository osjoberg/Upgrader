using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class ColumnInfoSqLiteTest : ColumnInfoTest
    {
        public ColumnInfoSqLiteTest() : base(new SqLiteDatabase("SqLite"))
        {            
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void ChangeTypeChangesTypePreservingNullable()
        {
            base.ChangeTypeChangesTypePreservingNullable();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void ChangeTypeChangesType()
        {
            base.ChangeTypeChangesType();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RenameRenamesColumn()
        {
            base.RenameRenamesColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RenamePreservesAutoIncrementPrimaryKeyColumn()
        {
            base.RenamePreservesAutoIncrementPrimaryKeyColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RenamePreservesPrimaryKeyColumn()
        {
            base.RenamePreservesPrimaryKeyColumn();
        }
    }
}
