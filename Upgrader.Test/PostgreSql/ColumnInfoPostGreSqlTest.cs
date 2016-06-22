using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;
using Upgrader.Schema;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class ColumnInfoPostgreSqlTest : ColumnInfoTest
    {
        public ColumnInfoPostgreSqlTest() : base(new PostgreSqlDatabase("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=UpgraderTest;"))
        {
        }

        [TestMethod]
        public override void TypeIsIntForIntColumn()
        {
            Database.Tables.Add("TypeColumn", new Column("TypeColumnId", "int"));

            Assert.AreEqual("integer", Database.Tables["TypeColumn"].Columns["TypeColumnId"].DataType);
        }

        [TestMethod]
        public override void ChangeTypeChangesType()
        {
            Database.Tables.Add("ChangeType", new Column("ChangeTypeId", "int"));
            Database.Tables["ChangeType"].Columns["ChangeTypeId"].ChangeType("real", true);

            Assert.AreEqual("real", Database.Tables["ChangeType"].Columns["ChangeTypeId"].DataType);
        }

        [TestMethod]
        public override void ChangeTypeChangesTypePreservingNullability()
        {
            Database.Tables.Add("ChangeTypeNullable", new Column("ChangeTypeNullableId", "int"));
            Database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].ChangeType("real");

            Assert.AreEqual("real", Database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].DataType);
            Assert.IsFalse(Database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].Nullable);
        }
    }
}
