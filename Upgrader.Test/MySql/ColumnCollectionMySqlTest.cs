using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.MySql;

namespace Upgrader.Test.MySql
{
    [TestClass]
    public class ColumnCollectionMySqlTest : ColumnCollectionTest
    {
        public ColumnCollectionMySqlTest() : base(new MySqlDatabase("MySql"))
        {            
        }
    }
}
