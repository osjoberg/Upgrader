﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class TableCollectionPostgreSqlTest : TableCollectionTest
    {
        public TableCollectionPostgreSqlTest() : base(new PostgreSqlDatabase("PostgreSql"))
        {
        }
    }
}
