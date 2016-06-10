﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Upgrader.SqLite;

namespace Upgrader.Test.SqLite
{
    [TestClass]
    public class ColumnInfoSqLiteTest : ColumnInfoTest
    {
        public ColumnInfoSqLiteTest() : base(new SqLiteDatabase("Data Source=UpgraderTest.sqlite;Version=3;"))
        {            
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void ChangeTypeChangesTypePreservingNullability()
        {
            base.ChangeTypeChangesTypePreservingNullability();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void ChangeTypeChangesType()
        {
            base.ChangeTypeChangesType();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public override void RenameRenamesColumn()
        {
            base.RenameRenamesColumn();
        }
    }
}
