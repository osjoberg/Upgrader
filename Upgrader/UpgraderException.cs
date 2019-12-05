using System;
using System.Linq;

namespace Upgrader
{
    /// <summary>
    /// Exception used to wrap internal exceptions in Upgrader. 
    /// </summary>
    [Serializable]
    public sealed class UpgraderException : Exception
    {
        internal static UpgraderException CannotCreateInstance(string typeName, string assemblyFilename)
        {
            return new UpgraderException($"Cannot create instance of type \"{typeName}\". Make sure \"{assemblyFilename}\" is loaded in the app domain.");            
        }

        internal static UpgraderException CannotFindAssembly(params string[] assemblyFilenames)
        {
            var assemblyFilenamesFormatted = string.Join(" or ", assemblyFilenames.Select(assemblyFilename => $"\"{assemblyFilename}\""));

            return new UpgraderException($"Cannot find asssembly named {assemblyFilenamesFormatted}.");
        }


        internal static UpgraderException CannotExecuteStatement(string sql, object parameters, Exception innerException)
        {
            return new UpgraderException(sql, parameters, innerException);
        }

        /// <summary>
        /// Gets SQL parameters for when inner exception occured.
        /// </summary>
        public object Parameters { get; }

        /// <summary>
        /// Gets SQL executed when inner exception occurred.
        /// </summary>
        public string Sql { get; }

        private UpgraderException(string sql, object parameters, Exception innerException) : base($"Error occurred when attempting to execute statement \"{sql}\". See inner exception for more details.", innerException)
        {
            Sql = sql;
            Parameters = parameters;          
        }

        private UpgraderException(string message) : base(message)
        {            
        }
    }
}
