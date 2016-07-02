using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class TableCollectionSqLiteTest : TableCollectionTest
    {
        public TableCollectionSqLiteTest() : base(new SqLiteDatabase("SqLite"))
        {
        }
    }
}
