using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class ForeignKeyCollectionPostgreSqlTest : ForeignKeyCollectionTest
    {
        public ForeignKeyCollectionPostgreSqlTest() : base(new PostgreSqlDatabase(AssemblyInitialize.PostgreSqlConnectionString))
        {            
        }
    }
}
