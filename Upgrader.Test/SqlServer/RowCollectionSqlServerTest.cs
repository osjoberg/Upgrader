using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.Schema;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class RowCollectionSqlServerTest : RowCollectionTest
    {
        public RowCollectionSqlServerTest() : base(new SqlServerDatabase(AssemblyInitialize.SqlServerConnectionString))
        {
        }

        [TestMethod]
        public void AddWithGeneratedColumnInsertsRowIntoTable()
        {
            Database.Tables.Add("InsertRowIntoTableWithGeneratedColumn", new Column<int>("InsertRowIntoTableId"), new Column("Generated", "rowversion"));
            Database.Tables["InsertRowIntoTableWithGeneratedColumn"].Rows.Add(new { InsertRowIntoTableId = 5 });
            Assert.AreEqual(5, Database.Tables["InsertRowIntoTableWithGeneratedColumn"].Rows.Query().Single().InsertRowIntoTableId);
        }
        
        [TestMethod]
        public void UpdateWithGeneratedColumnUpdatesRow()
        {
            Database.Tables.Add("UpdateUpdatesRowWithGeneratedColumn", new Column<int>("UpdateUpdatesRowId", ColumnModifier.AutoIncrementPrimaryKey), new Column<int>("Value"), new Column("Generated", "rowversion"));

            Database.Tables["UpdateUpdatesRowWithGeneratedColumn"].Rows.Add(new { Value = 1 });
            Database.Tables["UpdateUpdatesRowWithGeneratedColumn"].Rows.Update(new { UpdateUpdatesRowId = 1, Value = 2 });

            Assert.AreEqual(2, Database.Tables["UpdateUpdatesRowWithGeneratedColumn"].Rows.Query().Single().Value);
        }
    }
}
