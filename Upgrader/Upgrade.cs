using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using Upgrader.Infrastructure;
using Upgrader.MySql;
using Upgrader.PostgreSql;
using Upgrader.Schema;
using Upgrader.SqlServer;

using IsolationLevel = System.Transactions.IsolationLevel;

namespace Upgrader
{
    using Upgrader.SqLite;

    public class Upgrade<TDatabase> where TDatabase : Database
    {
        private readonly TDatabase obsoleteDatabase;
        private readonly string connectionStringOrName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Upgrade{TDatabase}"/> class.
        /// </summary>
        /// <param name="connectionStringOrName"> The connection string or connection name to get connection string for.</param>
        public Upgrade(string connectionStringOrName)
        {
            Validate.IsNotNullAndNotEmpty(connectionStringOrName, nameof(connectionStringOrName));

            this.connectionStringOrName = connectionStringOrName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Upgrade{TDatabase}" /> class.
        /// </summary>
        /// <param name="database">Database instance to use for upgrades.</param>
        [Obsolete]
        public Upgrade(TDatabase database)
        {
            Validate.IsNotNull(database, nameof(database));

            obsoleteDatabase = database;
        }

        /// <summary>
        /// Gets or sets the table name used for tracking executed steps.
        /// </summary>
        public string ExecutedStepsTable { get; set; } = "ExecutedSteps";

        /// <summary>
        /// Gets the current database instance in use.
        /// </summary>
        [Obsolete]
        public TDatabase Database => this.obsoleteDatabase;

        /// <summary>
        /// Gets or sets the Transaction Mode to use when migrating.
        /// </summary>
        public TransactionMode TransactionMode { get; set; } = TransactionMode.None;

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

            var firstTooLongStepName = stepNames.FirstOrDefault(stepName => stepName.Length > 100);

            Validate.IsTrue(firstTooLongStepName == null, nameof(steps), $"Step names must be 100 characters or less,  \"{firstTooLongStepName}\" is {firstTooLongStepName?.Length} characters long.");

            var firstDuplicateStepName = stepNames
                .GroupBy(stepName => stepName)
                .Where(step => step.Count() > 1)
                .Select(step => step.Key)
                .FirstOrDefault();

            Validate.IsTrue(firstDuplicateStepName == null, nameof(steps), $"Step names must be unique, \"{firstDuplicateStepName}\" occurs more than once.");

            Database database = null;
            try
            {
                database = obsoleteDatabase ?? CreateDatabaseInstance();

                using (new MutexScope("PerformUpgrade" + database.DatabaseName.ToLowerInvariant()))
                {
                    if (TransactionMode != TransactionMode.None)
                    {
                        database.SupportsTransactionalDataDescriptionLanguage();
                    }

                    if (database.Exists == false)
                    {
                        database.Create();
                    }

                    if (database.Tables.Exists(ExecutedStepsTable) == false)
                    {
                        database.Tables.Add(ExecutedStepsTable, new Column<string>("Step", 100), new Column<DateTime>("ExecutedAt"));
                    }

                    var alreadyExecutedStepNames = new HashSet<string>(database.Dapper.Query<string>($"SELECT {database.EscapeIdentifier("Step")} FROM {database.EscapeIdentifier(ExecutedStepsTable)}"));

                    var notExecutedSteps = stepsShallowClone
                        .Where(step => alreadyExecutedStepNames.Contains(step.StepName) == false)
                        .ToArray();

                    foreach (var step in notExecutedSteps)
                    {
                        if (TransactionMode == TransactionMode.OneTransactionPerStep)
                        {
                            ExecuteTransactionStep(database, TransactionIsolationLevel, TransactionTimeout, ExecutedStepsTable, step);
                        }
                        else
                        {
                            ExecuteStep(database, ExecutedStepsTable, step);
                        }
                    }
                }
            }
            finally
            {
                if (obsoleteDatabase == null)
                {
                    database?.Dispose();
                }
            }
        }

        private static void ExecuteStep(Database database, string changeTable, IStep step)
        {
            step.Execute(database);
            database.Tables[changeTable].Rows.Add(new { Step = step.StepName, ExecutedAt = DateTime.Now });
        }

        private Database CreateDatabaseInstance()
        {
            if (typeof(TDatabase) == typeof(SqlServerDatabase))
            {
                return new SqlServerDatabase(connectionStringOrName);
            }

            if (typeof(TDatabase) == typeof(PostgreSqlDatabase))
            {
                return new PostgreSqlDatabase(connectionStringOrName);
            }

            if (typeof(TDatabase) == typeof(MySqlDatabase))
            {
                return new MySqlDatabase(connectionStringOrName);
            }

            if (typeof(TDatabase) == typeof(SqLiteDatabase))
            {
                return new SqLiteDatabase(connectionStringOrName);
            }

            throw new NotSupportedException();
        }

        private void ExecuteTransactionStep(Database database, IsolationLevel isolationLevel, TimeSpan timeout, string changeTable, IStep step)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = isolationLevel, Timeout = timeout }))
            using (var stepDatabase = database.Clone())
            {
                ExecuteStep(stepDatabase, changeTable, step);
                transaction.Complete();
            }
        }
    }
}