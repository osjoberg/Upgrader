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
        public override void DataTypeDoesNotIncludeLengthOnChar()
        {
            Database.Tables.Add("SingleChar", new Column("SingleCharId", "char"));

            Assert.AreEqual("character", Database.Tables["SingleChar"].Columns["SingleCharId"].DataType);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RenamePreservesAutoIncrementPrimaryKeyColumn()
        {
            base.RenamePreservesAutoIncrementPrimaryKeyColumn();
        }
    }
}
