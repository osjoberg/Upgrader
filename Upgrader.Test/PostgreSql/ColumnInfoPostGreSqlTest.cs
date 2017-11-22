using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;
using Upgrader.Schema;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class ColumnInfoPostgreSqlTest : ColumnInfoTest
    {
        public ColumnInfoPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {
        }

        [TestMethod]
        public override void TypeIsIntForIntColumn()
        {
            Database.Tables.Add("TypeColumn", new Column("TypeColumnId", "int"));

            Assert.AreEqual("integer", Database.Tables["TypeColumn"].Columns["TypeColumnId"].DataType);
        }

        [TestMethod]
        public override void ChangeDataTypeChangesType()
        {
            Database.Tables.Add("ChangeDataType", new Column<int>("ChangeDataTypeId"));
            Database.Tables["ChangeDataType"].Columns["ChangeDataTypeId"].ChangeDataType<float>();

            Assert.AreEqual("real", Database.Tables["ChangeDataType"].Columns["ChangeDataTypeId"].DataType);
        }

        [TestMethod]
        public override void ChangeDataTypeChangesTypePreservingNullable()
        {
            Database.Tables.Add("ChangeDataTypeNullable", new Column<int>("ChangeDataTypeNullableId"));
            Database.Tables["ChangeDataTypeNullable"].Columns["ChangeDataTypeNullableId"].ChangeDataType<float>();

            Assert.AreEqual("real", Database.Tables["ChangeDataTypeNullable"].Columns["ChangeDataTypeNullableId"].DataType);
            Assert.IsFalse(Database.Tables["ChangeDataTypeNullable"].Columns["ChangeDataTypeNullableId"].Nullable);
        }

        [TestMethod]
        public override void ChangeDataTypeChangesTypePreservingNullable2()
        {
            Database.Tables.Add("ChangeDataTypeNullable2", new Column<int>("ChangeDataTypeNullable2Id"));
            Database.Tables["ChangeDataTypeNullable2"].Columns["ChangeDataTypeNullable2Id"].ChangeDataType<float>();

            Assert.AreEqual("real", Database.Tables["ChangeDataTypeNullable2"].Columns["ChangeDataTypeNullable2Id"].DataType);
            Assert.IsFalse(Database.Tables["ChangeDataTypeNullable2"].Columns["ChangeDataTypeNullable2Id"].Nullable);
        }

        [TestMethod]
        public override void DataTypeIncludesLengthOnVarchar()
        {
            Database.Tables.Add("VarcharLength", new Column("VarcharLengthId", "character varying(10)"));

            Assert.AreEqual("character varying(10)", Database.Tables["VarcharLength"].Columns["VarcharLengthId"].DataType);
        }

        [TestMethod]
        public override void DataTypeIncludesPrecisionOnDecimal()
        {
            Database.Tables.Add("DecimalPrecision", new Column("DecimalPrecisionId", "numeric(9)"));

            Assert.AreEqual("numeric(9)", Database.Tables["DecimalPrecision"].Columns["DecimalPrecisionId"].DataType);
        }

        [TestMethod]
        public override void DataTypeIncludesPrecisionAndScaleOnDecimalIfSpecified()
        {
            Database.Tables.Add("DecimalPrecisionAndScale", new Column("DecimalPrecisionAndScaleId", "decimal(8,2)"));

            Assert.AreEqual("numeric(8,2)", Database.Tables["DecimalPrecisionAndScale"].Columns["DecimalPrecisionAndScaleId"].DataType);
        }

        [TestMethod]
        public override void DataTypeDoesIncludeLengthOnChar()
        {
            Database.Tables.Add("SingleChar", new Column("SingleCharId", "char"));

            Assert.AreEqual("character(1)", Database.Tables["SingleChar"].Columns["SingleCharId"].DataType);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RenamePreservesAutoIncrementPrimaryKeyColumn()
        {
            base.RenamePreservesAutoIncrementPrimaryKeyColumn();
        }
    }
}
