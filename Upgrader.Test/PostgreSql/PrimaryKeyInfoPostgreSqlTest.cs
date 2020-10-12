﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.PostgreSql;

namespace Upgrader.Test.PostgreSql
{
    [TestClass]
    public class PrimaryKeyInfoPostgreSqlTest : PrimaryKeyInfoTest
    {
        public PrimaryKeyInfoPostgreSqlTest() : base(new PostgreSqlDatabase(AssemblyInitialize.PostgreSqlConnectionString))
        {
        }
    }
}
