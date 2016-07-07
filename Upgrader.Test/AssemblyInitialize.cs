using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;
using Upgrader.PostgreSql;
using Upgrader.SqLite;
using Upgrader.SqlServer;

namespace Upgrader.Test
{
    [TestClass]
    public static class AssemblyInitialize
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            var databases = new Database[]
            {
                new SqlServerDatabase("SqlServer"),
                new MySqlDatabase("MySql"), 
                new PostgreSqlDatabase("PostgreSql"),
                new SqLiteDatabase("SqLite") 
            };

            foreach (var database in databases)
            {                
                try
                {
                    if (database.Exists)
                    {
                        database.Remove();
                    }

                    database.Create();
                }
                finally
                {
                    database.Dispose();
                }
            }
        }
    }
}
