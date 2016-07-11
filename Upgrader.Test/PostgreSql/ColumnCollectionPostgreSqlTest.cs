﻿using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;
using Upgrader.Schema;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class ColumnCollectionPostgreSqlTest : ColumnCollectionTest
    {
        public ColumnCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {            
        }

        [TestMethod]
        public override void CanAddNonNullColumn()
        {
            Database.Tables.Add("CanAddNotNullColumn", new Column("CanAddNotNullColumnId", "int"));
            Database.Connection.Execute("INSERT INTO \"CanAddNotNullColumn\" VALUES (1)");
            Database.Tables["CanAddNotNullColumn"].Columns.Add("NewNotNullColumn", "int", 5);

            Assert.AreEqual(ColumnModifier.None, Database.Tables["CanAddNotNullColumn"].Columns["NewNotNullColumn"].Modifier);
        }
    }
}
