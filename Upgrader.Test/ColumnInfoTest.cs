using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class ColumnInfoTest
    {
        protected readonly Database Database;

        protected ColumnInfoTest(Database database)
        {
            Database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Dispose();
        }

        [TestMethod]
        public void NullableIsTrueForNullableColumn()
        {
            Database.Tables.Add("NullableColumn", new Column("NullableColumnId", "int", true));

            Assert.IsTrue(Database.Tables["NullableColumn"].Columns["NullableColumnId"].Nullable);
        }

        [TestMethod]
        public void TableNameReturnsTableName()
        {
            Database.Tables.Add("TableName", new Column("TableNameId", "int"));

            Assert.AreEqual("TableName", Database.Tables["TableName"].Columns.Single().TableName);
        }

        [TestMethod]
        public void ColumnNameReturnsColumnName()
        {
            Database.Tables.Add("ColumnName", new Column("ColumnNameId", "int"));

            Assert.AreEqual("ColumnNameId", Database.Tables["ColumnName"].Columns.Single().ColumnName);
        }

        [TestMethod]
        public void NullableIsFalseForNonNullableColumn()
        {
            Database.Tables.Add("NonNullableColumn", new Column("NonNullableColumnId", "int"));

            Assert.IsFalse(Database.Tables["NonNullableColumn"].Columns["NonNullableColumnId"].Nullable);
        }

        [TestMethod]
        public virtual void TypeIsIntForIntColumn()
        {
            Database.Tables.Add("TypeColumn", new Column("TypeColumnId", "int"));

            Assert.AreEqual("int", Database.Tables["TypeColumn"].Columns["TypeColumnId"].DataType);
        }

        [TestMethod]
        public virtual void ChangeTypeChangesType()
        {
            Database.Tables.Add("ChangeType", new Column("ChangeTypeId", "int"));
            Database.Tables["ChangeType"].Columns["ChangeTypeId"].ChangeType("float", true);

            Assert.AreEqual("float", Database.Tables["ChangeType"].Columns["ChangeTypeId"].DataType);
        }

        [TestMethod]
        public virtual void ChangeTypeChangesTypePreservingNullability()
        {
            Database.Tables.Add("ChangeTypeNullable", new Column("ChangeTypeNullableId", "int"));
            Database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].ChangeType("float");

            Assert.AreEqual("float", Database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].DataType);
            Assert.IsFalse(Database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].Nullable);
        }

        [TestMethod]
        public virtual void RenameRenamesColumn()
        {
            Database.Tables.Add("RenameColumn", new Column("RenameColumnId", "int"));

            Database.Tables["RenameColumn"].Columns["RenameColumnId"].Rename("NewColumnNameId");

            Assert.IsNotNull(Database.Tables["RenameColumn"].Columns["NewColumnNameId"]);
        }

        [TestMethod]
        public void AutoIncrementIsTrueWhenColumnIsAutoIncrement()
        {
            Database.Tables.Add("auto_increment", new Column("auto_increment_id", "integer", ColumnModifier.AutoIncrementPrimaryKey));

            Assert.IsTrue(Database.Tables["auto_increment"].Columns["auto_increment_id"].AutoIncrement);
        }

        [TestMethod]
        public void AutoIncrementIsFalseWhenColumnIsNotAutoIncrement()
        {
            Database.Tables.Add("NoAutoIncrement", new Column("NoAutoIncrementId", "integer"));

            Assert.IsFalse(Database.Tables["NoAutoIncrement"].Columns["NoAutoIncrementId"].AutoIncrement);
        }
    }
}
