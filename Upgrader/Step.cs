using System;

namespace Upgrader
{
    /// <summary>
    /// Describes a step in the schema upgrade.
    /// </summary>
    public class Step : IStep
    {
        private readonly Action<Database> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class with a unique name and action.
        /// </summary>
        /// <param name="stepName">Unique name identifying the step.</param>
        /// <param name="action">Action to perform when step is executed.</param>
        public Step(string stepName, Action<Database> action)
        {
            this.action = action;
            StepName = stepName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class with a unique name and action.
        /// </summary>
        /// <param name="stepName">Unique name identifying the step.</param>
        /// <param name="action">Action to perform when step is executed.</param>
        public Step(string stepName, Action action) : this(stepName, database => action.Invoke())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class with a unique name. Only used when inheriting from <see cref="Step"/>.
        /// </summary>
        /// <param name="stepName">Unique name identifying the step.</param>
        protected Step(string stepName) : this(stepName, () => { throw new InvalidOperationException("Execute method needs to be overridden when type is inherited from Step."); })
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class with a unique name based on the type name of the inheriting class. Only used when inheriting from <see cref="Step"/>.
        /// </summary>
        protected Step() : this("")
        {
            StepName = this.GetType().Name;
        }

        /// <summary>
        /// Gets the unique name of the step.
        /// </summary>
        public virtual string StepName { get; }

        /// <summary>
        /// Execute the step.
        /// </summary>
        /// <param name="database">Database instance to be used when executing the step.</param>
        public virtual void Execute(Database database)
        {
            action(database);
        }
    }
}
