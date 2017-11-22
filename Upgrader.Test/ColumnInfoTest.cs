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
        public virtual void ChangeDataTypeChangesType()
        {
            Database.Tables.Add("ChangeDataType", new Column<int>("ChangeDataTypeId"));
            Database.Tables["ChangeDataType"].Columns["ChangeDataTypeId"].ChangeDataType<float>();

            Assert.AreEqual("real", Database.Tables["ChangeDataType"].Columns["ChangeDataTypeId"].DataType);
        }

        [TestMethod]
        public virtual void ChangeDataTypeChangesTypePreservingNullable()
        {
            Database.Tables.Add("ChangeDataTypeNullable", new Column("ChangeDataTypeNullableId", "int"));
            Database.Tables["ChangeDataTypeNullable"].Columns["ChangeDataTypeNullableId"].ChangeDataType<float>();

            Assert.AreEqual("real", Database.Tables["ChangeDataTypeNullable"].Columns["ChangeDataTypeNullableId"].DataType);
            Assert.IsFalse(Database.Tables["ChangeDataTypeNullable"].Columns["ChangeDataTypeNullableId"].Nullable);
        }

        [TestMethod]
        public virtual void ChangeDataTypeChangesTypePreservingNullable2()
        {
            Database.Tables.Add("ChangeDataTypeNullable2", new Column<int>("ChangeDataTypeNullable2Id"));
            Database.Tables["ChangeDataTypeNullable2"].Columns["ChangeDataTypeNullable2Id"].ChangeDataType<double>();
            
            Assert.AreEqual(Database.TypeMappings[typeof(double)], Database.Tables["ChangeDataTypeNullable2"].Columns["ChangeDataTypeNullable2Id"].DataType);
            Assert.IsFalse(Database.Tables["ChangeDataTypeNullable2"].Columns["ChangeDataTypeNullable2Id"].Nullable);
        }

        [TestMethod]
        public virtual void RenamePreservesAutoIncrementPrimaryKeyColumn()
        {
            Database.Tables.Add("RenameAutoIncrementPrimaryKeyColumn", new Column("id", "int", ColumnModifier.AutoIncrementPrimaryKey));
            Database.Tables["RenameAutoIncrementPrimaryKeyColumn"].Columns.Rename("id", "RenameAutoIncrementPrimaryKeyColumnId");

            Assert.IsNotNull(Database.Tables["RenameAutoIncrementPrimaryKeyColumn"].PrimaryKey);
            Assert.IsTrue(Database.Tables["RenameAutoIncrementPrimaryKeyColumn"].Columns["RenameAutoIncrementPrimaryKeyColumnId"].AutoIncrement);
        }

        [TestMethod]
        public virtual void RenamePreservesPrimaryKeyColumn()
        {
            Database.Tables.Add("RenamePrimaryKeyColumn", new Column("id", "int", ColumnModifier.PrimaryKey));
            Database.Tables["RenamePrimaryKeyColumn"].Columns.Rename("id", "RenamePrimaryKeyColumnId");

            Assert.IsNotNull(Database.Tables["RenamePrimaryKeyColumn"].PrimaryKey);
        }

        [TestMethod]
        public void AutoIncrementIsTrueWhenColumnIsAutoIncrement()
        {
            Database.Tables.Add("AutoIncrement", new Column("AutoIncrementId", "integer", ColumnModifier.AutoIncrementPrimaryKey));

            Assert.IsTrue(Database.Tables["AutoIncrement"].Columns["AutoIncrementId"].AutoIncrement);
        }

        [TestMethod]
        public void AutoIncrementIsFalseWhenColumnIsNotAutoIncrement()
        {
            Database.Tables.Add("NoAutoIncrement", new Column("NoAutoIncrementId", "integer"));

            Assert.IsFalse(Database.Tables["NoAutoIncrement"].Columns["NoAutoIncrementId"].AutoIncrement);
        }

        [TestMethod]
        public virtual void DataTypeIncludesLengthOnVarchar()
        {
            Database.Tables.Add("VarcharLength", new Column("VarcharLengthId", "varchar(10)"));

            Assert.AreEqual("varchar(10)", Database.Tables["VarcharLength"].Columns["VarcharLengthId"].DataType);
        }

        [TestMethod]
        public virtual void DataTypeIncludesPrecisionOnDecimal()
        {
            Database.Tables.Add("DecimalPrecision", new Column("DecimalPrecisionId", "decimal(9)"));

            Assert.AreEqual("decimal(9)", Database.Tables["DecimalPrecision"].Columns["DecimalPrecisionId"].DataType);
        }

        [TestMethod]
        public virtual void DataTypeIncludesPrecisionAndScaleOnDecimalIfSpecified()
        {
            Database.Tables.Add("DecimalPrecisionAndScale", new Column("DecimalPrecisionAndScaleId", "decimal(8,2)"));

            Assert.AreEqual("decimal(8,2)", Database.Tables["DecimalPrecisionAndScale"].Columns["DecimalPrecisionAndScaleId"].DataType);
        }

        [TestMethod]
        public virtual void DataTypeDoesIncludeLengthOnChar()
        {
            Database.Tables.Add("SingleChar", new Column("SingleCharId", "char(1)"));

            Assert.AreEqual("char(1)", Database.Tables["SingleChar"].Columns["SingleCharId"].DataType);
        }
    }
}
