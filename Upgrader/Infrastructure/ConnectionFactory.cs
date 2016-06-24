using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Upgrader.Infrastructure
{
    internal class ConnectionFactory
    {
        private readonly Type connectionType;

        public ConnectionFactory(string assemblyFileName, string connectionTypeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            var adoProviderAssembly = assemblies.SingleOrDefault(assembly => assembly.ManifestModule.Name == assemblyFileName);
            if (adoProviderAssembly == null && File.Exists(assemblyFileName))
            {
                adoProviderAssembly = Assembly.LoadFrom(assemblyFileName);
            }

            if (adoProviderAssembly == null)
            {
                throw UpgraderException.CannotCreateInstance(connectionTypeName, assemblyFileName);
            }

            connectionType = adoProviderAssembly.GetType(connectionTypeName, false);
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
