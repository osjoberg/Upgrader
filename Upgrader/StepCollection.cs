using System;
using System.Collections.ObjectModel;

namespace Upgrader
{
    /// <summary>
    /// Collection of Steps.
    /// </summary>
    public class StepCollection : Collection<IStep>
    {
        /// <summary>
        /// Add a step to the collection.
        /// </summary>
        /// <param name="stepName">Unique name of the step.</param>
        /// <param name="action">Action to execute.</param>
        public void Add(string stepName, Action action)
        {
            Add(new Step(stepName, action));
        }

        /// <summary>
        /// Add a step to the collection.
        /// </summary>
        /// <param name="stepName">Unique name of the step.</param>
        /// <param name="action">Action to execute.</param>
        public void Add(string stepName, Action<Database> action)
        {
            Add(new Step(stepName, action));
        }
    }
}
