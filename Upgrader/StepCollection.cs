using System;
using System.Collections.ObjectModel;

namespace Upgrader
{
    public class StepCollection : Collection<IStep>
    {
        public void Add(string stepName, Action action)
        {
            Add(new Step(stepName, action));
        }

        public void Add(string stepName, Action<Database> action)
        {
            Add(new Step(stepName, action));
        }
    }
}
