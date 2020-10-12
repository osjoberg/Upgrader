using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class UpgradeSqlServerTest : UpgradeTest<SqlServerDatabase>
    {
        public UpgradeSqlServerTest() : base(new SqlServerDatabase(AssemblyInitialize.SqlServerConnectionString), AssemblyInitialize.SqlServerConnectionString)
        {            
        }
    }
}
