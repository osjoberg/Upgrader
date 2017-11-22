using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Upgrader.Schema;

namespace Upgrader.Test
{
    [TestClass]
    public abstract class TypeMappingCollectionTest
    {
        private readonly Database database;

        protected TypeMappingCollectionTest(Database database)
        {
            this.database = database;
        }

        [TestMethod]
        public virtual void TestAddBooleanColumn()
        {
            TestAddType(false, true);
        }

        [TestMethod]
        public virtual void TestAddByteColumn()
        {
            TestAddType(byte.MinValue, byte.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddCharColumn()
        {
            TestAddType(char.MinValue, char.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddDateTimeColumn()
        {
            TestAddType(DateTime.MinValue, DateTime.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddDecimalColumn()
        {
            TestAddType(Math.Round(1 / 3M, 5));
        }

        [TestMethod]
        public virtual void TestAddDoubleColumn()
        {
            TestAddType(double.MinValue, double.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddGuidColumn()
        {
            TestAddType(Guid.Empty, Guid.Parse("687f0cb6-5b31-4c97-a174-01c4b7ee26e2"));
        }

        [TestMethod]
        public virtual void TestAddFloatColumn()
        {
            TestAddType(float.MinValue, float.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddInt32Column()
        {
            TestAddType(int.MinValue, int.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddInt64Column()
        {
            TestAddType(long.MinValue, long.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddInt16Column()
        {
            TestAddType(short.MinValue, short.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddStringColumnDefaultLength()
        {
            TestAddType("", new string('a', 50), new string(Enumerable.Range(10000, 50).Select(i => (char)i).ToArray()));
        }

        [TestMethod]
        public virtual void TestAddStringColumnSpecifiedLength()
        {
            TestAddType(10, false, new string('a', 10), new string(Enumerable.Range(10000, 10).Select(i => (char)i).ToArray()));
        }

        [TestMethod]
        public virtual void TestAddTimeSpanColumn()
        {
            TestAddType(TimeSpan.MinValue, TimeSpan.MaxValue);
        }

        [TestMethod]
        public void TestAddNullableBooleanColumn()
        {
            TestAddType<bool?>(null, false, true);
        }

        [TestMethod]
        public virtual void TestAddNullableByteColumn()
        {
            TestAddType<byte?>(null, byte.MinValue, byte.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddNullableCharColumn()
        {
            TestAddType<char?>(null, char.MinValue, char.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddNullableDateTimeColumn()
        {
            TestAddType<DateTime?>(null, DateTime.MinValue, DateTime.MaxValue);
        }

        [TestMethod]
        public void TestAddNullableDecimalColumn()
        {
            TestAddType<decimal?>(null, Math.Round(1 / 3M, 5));
        }

        [TestMethod]
        public virtual void TestAddNullableDoubleColumn()
        {
            TestAddType<double?>(null, double.MinValue, double.MaxValue);
        }

        [TestMethod]
        public void TestAddNullableGuidColumn()
        {
            TestAddType<Guid?>(null, Guid.Empty, Guid.Parse("687f0cb6-5b31-4c97-a174-01c4b7ee26e2"));
        }

        [TestMethod]
        public void TestAddNullableFloatColumn()
        {
            TestAddType<float?>(null, float.MinValue, float.MaxValue);
        }

        [TestMethod]
        public virtual void TestAddNullableInt32Column()
        {
            TestAddType<int?>(null, int.MinValue, int.MaxValue);
        }

        [TestMethod]
        public void TestAddNullableInt64Column()
        {
            TestAddType<long?>(null, long.MinValue, long.MaxValue);
        }

        [TestMethod]
        public void TestAddNullableInt16Column()
        {
            TestAddType<short?>(null, short.MinValue, short.MaxValue);
        }

        [TestMethod]
        public void TestAddNullableStringColumnDefaultLength()
        {
            TestAddType(50, true, null, "", new string('a', 50), new string(Enumerable.Range(10000, 50).Select(i => (char)i).ToArray()));
        }

        [TestMethod]
        public void TestAddNullableStringColumnSpecifiedLength()
        {
            TestAddType(10, true, null, "", new string('a', 10), new string(Enumerable.Range(10000, 10).Select(i => (char)i).ToArray()));
        }

        [TestMethod]
        public virtual void TestAddNullableTimeSpanColumn()
        {
            TestAddType<TimeSpan?>(null, TimeSpan.MinValue, TimeSpan.MaxValue);
        }

        protected void TestAddType<TType>(params TType[] values)
        {
            TestAddType(1, Nullable.GetUnderlyingType(typeof(TType)) != null, values);            
        }

        protected void TestAddType<TType>(int length, bool nullable, params TType[] values)
        {
            var tableName = GetTableName(typeof(TType), length, nullable);

            database.Tables.Add(tableName, new Column("Id", "integer", ColumnModifier.PrimaryKey));

            if (length == 1)
            {
                database.Tables[tableName].Columns.Add<TType>("Value");
            }
            else
            {
                database.Tables[tableName].Columns.Add<TType>("Value", length, nullable);
            }

            database.Tables[tableName].Rows.Add(new { Id = 1, Value = values.First() });

            foreach (var value in values)
            {
                database.Tables[tableName].Rows.Update(new { Id = 1, Value = value });
                Assert.AreEqual(value, database.Tables[tableName].Rows.Query<KeyValuePair<int, TType>>().Single().Value);
            }

            var dataType = database.TypeMappings[Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType)];
            if (length == 1)
            {
                Assert.AreEqual(dataType, database.Tables[tableName].Columns["Value"].DataType);
            }
            else
            {
                Assert.AreEqual(dataType.Substring(0, dataType.IndexOf('(')) + $"({length})", database.Tables[tableName].Columns["Value"].DataType);
            }

            Assert.AreEqual(nullable, database.Tables[tableName].Columns["Value"].Nullable);
        }

        private static string GetTableName(Type type, int length, bool nullable)
        {
            nullable = Nullable.GetUnderlyingType(type) != null || nullable;
            type = Nullable.GetUnderlyingType(type) ?? type;

            return "TypeMappingCollectionAdd" + (nullable ? "Nullable" : "") + type.Name + (length != 1 ? length.ToString() : "");
        }
    }
}
