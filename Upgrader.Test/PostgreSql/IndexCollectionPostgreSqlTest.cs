using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class IndexCollectionPostgreSqlTest : IndexCollectionTest
    {
        public IndexCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=UpgraderTest;"))
        {            
        }
    }
}
