using System;

namespace Upgrader
{
    [Serializable]
    public sealed class UpgraderException : Exception
    {
        public static UpgraderException CannotCreateInstance(string typeName, string assembly)
        {
            return new UpgraderException($"Cannot create instance of type \"{typeName}\". Make sure \"{assembly}\" is loaded in the app domain.");            
        }

        public static UpgraderException CannotExecuteStatement(string sql, object parameters, Exception innerException)
        {
            return new UpgraderException(sql, parameters, innerException);
        }

        public object Parameters { get; }

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
