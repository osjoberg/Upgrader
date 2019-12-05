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

        public ConnectionFactory(params AdoProvider[] adoProviders)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var adoAssembly = assemblies
                .FirstOrDefault(assembly => adoProviders.Any(ap => ap.AssemblyFileName == assembly.ManifestModule.Name));

            var adoProvider = adoAssembly == null ? null : adoProviders.First(ap => ap.AssemblyFileName == adoAssembly.ManifestModule.Name);

            if (adoAssembly == null)
            {
                adoProvider = adoProviders.FirstOrDefault(ap => File.Exists(ap.AssemblyFileName));
                if (adoProvider == null)
                {
                    throw UpgraderException.CannotFindAssembly(adoProviders.Select(ap => ap.AssemblyFileName).ToArray());
                }

                adoAssembly = Assembly.LoadFrom(adoProvider.AssemblyFileName);
            }

            connectionType = adoAssembly.GetTypes().SingleOrDefault(type => type.Namespace + "." + type.Name == adoProvider.ConnectionTypeName);
            if (connectionType == null)
            {
                throw UpgraderException.CannotCreateInstance(adoProvider.ConnectionTypeName, adoProvider.AssemblyFileName);
            }
        }

        public ConnectionFactory(string assemblyFileName, string connectionTypeName) : this(new AdoProvider(assemblyFileName, connectionTypeName))
        {
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            return (IDbConnection)Activator.CreateInstance(connectionType, connectionString);
        }
    }
}
