using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class ColumnCollectionPostgreSqlTest : ColumnCollectionTest
    {
        public ColumnCollectionPostgreSqlTest() : base(new PostgreSqlDatabase(AssemblyInitialize.PostgreSqlConnectionString))
        {            
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddComputedAddsVirtualComputedColumn()
        {
            base.AddComputedAddsVirtualComputedColumn();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddNullableComputedAddsVirtualComputedColumn()
        {
            base.AddNullableComputedAddsVirtualComputedColumn();
        }
    }
}
