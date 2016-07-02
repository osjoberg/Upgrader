using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class IndexCollectionMySqlTest : IndexCollectionTest
    {
        public IndexCollectionMySqlTest() : base(new MySqlDatabase("MySql"))
        {            
        }
    }
}
