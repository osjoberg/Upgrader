using System;

namespace Upgrader
{
    public class Step : IStep
    {
        private readonly Action<Database> action;

        public Step(string stepName, Action<Database> action)
        {
            this.action = action;
            StepName = stepName;
        }

        public Step(string stepName, Action action) : this(stepName, database => action.Invoke())
        {
        }

        protected Step(string stepName) : this(stepName, () => { throw new InvalidOperationException("Execute method needs to be overridden when type is inherited from Step."); })
        {            
        }

        protected Step() : this("")
        {
            StepName = this.GetType().Name;
        }

        public virtual string StepName { get; }

        public virtual void Execute(Database database)
        {
            action(database);
        }
    }
}
