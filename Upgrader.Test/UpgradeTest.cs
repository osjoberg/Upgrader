using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class UpgradeTest<TDatabase> where TDatabase : Database
    {
        private readonly TDatabase database;
        private readonly string connectionString;

        protected UpgradeTest(TDatabase database, string connectionString)
        {
            this.database = database;
            this.connectionString = connectionString;
        }

        [TestCleanup]
        public void Cleanup()
        {
            database.Dispose();
        }

        [TestMethod]
        public void PerformUpgradeCreatesDatabaseIfItDoesNotExist()
        {
            if (database.Exists)
            {
                database.Remove();
            }

            var upgrade = new Upgrade<TDatabase>(connectionString);
            upgrade.PerformUpgrade(Enumerable.Empty<IStep>());

            Assert.IsTrue(database.Exists);

            database.Remove();
        }

        [TestMethod]
        public void PerformUpgradeCreatesTableIfItDoesNotExist()
        {
            var upgrade = new Upgrade<TDatabase>(connectionString)
            {
                ExecutedStepsTable = "UpgradeCreatesTable"
            };

            upgrade.PerformUpgrade(Enumerable.Empty<IStep>());

            CollectionAssert.AreEqual(new[] { "Step", "ExecutedAt" }, database.Tables["UpgradeCreatesTable"].Columns.Select(column => column.ColumnName).ToArray());
        }

        [TestMethod]
        public void PerformUpgradeExecutesSteps()
        {
            var upgrade = new Upgrade<TDatabase>(connectionString)
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
            var upgrade = new Upgrade<TDatabase>(connectionString)
            {
                ExecutedStepsTable = "UpgradeExecutesStepsRecords"
            };

            var steps = new List<Step>
            {
                new Step("StepName", () => { })
            };

            upgrade.PerformUpgrade(steps);

            Assert.AreEqual("StepName", database.Tables["UpgradeExecutesStepsRecords"].Rows.Query().Single().Step);
        }

        [TestMethod]
        public void PerformUpgradeDoesNotExecutesAlreadyExecutedSteps()
        {
            var upgrade = new Upgrade<TDatabase>(connectionString)
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
        public virtual void PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs()
        {
            PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs(connectionString);
        }

        [TestMethod]
        public void ConnectionIsOpenAfterInstanceIsCreated()
        {
            database.Connection.Execute("SELECT 1");
        }

        [TestMethod]
        public void PerformUpgradeWithTransactionModeNoneDoesNotRollbackChangesWhenExceptionOccurs()
        {
            var upgrade = new Upgrade<TDatabase>(connectionString)
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
                        database.Tables.Add("NonAtomicTable", new Column<int>("NonAtomicTableId"));
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

            Assert.IsTrue(database.Tables.Exists("NonAtomicTable"));
        }

        protected void PerformUpgradeWithTransactionModeOneTransactionPerStepDoesRollbackChangesWhenExceptionOccurs(string overrideConnectionString)
        {
            var upgrade = new Upgrade<TDatabase>(overrideConnectionString)
            {
                ExecutedStepsTable = "UpgradeTransactionModeOneTransactionPerStep",
                TransactionMode = TransactionMode.OneTransactionPerStep
            };

            var steps = new List<Step>
            {
                new Step(
                    "Atomic",
                    db =>
                    {
                        db.Tables.Add("AtomicTable", new Column<int>("AtomicTableId"));
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

            Assert.IsFalse(database.Tables.Exists("AtomicTable"));
        }
    }
}
