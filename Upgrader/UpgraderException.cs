using System;

namespace Upgrader
{
    /// <summary>
    /// Exception used to wrap internal exceptions in Upgrader. 
    /// </summary>
    [Serializable]
    public sealed class UpgraderException : Exception
    {
        internal static UpgraderException CannotCreateInstance(string typeName, string assembly)
        {
            return new UpgraderException($"Cannot create instance of type \"{typeName}\". Make sure \"{assembly}\" is loaded in the app domain.");            
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
