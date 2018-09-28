using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class ColumnInfoPostgreSqlTest : ColumnInfoTest
    {
        public ColumnInfoPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RenamePreservesAutoIncrementPrimaryKeyColumn()
        {
            base.RenamePreservesAutoIncrementPrimaryKeyColumn();
        }
    }
}
