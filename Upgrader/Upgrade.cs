using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Upgrader.Infrastructure;
using Upgrader.Schema;

using IsolationLevel = System.Transactions.IsolationLevel;

namespace Upgrader
{
    public class Upgrade<TDatabase> where TDatabase : Database
    {
        private TransactionMode transactionMode = TransactionMode.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="Upgrade{TDatabase}" /> class.
        /// </summary>
        /// <param name="database">Database instance to use for upgrades.</param>
        public Upgrade(TDatabase database)
        {
            Validate.IsNotNull(database, nameof(database));

            Database = database;
        }

        /// <summary>
        /// Gets or sets the table name used for tracking executed steps.
        /// </summary>
        public string ExecutedStepsTable { get; set; } = "ExecutedSteps";

        /// <summary>
        /// Gets the current database instance in use.
        /// </summary>
        public TDatabase Database { get; }

        /// <summary>
        /// Gets or sets the Transaction Mode to use when migrating.
        /// </summary>
        public TransactionMode TransactionMode
        {
            get => transactionMode;
            set
            {
                if (value != TransactionMode.None)
                {
                    Database.SupportsTransactionalDataDescriptionLanguage();
                }

                transactionMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the transaction timeout.
        /// </summary>
        public TimeSpan TransactionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the Transaction Isolation Level to use when migrating with "TransactionMode" is not "None".
        /// </summary>
        public IsolationLevel TransactionIsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// Perform database schema upgrade. Will only execute steps that are not already executed on the target database.
        /// </summary>
        /// <param name="steps">Steps to evaluate and execute if not already executed.</param>
        public void PerformUpgrade(IEnumerable<IStep> steps)
        {
            Validate.IsNotNull(steps, nameof(steps));

            var stepsShallowClone = steps.ToArray();
            var stepNames = stepsShallowClone.Select(step => step.StepName).ToArray();

            var allStepNamesHaveValue = stepNames.All(stepName => string.IsNullOrEmpty(stepName) == false);
            Validate.IsTrue(allStepNamesHaveValue, nameof(steps), "Step names must be non-null and non-empty.");

            var firstTooLongStepName = stepNames
                .FirstOrDefault(stepName => stepName.Length > 100);

            Validate.IsTrue(firstTooLongStepName == null, nameof(steps), $"Step names must be 100 characters or less,  \"{firstTooLongStepName}\" is {firstTooLongStepName?.Length} characters long.");

            var firstDuplicateStepName = stepNames
                .GroupBy(stepName => stepName)
                .Where(step => step.Count() > 1)
                .Select(step => step.Key)
                .FirstOrDefault();

            Validate.IsTrue(firstDuplicateStepName == null, nameof(steps), $"Step names must be unique, \"{firstDuplicateStepName}\" occurs more than once.");

            using (new MutexScope("PerformUpgrade" + Database.DatabaseName.ToLowerInvariant()))
            {
                if (Database.Exists == false)
                {
                    Database.Create();
                }

                if (Database.Tables[ExecutedStepsTable] == null)
                {
                    Database.Tables.Add(ExecutedStepsTable, new Column<string>("Step", 100), new Column<DateTime>("ExecutedAt"));
                }

                var alreadyExecutedStepNames = new HashSet<string>(Database.Dapper.Query<string>($"SELECT {Database.EscapeIdentifier("Step")} FROM {Database.EscapeIdentifier(ExecutedStepsTable)}"));

                var notExecutedSteps = stepsShallowClone.Where(step => alreadyExecutedStepNames.Contains(step.StepName) == false).ToArray();
                foreach (var step in notExecutedSteps)
                {
                    if (TransactionMode == TransactionMode.OneTransactionPerStep)
                    {
                        ExecuteTransactionStep(Database, TransactionIsolationLevel, TransactionTimeout, ExecutedStepsTable, step);
                    }
                    else
                    {
                        ExecuteStep(Database, ExecutedStepsTable, step);
                    }
                }
            }
        }

        private static void ExecuteTransactionStep(Database database, IsolationLevel isolationLevel, TimeSpan timeout, string changeTable, IStep step)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = isolationLevel, Timeout = timeout }))
            using (var stepDatabase = database.Clone())
            {
                ExecuteStep(stepDatabase, changeTable, step);
                transaction.Complete();
            }
        }

        private static void ExecuteStep(Database database, string changeTable, IStep step)
        {
            step.Execute(database);
            database.Tables[changeTable].Rows.Add(new { Step = step.StepName, ExecutedAt = DateTime.Now });
        }
    }
}