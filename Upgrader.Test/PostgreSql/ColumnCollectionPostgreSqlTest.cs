using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class ColumnCollectionPostgreSqlTest : ColumnCollectionTest
    {
        public ColumnCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {            
        }
    }
}
