using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class UpgradeSqLiteTest : UpgradeTest<SqLiteDatabase>
    {
        public UpgradeSqLiteTest() : base(new SqLiteDatabase(AssemblyInitialize.SqLiteConnectionString), AssemblyInitialize.SqLiteConnectionString)
        {            
        }
    }
}
