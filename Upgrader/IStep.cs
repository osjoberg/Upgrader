namespace Upgrader
{
    /// <summary>
    /// Interface describing a step.
    /// </summary>
    public interface IStep
    {
        /// <summary>
        /// Gets the unique name of the step.
        /// </summary>
        string StepName { get; }

        /// <summary>
        /// Execute the step.
        /// </summary>
        /// <param name="database">Database instance used for step execution.</param>
        void Execute(Database database);
    }
}
