using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class ColumnInfoTest
    {
        private readonly Database database;

        protected ColumnInfoTest(Database database)
        {
            this.database = database;
        }

        [TestCleanup]
        public void Cleanup()
        {
            database.Dispose();
        }

        [TestMethod]
        public void NullableIsTrueForNullableColumn()
        {
            database.Tables.Add("NullableColumn", new Column("NullableColumnId", "int", true));

            Assert.IsTrue(database.Tables["NullableColumn"].Columns["NullableColumnId"].Nullable);
        }

        [TestMethod]
        public void TableNameReturnsTableName()
        {
            database.Tables.Add("TableName", new Column("TableNameId", "int"));

            Assert.AreEqual("TableName", database.Tables["TableName"].Columns.Single().TableName);
        }

        [TestMethod]
        public void ColumnNameReturnsColumnName()
        {
            database.Tables.Add("ColumnName", new Column("ColumnNameId", "int"));

            Assert.AreEqual("ColumnNameId", database.Tables["ColumnName"].Columns.Single().ColumnName);
        }

        [TestMethod]
        public void NullableIsFalseForNonNullableColumn()
        {
            database.Tables.Add("NonNullableColumn", new Column("NonNullableColumnId", "int"));

            Assert.IsFalse(database.Tables["NonNullableColumn"].Columns["NonNullableColumnId"].Nullable);
        }

        [TestMethod]
        public void TypeIsIntForIntColumn()
        {
            database.Tables.Add("TypeColumn", new Column("TypeColumnId", "int"));

            Assert.AreEqual("int", database.Tables["TypeColumn"].Columns["TypeColumnId"].DataType);
        }

        [TestMethod]
        public virtual void ChangeTypeChangesType()
        {
            database.Tables.Add("ChangeType", new Column("ChangeTypeId", "int"));
            database.Tables["ChangeType"].Columns["ChangeTypeId"].ChangeType("float", true);

            Assert.AreEqual("float", database.Tables["ChangeType"].Columns["ChangeTypeId"].DataType);
        }

        [TestMethod]
        public virtual void ChangeTypeChangesTypePreservingNullability()
        {
            database.Tables.Add("ChangeTypeNullable", new Column("ChangeTypeNullableId", "int"));
            database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].ChangeType("float");

            Assert.AreEqual("float", database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].DataType);
            Assert.IsFalse(database.Tables["ChangeTypeNullable"].Columns["ChangeTypeNullableId"].Nullable);
        }


        [TestMethod]
        public virtual void RenameRenamesColumn()
        {
            database.Tables.Add("RenameColumn", new Column("RenameColumnId", "int"));

            database.Tables["RenameColumn"].Columns["RenameColumnId"].Rename("NewColumnNameId");

            Assert.IsNotNull(database.Tables["RenameColumn"].Columns["NewColumnNameId"]);
        }
    }
}
