using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class TypeMappingCollectionMySqlTest : TypeMappingCollectionTest
    {
        public TypeMappingCollectionMySqlTest() : base(new MySqlDatabase("MySql"))
        {           
        }

        [TestMethod]
        public override void TestAddDateTimeColumn()
        {
            TestAddType(new DateTime(1000, 01, 01, 00, 00, 00), new DateTime(9999, 12, 31, 23, 59, 59));
        }

        [TestMethod]
        public override void TestAddNullableDateTimeColumn()
        {
            TestAddType<DateTime?>(null, new DateTime(1000, 01, 01, 00, 00, 00), new DateTime(9999, 12, 31, 23, 59, 59));
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
