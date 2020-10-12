using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class ColumnInfoSqLiteTest : ColumnInfoTest
    {
        public ColumnInfoSqLiteTest() : base(new SqLiteDatabase(AssemblyInitialize.SqLiteConnectionString))
        {            
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void ChangeDataTypeChangesTypePreservingNullable()
        {
            base.ChangeDataTypeChangesTypePreservingNullable();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void ChangeDataTypeChangesTypePreservingNullable2()
        {
            base.ChangeDataTypeChangesTypePreservingNullable2();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void ChangeDataTypeChangesType()
        {
            base.ChangeDataTypeChangesType();
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
