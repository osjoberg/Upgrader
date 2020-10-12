using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;
using Upgrader.Schema;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class TableCollectionMySqlTest : TableCollectionTest
    {
        public TableCollectionMySqlTest() : base(new MySqlDatabase(AssemblyInitialize.MySqlConnectionString))
        {
        }

        [TestMethod]
        public override void TablesCanBeEnumerated()
        {
            Database.Tables.Add("EnumerateTable", new Column<int>("EnumerateTableId"));

            Assert.AreEqual(1, Database.Tables.Count(table => table.TableName.Equals("EnumerateTable", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
