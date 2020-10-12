using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class IndexCollectionMySqlTest : IndexCollectionTest
    {
        public IndexCollectionMySqlTest() : base(new MySqlDatabase(AssemblyInitialize.MySqlConnectionString))
        {            
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void AddAddsIndexWithIncludeColumn()
        {
            base.AddAddsIndexWithIncludeColumn();
        }
    }
}
