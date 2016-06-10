using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class ForeignKeyCollectionSqLiteTest : ForeignKeyCollectionTest
    {
        public ForeignKeyCollectionSqLiteTest() : base(new SqLiteDatabase("Data Source=UpgraderTest.sqlite;Version=3;"))
        {            
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void RemoveRemovesForeignKey()
        {
            base.RemoveRemovesForeignKey();
        }
    }
}
