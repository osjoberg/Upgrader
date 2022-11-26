using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.Schema;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class ColumnInfoSqlServerTest : ColumnInfoTest
    {
        public ColumnInfoSqlServerTest() : base(new SqlServerDatabase(AssemblyInitialize.SqlServerConnectionString))
        {            
        }

        [TestMethod]
        public void IsGeneratedIsTrueForGeneratedColumn()
        {
            Database.Tables.Add("IsGeneratedIsTrueForGeneratedColumn", new Column("Generated", "rowversion"));
            Assert.IsTrue(Database.Tables["IsGeneratedIsTrueForGeneratedColumn"].Columns["Generated"].IsGenerated());
        }
    }
}
