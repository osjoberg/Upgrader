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
        public string ExecutedStepsTable { get; set; } = "ExecutedSteps";

        public TDatabase Database { get; }

        public TransactionMode TransactionMode { get; set; } = TransactionMode.OneTransactionPerStep;

        public Upgrade(TDatabase database)
        {
            Validate.IsNotNull(database, nameof(database));

            Database = database;
        }

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
                .SingleOrDefault();

            Validate.IsTrue(firstDuplicateStepName == null, nameof(steps), $"Step names must be unique, \"{firstDuplicateStepName}\" occurs more than once.");

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