using System;
using System.Data.SqlTypes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class TypeMappingCollectionSqlServerTest : TypeMappingCollectionTest
    {
        public TypeMappingCollectionSqlServerTest() : base(new SqlServerDatabase(AssemblyInitialize.SqlServerConnectionString))
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
        [ExpectedException(typeof(ArgumentException))]
        public override void TestAddTimeSpanColumn()
        {
            base.TestAddTimeSpanColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public override void TestAddNullableTimeSpanColumn()
        {
            base.TestAddNullableTimeSpanColumn();
        }
    }
}
