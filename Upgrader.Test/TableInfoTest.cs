using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class TableInfoTest
    {
        protected TableInfoTest(Database database)
        {
            Database = database;
        }

        protected Database Database { get; }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Dispose();
        }

        [TestMethod]
        public virtual void AddPrimaryKeyAddsPrimaryKey()
        {
            Database.Tables.Add("AddPrimaryKey", new Column("AddPrimaryKeyId", "int"));
            Database.Tables["AddPrimaryKey"].AddPrimaryKey("AddPrimaryKeyId");
            
            Assert.AreEqual("AddPrimaryKeyId", Database.Tables["AddPrimaryKey"].PrimaryKey.ColumnNames.Single());
        }

        [TestMethod]
        public virtual void AddPrimaryKeyWithMultipleColumnsAddsPrimaryKeyWithMultipleColums()
        {
            Database.Tables.Add("AddPrimaryKeyMultiple", new Column("AddPrimaryKeyMultipleId", "int"), new Column("Multiple", "int"));
            Database.Tables["AddPrimaryKeyMultiple"].AddPrimaryKey(new[] { "AddPrimaryKeyMultipleId", "Multiple" });

            CollectionAssert.AreEqual(new[] { "AddPrimaryKeyMultipleId", "Multiple" }, Database.Tables["AddPrimaryKeyMultiple"].PrimaryKey.ColumnNames);
        }

        [TestMethod]
        public void PrimaryKeyPropertyIsNullWhenNoPrimaryKeyIsPresent()
        {
            Database.Tables.Add("GetPrimaryKeyColumns", new Column("GetPrimaryKeyColumnsId", "int"));

            Assert.IsNull(Database.Tables["GetPrimaryKeyColumns"].PrimaryKey);
        }

        [TestMethod]
        public virtual void RemovePrimaryKeyRemovesPrimaryKey()
        {
            Database.Tables.Add("RemovePrimaryKey", new Column("RemovePrimaryKeyId", "int"));
            Database.Tables["RemovePrimaryKey"].AddPrimaryKey("RemovePrimaryKeyId");
            Database.Tables["RemovePrimaryKey"].RemovePrimaryKey();

            Assert.IsNull(Database.Tables["RemovePrimaryKey"].PrimaryKey);
        }

        [TestMethod]
        public void RenameRenamesTable()
        {
            Database.Tables.Add("RenameTable", new Column("RenameTableId", "int"));

            Database.Tables["RenameTable"].Rename("NewTableName");

            Assert.IsNotNull(Database.Tables["NewTableName"]);
        }
    }
}
