using System;
using System.Threading;

namespace Upgrader
{
    internal class MutexScope : IDisposable
    {
        private readonly Mutex mutex;

        public MutexScope(string name)
        {
            bool created;
            mutex = new Mutex(true, name, out created);
            if (created == false)
            {
                mutex.WaitOne();
            }
        }

        public void Dispose()
        {
            mutex.ReleaseMutex();
        }
    }
}