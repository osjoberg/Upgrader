using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;
using Upgrader.Schema;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class TableCollectionPostgreSqlTest : TableCollectionTest
    {
        public TableCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {
        }

        [TestMethod]
        public override void AddWithAutoIncrementCreatesTableWithAutoIncrement()
        {
            Database.Tables.Add("AddAutoIncrementTable", new Column("AddAutoIncrementTableId", "integer", ColumnModifier.AutoIncrementPrimaryKey), new Column("Data", "int"));

            Database.Connection.Execute("INSERT INTO \"AddAutoIncrementTable\" (\"Data\") VALUES (12345)");

            Assert.AreEqual(1, Database.Connection.ExecuteScalar<int>("SELECT \"AddAutoIncrementTableId\" FROM \"AddAutoIncrementTable\""));
        }

    }
}
