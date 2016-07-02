using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class IndexCollectionSqLiteTest : IndexCollectionTest
    {
        public IndexCollectionSqLiteTest() : base(new SqLiteDatabase("SqLite"))
        {            
        }
    }
}
