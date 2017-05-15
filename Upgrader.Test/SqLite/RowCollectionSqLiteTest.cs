using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class RowCollectionSqLiteTest : RowCollectionTest
    {
        public RowCollectionSqLiteTest() : base(new SqLiteDatabase("SqLite"))
        {
        }
    }
}
