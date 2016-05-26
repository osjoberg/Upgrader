namespace Upgrader
{
    public interface IStep
    {
        string StepName { get; }

        void Execute(Database database);
    }
}
