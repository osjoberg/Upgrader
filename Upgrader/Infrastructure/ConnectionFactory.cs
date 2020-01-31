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

            var adoProviderAssembly = assemblies
                .Select(a => new AdoProviderAssembly(adoProviders.FirstOrDefault(ap => ap.AssemblyFileName == a.ManifestModule.Name), a))
                .Where(apa => apa.AdoProvider != null)
                .OrderBy(apa => Array.IndexOf(adoProviders, apa.AdoProvider))
                .FirstOrDefault() ;

            if (adoProviderAssembly == null)
            {
                adoProviderAssembly = adoProviders.Where(ap => File.Exists(ap.AssemblyFileName))
                    .Select(ap => new AdoProviderAssembly(ap, Assembly.LoadFrom(ap.AssemblyFileName)))
                    .FirstOrDefault();
            }

            if (adoProviderAssembly == null)
            {
                throw UpgraderException.CannotFindAssembly(adoProviders.Select(ap => ap.AssemblyFileName).ToArray());
            }

            var adoProvider = adoProviderAssembly.AdoProvider;
            var assembly = adoProviderAssembly.Assembly;

            connectionType = assembly.GetTypes().SingleOrDefault(type => type.Namespace + "." + type.Name == adoProvider.ConnectionTypeName);
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

        internal class AdoProviderAssembly
        {
            public AdoProviderAssembly(AdoProvider adoProvider, Assembly assembly)
            {
                AdoProvider = adoProvider;
                Assembly = assembly;
            }

            public AdoProvider AdoProvider { get; }

            public Assembly Assembly { get; }
        }
    }
}
