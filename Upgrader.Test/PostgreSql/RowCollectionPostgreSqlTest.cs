using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class RowCollectionPostgreSqlTest : RowCollectionTest
    {
        public RowCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {
        }
    }
}
