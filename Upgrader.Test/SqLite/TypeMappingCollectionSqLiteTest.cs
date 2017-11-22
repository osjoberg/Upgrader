using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class TypeMappingCollectionSqLiteTest : TypeMappingCollectionTest
    {
        public TypeMappingCollectionSqLiteTest() : base(new SqLiteDatabase("SqLite"))
        {           
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public override void TestAddNullableTimeSpanColumn()
        {
            base.TestAddNullableTimeSpanColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddBooleanColumn()
        {
            base.TestAddBooleanColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddByteColumn()
        {
            base.TestAddByteColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddCharColumn()
        {
            base.TestAddCharColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddDateTimeColumn()
        {
            base.TestAddDateTimeColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddDecimalColumn()
        {
            base.TestAddDecimalColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddDoubleColumn()
        {
            base.TestAddDoubleColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddFloatColumn()
        {
            base.TestAddFloatColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddGuidColumn()
        {
            base.TestAddGuidColumn();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddInt16Column()
        {
            base.TestAddInt16Column();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddInt32Column()
        {
            base.TestAddInt32Column();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddInt64Column()
        {
            base.TestAddInt64Column();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddStringColumnDefaultLength()
        {
            base.TestAddStringColumnDefaultLength();
        }

        [ExpectedException(typeof(NotSupportedException))]
        [TestMethod]
        public override void TestAddStringColumnSpecifiedLength()
        {
            base.TestAddStringColumnSpecifiedLength();
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public override void TestAddTimeSpanColumn()
        {
            base.TestAddTimeSpanColumn();
        }
    }
}
