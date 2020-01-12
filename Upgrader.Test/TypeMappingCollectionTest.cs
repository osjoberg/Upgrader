using System;
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
            TestAddType(Math.Round(1 / 3M, 5), default(decimal));
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
            TestAddType(float.MinValue + 1, float.MaxValue, default(float));
        }

        [TestMethod]
        public virtual void TestAddInt32Column()
        {
            TestAddType(int.MinValue, int.MaxValue, default(int));
        }

        [TestMethod]
        public virtual void TestAddInt64Column()
        {
            TestAddType(long.MinValue, long.MaxValue, default(long));
        }

        [TestMethod]
        public virtual void TestAddInt16Column()
        {
            TestAddType(short.MinValue, short.MaxValue, default(short));
        }

        [TestMethod]
        public virtual void TestAddStringColumnDefaultLength()
        {
            TestAddStringType(-1, false, new string('a', 50), new string(Enumerable.Range(10000, 50).Select(i => (char)i).ToArray()));
        }

        [TestMethod]
        public virtual void TestAddStringColumnSpecifiedLength()
        {
            TestAddStringType(10, false, new string('a', 10), new string(Enumerable.Range(10000, 10).Select(i => (char)i).ToArray()));
        }

        [TestMethod]
        public virtual void TestAddTimeSpanColumn()
        {
            TestAddType(TimeSpan.MinValue, TimeSpan.MaxValue, default(TimeSpan));
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
            TestAddType<decimal?>(null, Math.Round(1 / 3M, 5), default(decimal));
        }

        [TestMethod]
        public virtual void TestAddNullableDoubleColumn()
        {
            TestAddType<double?>(null, double.MinValue, double.MaxValue, default(double));
        }

        [TestMethod]
        public void TestAddNullableGuidColumn()
        {
            TestAddType<Guid?>(null, Guid.Empty, Guid.Parse("687f0cb6-5b31-4c97-a174-01c4b7ee26e2"));
        }

        [TestMethod]
        public void TestAddNullableFloatColumn()
        {
            TestAddType<float?>(null, float.MinValue, float.MaxValue, default(float));
        }

        [TestMethod]
        public virtual void TestAddNullableInt32Column()
        {
            TestAddType<int?>(null, int.MinValue, int.MaxValue, default(int));
        }

        [TestMethod]
        public void TestAddNullableInt64Column()
        {
            TestAddType<long?>(null, long.MinValue, long.MaxValue, default(long));
        }

        [TestMethod]
        public void TestAddNullableInt16Column()
        {
            TestAddType<short?>(null, short.MinValue, short.MaxValue, default(short));
        }

        [TestMethod]
        public void TestAddNullableStringColumnDefaultLength()
        {
            TestAddStringType(50, true, null, "", new string('a', 50), new string(Enumerable.Range(10000, 50).Select(i => (char)i).ToArray()));
        }

        [TestMethod]
        public void TestAddNullableStringColumnSpecifiedLength()
        {
            TestAddStringType(10, true, null, "", new string('a', 10), new string(Enumerable.Range(10000, 10).Select(i => (char)i).ToArray()));
        }

        [TestMethod]
        public virtual void TestAddNullableTimeSpanColumn()
        {
            TestAddType<TimeSpan?>(null, TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero);
        }

        protected void TestAddType<TType>(params TType[] values)
        {
            TestAddType(1, Nullable.GetUnderlyingType(typeof(TType)) != null, values);            
        }

        protected void TestAddType<TType>(int length, bool nullable, params TType[] values)
        {
            var tableName = GetTableName(typeof(TType), length, nullable);

            database.Tables.Add(tableName, new Column<int>("Id", ColumnModifier.PrimaryKey));

            foreach (var value in values)
            {
                var columnName = "Value" + (values.ToList().IndexOf(value) + 1);

                database.Tables[tableName].Columns.Add(columnName, value);

                var dataType = database.TypeMappings[Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType)];
                if (length == 1)
                {
                    Assert.AreEqual(dataType, database.Tables[tableName].Columns[columnName].GetDataType());
                }
                else
                {
                    Assert.AreEqual(dataType.Substring(0, dataType.IndexOf('(')) + $"({length})", database.Tables[tableName].Columns[columnName].GetDataType());
                }

                Assert.AreEqual(nullable, database.Tables[tableName].Columns[columnName].IsNullable());
            }
        }

        protected void TestAddStringType<TType>(int length, bool nullable, params TType[] values) where TType : IComparable<string>
        {
            var tableName = GetTableName(typeof(TType), length, nullable);

            database.Tables.Add(tableName, new Column<int>("Id", ColumnModifier.PrimaryKey));

            foreach (var value in values)
            {
                var columnName = "Value" + (values.ToList().IndexOf(value) + 1);

                if (length == -1 && nullable == false)
                {
                    database.Tables[tableName].Columns.Add<TType>(columnName);
                }
                else if (length == -1)
                {
                    database.Tables[tableName].Columns.Add(columnName, value);
                }
                else
                {
                    database.Tables[tableName].Columns.Add(columnName, length, nullable, value);
                }

                var dataType = database.TypeMappings[Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType)];
                if (length == -1)
                {
                    Assert.AreEqual(dataType, database.Tables[tableName].Columns[columnName].GetDataType());
                }
                else
                {
                    Assert.AreEqual(dataType.Substring(0, dataType.IndexOf('(')) + $"({length})", database.Tables[tableName].Columns[columnName].GetDataType());
                }

                Assert.AreEqual(nullable, database.Tables[tableName].Columns[columnName].IsNullable());
            }
        }

        private static string GetTableName(Type type, int length, bool nullable)
        {
            nullable = Nullable.GetUnderlyingType(type) != null || nullable;
            type = Nullable.GetUnderlyingType(type) ?? type;

            return "TypeMappingCollectionAdd" + (nullable ? "Nullable" : "") + type.Name + (length != 1 ? length.ToString() : "");
        }
    }
}
