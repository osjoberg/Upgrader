using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;
using Upgrader.SqlServer;

namespace Upgrader.Test
{
    [TestClass]
    public class UpgradeTest
    {
        [TestMethod]
        public void PerformUpgradeCreatesTableIfItDoesNotExist()
        {
            var database = new SqlServerDatabase("Server = (local); Integrated Security = true; Initial Catalog = UpgraderTest");
            var upgrade = new Upgrade<SqlServerDatabase>(database)
            {
                ExecutedStepsTable = "UpgradeCreatesTable"
            };

            upgrade.PerformUpgrade(Enumerable.Empty<IStep>());

            CollectionAssert.AreEqual(new[] { "Step", "ExecutedAt" }, upgrade.Database.Tables["UpgradeCreatesTable"].Columns.Select(column => column.ColumnName).ToArray());
        }

        [TestMethod]
        public void PerformUpgradeExecutesSteps()
        {
            var database = new SqlServerDatabase("Server = (local); Integrated Security = true; Initial Catalog = UpgraderTest");
            var upgrade = new Upgrade<SqlServerDatabase>(database)
            {
                ExecutedStepsTable = "UpgradeExecutesSteps"
            };

            var stepExecutionCount = 0;

            var steps = new List<Step>
            {
                new Step("StepName", () => stepExecutionCount++)
            };

            upgrade.PerformUpgrade(steps);

            Assert.AreEqual(1, stepExecutionCount);
        }

        [TestMethod]
        public void PerformUpgradeRecordsExecutedStepName()
        {
            var database = new SqlServerDatabase("Server = (local); Integrated Security = true; Initial Catalog = UpgraderTest");
            var upgrade = new Upgrade<SqlServerDatabase>(database)
            {
                ExecutedStepsTable = "UpgradeExecutesStepsRecords"
            };

            var steps = new List<Step>
            {
                new Step("StepName", () => { })
            };

            upgrade.PerformUpgrade(steps);

            Assert.AreEqual("StepName", database.Connection.Query<string>("SELECT Step FROM UpgradeExecutesStepsRecords").Single());
        }

        [TestMethod]
        public void PerformUpgradeDoesNotExecutesAlreadyExecutedSteps()
        {
            var database = new SqlServerDatabase("Server = (local); Integrated Security = true; Initial Catalog = UpgraderTest");
            var upgrade = new Upgrade<SqlServerDatabase>(database)
            {
                ExecutedStepsTable = "UpgradeExecutesNotExecutedSteps"
            };

            var stepExecutionCount = 0;

            var steps = new List<Step>
            {
                new Step("StepName", () => stepExecutionCount++)
            };

            upgrade.PerformUpgrade(steps);
            upgrade.PerformUpgrade(steps);

            Assert.AreEqual(1, stepExecutionCount);
        }

        [TestMethod]
        public void PerformUpgradeWithTransactioModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs()
        {
            var database = new SqlServerDatabase("Server = (local); Integrated Security = true; Initial Catalog = UpgraderTest");
            var upgrade = new Upgrade<SqlServerDatabase>(database)
            {
                ExecutedStepsTable = "UpgradeTransactionModeOneTransactionPerStep"
            };

            var steps = new List<Step>
            {
                new Step(
                    "Atomic", 
                    () =>
                    {
                        database.Tables.Add("AtomicTable", new Column("AtomicTableId", "int"));
                        throw new InvalidOperationException("Injected fault");                            
                    })
            };

            try
            {
                upgrade.PerformUpgrade(steps);
            }
            catch (InvalidOperationException)
            {
            }

            Assert.IsNull(database.Tables["AtomicTable"]);
        }

        [TestMethod]
        public void PerformUpgradeWithTransactioModeNoneDoesNotRollbackChangesWhenExceptionOccurs()
        {
            var database = new SqlServerDatabase("Server = (local); Integrated Security = true; Initial Catalog = UpgraderTest");
            var upgrade = new Upgrade<SqlServerDatabase>(database)
            {
                ExecutedStepsTable = "UpgradeTransactionModeOneTransactionPerStep", 
                TransactionMode = TransactionMode.None               
            };

            var steps = new List<Step>
            {
                new Step(
                    "NonAtomic", 
                    () =>
                    {
                        database.Tables.Add("NonAtomicTable", new Column("NonAtomicTableId", "int"));
                        throw new InvalidOperationException("Injected fault");
                    })
            };

            try
            {
                upgrade.PerformUpgrade(steps);
            }
            catch (InvalidOperationException)
            {
            }

            Assert.IsNotNull(database.Tables["NonAtomicTable"]);
        }
    }
}
