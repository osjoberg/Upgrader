using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class UpgradePostgreSqlTest : UpgradeTest<PostgreSqlDatabase>
    {
        public UpgradePostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs()
        {
            base.PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs();
        }

        [TestMethod]
        public void PerformUpgradeWithTransactioModeOneTransactionPerStepWithEnlistTrueDoesRollbackChangesWhenExceptionOccurs()
        {            
            base.PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs(new PostgreSqlDatabase("PostgreSqlEnlist"));
        }
    }
}