using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class TypeMappingCollectionPostgreSqlTest : TypeMappingCollectionTest
    {
        public TypeMappingCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {           
        }

        [TestMethod]
        public override void TestAddCharColumn()
        {
            TestAddType((char)(char.MinValue + 1), char.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public override void TestAddByteColumn()
        {
            base.TestAddByteColumn();
        }

        [TestMethod]
        public override void TestAddNullableCharColumn()
        {
            TestAddType<char?>(null, (char)(char.MinValue + 1), char.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public override void TestAddNullableByteColumn()
        {
            base.TestAddNullableByteColumn();
        }

        [TestMethod]
        public override void TestAddTimeSpanColumn()
        {
            TestAddType(TimeSpan.Zero, TimeSpan.FromDays(1).Subtract(TimeSpan.FromMilliseconds(1)));
        }

        [TestMethod]
        public override void TestAddNullableTimeSpanColumn()
        {
            TestAddType<TimeSpan?>(null, TimeSpan.Zero, TimeSpan.FromDays(1).Subtract(TimeSpan.FromMilliseconds(1)));
        }
    }
}
