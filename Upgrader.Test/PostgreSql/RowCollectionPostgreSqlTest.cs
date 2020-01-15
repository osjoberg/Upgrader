using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.PostgreSql;
using Upgrader.Schema;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class RowCollectionPostgreSqlTest : RowCollectionTest
    {
        public RowCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {
        }

        public override void RemoveWhereDeletesSpecificRows()
        {
            Database.Tables.Add("RemoveDeletesSpecificRows", new Column<int>("RemoveDeletesSpecificRowsId", ColumnModifier.PrimaryKey), new Column<int>("Value"));

            Database.Tables["RemoveDeletesSpecificRows"].Rows.Add(new { RemoveDeletesSpecificRowsId = 1, Value = 1 });
            Database.Tables["RemoveDeletesSpecificRows"].Rows.Add(new { RemoveDeletesSpecificRowsId = 2, Value = 2 });
            Database.Tables["RemoveDeletesSpecificRows"].Rows.Remove("\"Value\" = 1");

            Assert.AreEqual(1, Database.Tables["RemoveDeletesSpecificRows"].Rows.Query().Count());
        }
    }
}
