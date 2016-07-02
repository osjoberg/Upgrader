using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class IndexInfoSqLiteTest : IndexInfoTest
    {
        public IndexInfoSqLiteTest() : base(new SqLiteDatabase("SqLite"))
        {            
        }
    }
}
