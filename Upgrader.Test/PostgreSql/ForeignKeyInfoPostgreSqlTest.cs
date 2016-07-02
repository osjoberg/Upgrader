using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class ForeignKeyInfoPostgreSqlTest : ForeignKeyInfoTest
    {
        public ForeignKeyInfoPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {            
        }
    }
}
