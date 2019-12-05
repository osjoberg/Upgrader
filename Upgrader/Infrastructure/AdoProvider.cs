namespace Upgrader.Infrastructure
{
    internal class AdoProvider
    {
        public AdoProvider(string assemblyFileName, string connectionTypeName)
        {
            AssemblyFileName = assemblyFileName;
            ConnectionTypeName = connectionTypeName;
        }

        public string AssemblyFileName { get; }

        public string ConnectionTypeName { get; }
    }
}