using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class IndexInfoPostgreSqlTest : IndexInfoTest
    {
        public IndexInfoPostgreSqlTest() : base(new PostgreSqlDatabase(AssemblyInitialize.PostgreSqlConnectionString))
        {            
        }
    }
}
