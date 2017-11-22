using System;
using System.Data.SqlTypes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class TypeMappingCollectionSqlServerTest : TypeMappingCollectionTest
    {
        public TypeMappingCollectionSqlServerTest() : base(new SqlServerDatabase("SqlServer"))
        {           
        }

        [TestMethod]
        public override void TestAddDateTimeColumn()
        {
            TestAddType(SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value);
        }

        [TestMethod]
        public override void TestAddNullableDateTimeColumn()
        {
            TestAddType<DateTime?>(null, SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value);
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
