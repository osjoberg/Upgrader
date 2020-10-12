using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class UpgradePostgreSqlTest : UpgradeTest<PostgreSqlDatabase>
    {
        public UpgradePostgreSqlTest() : base(new PostgreSqlDatabase(AssemblyInitialize.PostgreSqlConnectionString), AssemblyInitialize.PostgreSqlConnectionString)
        {
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs()
        {
            base.PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs();
        }

        [TestMethod]
        public void PerformUpgradeWithTransactionModeOneTransactionPerStepWithEnlistTrueDoesRollbackChangesWhenExceptionOccurs()
        {            
            base.PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs(AssemblyInitialize.PostgreSqlEnlist);
        }
    }
}