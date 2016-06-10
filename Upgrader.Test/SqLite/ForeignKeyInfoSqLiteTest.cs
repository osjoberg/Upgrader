using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class ForeignKeyInfoSqLiteTest : ForeignKeyInfoTest
    {
        public ForeignKeyInfoSqLiteTest() : base(new SqLiteDatabase("Data Source=UpgraderTest.sqlite;Version=3;"))
        {            
        }
    }
}
