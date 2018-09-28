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
            Database.Tables.Add("NullableColumn", new Column<int?>("NullableColumnId"));

            Assert.IsTrue(Database.Tables["NullableColumn"].Columns["NullableColumnId"].Nullable);
        }

        [TestMethod]
        public void TableNameReturnsTableName()
        {
            Database.Tables.Add("TableName", new Column<int>("TableNameId"));

            Assert.AreEqual("TableName", Database.Tables["TableName"].Columns.Single().TableName);
        }

        [TestMethod]
        public void ColumnNameReturnsColumnName()
        {
            Database.Tables.Add("ColumnName", new Column<int>("ColumnNameId"));

            Assert.AreEqual("ColumnNameId", Database.Tables["ColumnName"].Columns.Single().ColumnName);
        }

        [TestMethod]
        public void NullableIsFalseForNonNullableColumn()
        {
            Database.Tables.Add("NonNullableColumn", new Column<int>("NonNullableColumnId"));

            Assert.IsFalse(Database.Tables["NonNullableColumn"].Columns["NonNullableColumnId"].Nullable);
        }

        [TestMethod]
        public void TypeIsIntForIntColumn()
        {
            Database.Tables.Add("TypeColumn", new Column<int>("TypeColumnId"));

            Assert.AreEqual(Database.TypeMappings[typeof(int)], Database.Tables["TypeColumn"].Columns["TypeColumnId"].DataType);
        }

        [TestMethod]
        public virtual void ChangeDataTypeChangesType()
        {
            Database.Tables.Add("ChangeDataType", new Column<int>("ChangeDataTypeId"));
            Database.Tables["ChangeDataType"].Columns["ChangeDataTypeId"].ChangeDataType<float>();
            
            Assert.AreEqual(Database.TypeMappings[typeof(float)], Database.Tables["ChangeDataType"].Columns["ChangeDataTypeId"].DataType);
        }

        [TestMethod]
        public virtual void ChangeDataTypeChangesTypePreservingNullable()
        {
            Database.Tables.Add("ChangeDataTypeNullable", new Column<int>("ChangeDataTypeNullableId"));
            Database.Tables["ChangeDataTypeNullable"].Columns["ChangeDataTypeNullableId"].ChangeDataType<float>();

            Assert.AreEqual(Database.TypeMappings[typeof(float)], Database.Tables["ChangeDataTypeNullable"].Columns["ChangeDataTypeNullableId"].DataType);
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
            Database.Tables.Add("RenameAutoIncrementPrimaryKeyColumn", new Column<int>("id", ColumnModifier.AutoIncrementPrimaryKey));
            Database.Tables["RenameAutoIncrementPrimaryKeyColumn"].Columns.Rename("id", "RenameAutoIncrementPrimaryKeyColumnId");

            Assert.IsNotNull(Database.Tables["RenameAutoIncrementPrimaryKeyColumn"].PrimaryKey);
            Assert.IsTrue(Database.Tables["RenameAutoIncrementPrimaryKeyColumn"].Columns["RenameAutoIncrementPrimaryKeyColumnId"].AutoIncrement);
        }

        [TestMethod]
        public virtual void RenamePreservesPrimaryKeyColumn()
        {
            Database.Tables.Add("RenamePrimaryKeyColumn", new Column<int>("id", ColumnModifier.PrimaryKey));
            Database.Tables["RenamePrimaryKeyColumn"].Columns.Rename("id", "RenamePrimaryKeyColumnId");

            Assert.IsNotNull(Database.Tables["RenamePrimaryKeyColumn"].PrimaryKey);
        }

        [TestMethod]
        public void AutoIncrementIsTrueWhenColumnIsAutoIncrement()
        {
            Database.Tables.Add("AutoIncrement", new Column<int>("AutoIncrementId", ColumnModifier.AutoIncrementPrimaryKey));

            Assert.IsTrue(Database.Tables["AutoIncrement"].Columns["AutoIncrementId"].AutoIncrement);
        }

        [TestMethod]
        public void AutoIncrementIsFalseWhenColumnIsNotAutoIncrement()
        {
            Database.Tables.Add("NoAutoIncrement", new Column<int>("NoAutoIncrementId"));

            Assert.IsFalse(Database.Tables["NoAutoIncrement"].Columns["NoAutoIncrementId"].AutoIncrement);
        }

        [TestMethod]
        public void DataTypeIncludesLengthOnVarchar()
        {
            Database.Tables.Add("VarcharLength", new Column<string>("VarcharLengthId", 10));
           
            Assert.AreEqual(Database.TypeMappings.GetDataType(typeof(string), 10), Database.Tables["VarcharLength"].Columns["VarcharLengthId"].DataType);
        }

        [TestMethod]
        public void DataTypeIncludesPrecisionOnDecimal()
        {
            Database.Tables.Add("DecimalPrecision", new Column<decimal>("DecimalPrecisionId", 9, 0));

            Assert.AreEqual(Database.TypeMappings.GetDataType(typeof(decimal), 9, 0), Database.Tables["DecimalPrecision"].Columns["DecimalPrecisionId"].DataType);
        }

        [TestMethod]
        public void DataTypeIncludesPrecisionAndScaleOnDecimalIfSpecified()
        {
            Database.Tables.Add("DecimalPrecisionAndScale", new Column<decimal>("DecimalPrecisionAndScaleId", 8, 2));

            Assert.AreEqual(Database.TypeMappings.GetDataType(typeof(decimal), 8, 2), Database.Tables["DecimalPrecisionAndScale"].Columns["DecimalPrecisionAndScaleId"].DataType);
        }

        [TestMethod]
        public void DataTypeDoesIncludeLengthOnChar()
        {
            Database.Tables.Add("SingleChar", new Column<char>("SingleCharId"));

            Assert.AreEqual(Database.TypeMappings.GetDataType(typeof(char)), Database.Tables["SingleChar"].Columns["SingleCharId"].DataType);
        }
    }
}
