using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class IndexCollectionPostgreSqlTest : IndexCollectionTest
    {
        public IndexCollectionPostgreSqlTest() : base(new PostgreSqlDatabase(AssemblyInitialize.PostgreSqlConnectionString))
        {            
        }
    }
}
