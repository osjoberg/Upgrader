using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class UpgradePostgreSqlTest : UpgradeTest<PostgreSqlDatabase>
    {
        public UpgradePostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {            
        }
    }
}
