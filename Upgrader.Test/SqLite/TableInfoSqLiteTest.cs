using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class TableInfoSqLiteTest : TableInfoTest
    {
        public TableInfoSqLiteTest() : base(new SqLiteDatabase(AssemblyInitialize.SqLiteConnectionString))
        {
        }

        [ExpectedException(typeof(NotSupportedException))]
        public override void AddPrimaryKeyAddsPrimaryKey()
        {
            base.AddPrimaryKeyAddsPrimaryKey();            
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddPrimaryKeyWithMultipleColumnsAddsPrimaryKeyWithMultipleColums()
        {                    
            base.AddPrimaryKeyWithMultipleColumnsAddsPrimaryKeyWithMultipleColums();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RemovePrimaryKeyRemovesPrimaryKey()
        {         
            base.RemovePrimaryKeyRemovesPrimaryKey();   
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void TruncateTruncatesTable()
        {
            base.TruncateTruncatesTable();
        }
    }
}
