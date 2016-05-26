﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqlServer;

namespace Upgrader.Test.SqlServer
{
    [TestClass]
    public class ForeignKeyInfoSqlServerTest : ForeignKeyInfoTest
    {
        public ForeignKeyInfoSqlServerTest() : base(new SqlServerDatabase("Server=(local);Integrated Security=true;Initial Catalog=UpgraderTest"))
        {            
        }
    }
}
