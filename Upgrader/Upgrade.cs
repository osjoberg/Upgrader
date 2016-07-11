using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Transactions;
using Upgrader.Infrastructure;
using Upgrader.Schema;

namespace Upgrader
{
    public class Upgrade<TDatabase> where TDatabase : Database
    {
        /// <summary>
        /// Geta or seta the table name used for tracking executed steps.
        /// </summary>
        public string ExecutedStepsTable { get; set; } = "ExecutedSteps";

        /// <summary>
        /// Gets the current database instance in use.
        /// </summary>
        public TDatabase Database { get; }

        /// <summary>
        /// Gets or sets the Transaction Mode to use when migrating.
        /// </summary>
        public TransactionMode TransactionMode { get; set; } = TransactionMode.OneTransactionPerStep;

        /// <summary>
        /// Create a new instance of the Upgrade engine.
        /// </summary>
        /// <param name="database">Database instance to use for upgrades.</param>
        public Upgrade(TDatabase database)
        {
            Validate.IsNotNull(database, nameof(database));

            Database = database;
        }

        /// <summary>
        /// Perform database schema upgrade. Will only execute steps that are not already executed on the target database.
        /// </summary>
        /// <param name="steps">Steps to evaluate and execute if not already executed.</param>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
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

            bool? databaseExists;
            try
            {
                databaseExists = Database.Exists;
            }
            catch (UpgraderException)
            {
                databaseExists = null;
            }

            if (databaseExists == false)
            {
                Database.Create();
            }

            if (Database.Tables[ExecutedStepsTable] == null)
            {
                Database.Tables.Add(
                    ExecutedStepsTable, new Column("Step", "NVARCHAR(100)"), new Column("ExecutedAt", "DATETIME"));
            }

            var alreadyExecutedStepNames = new HashSet<string>(Database.Dapper.Query<string>($"SELECT Step FROM {ExecutedStepsTable}"));

            var notExecutedSteps = stepsShallowClone.Where(step => alreadyExecutedStepNames.Contains(step.StepName) == false).ToArray();
            foreach (var step in notExecutedSteps)
            {
                if (TransactionMode == TransactionMode.OneTransactionPerStep) 
                {
                   ExecuteTransactionStep(Database, ExecutedStepsTable, step);   
                }
                else
                {
                    ExecuteStep(Database, ExecutedStepsTable, step);
                }                
            }
        }

        private static void ExecuteTransactionStep(Database database, string changeTable, IStep step)
        {
            using (var transaction = new TransactionScope())
            {
                ExecuteStep(database, changeTable, step);
                transaction.Complete();
            }
        }

        private static void ExecuteStep(Database database, string changeTable, IStep step)
        {
            step.Execute(database);
            database.Dapper.Execute($"INSERT INTO {changeTable} VALUES(@Name, @ExecutedAt)", new { Name = step.StepName, ExecutedAt = DateTime.Now });
        }
    }
}