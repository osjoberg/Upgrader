using System;
using System.Data;
using System.Linq;

namespace Upgrader.Infrastructure
{
    internal class ConnectionFactory
    {
        private readonly Type connectionType;

        public ConnectionFactory(string assemblyFileName, string connectionTypeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var mySqlAssembly = assemblies.SingleOrDefault(assembly => assembly.ManifestModule.Name == assemblyFileName);
            if (mySqlAssembly == null)
            {
                throw UpgraderException.CannotCreateInstance(connectionTypeName, assemblyFileName);
            }

            connectionType = mySqlAssembly.GetType(connectionTypeName, false);
            if (connectionType == null)
            {
                throw UpgraderException.CannotCreateInstance(connectionTypeName, assemblyFileName);
            }
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            return (IDbConnection)Activator.CreateInstance(connectionType, connectionString);
        }
    }
}
