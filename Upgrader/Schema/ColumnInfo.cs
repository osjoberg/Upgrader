using System;

using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <summary>
    /// Column information and modification.
    /// </summary>
    public class ColumnInfo
    {
        private readonly Database database;

        internal ColumnInfo(Database database, string tableName, string columnColumnName)
        {            
            this.database = database;
            TableName = tableName;
            ColumnName = columnColumnName;
        }

        /// <summary>
        /// Gets column name.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets column SQL data type.
        /// </summary>
        /// <returns>Column SQL data type.</returns>
        public string GetDataType() => database.GetColumnDataType(TableName, ColumnName);

        /// <summary>
        /// Gets if column is nullable.
        /// </summary>
        /// <returns>True if column is nullable, otherwise False.</returns>
        public bool IsNullable() => database.GetColumnNullable(TableName, ColumnName);

        /// <summary>
        /// Gets if column is configured to auto increment.
        /// </summary>
        /// <returns>True if the column is auto incremented, otherwise False.</returns>
        public bool IsAutoIncrement() => database.GetColumnAutoIncrement(TableName, ColumnName);

        /// <summary>
        /// Change data type of column.
        /// </summary>
        /// <param name="dataType">SQL data type.</param>
        public void ChangeDataType(string dataType)
        {
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            database.ChangeColumn(TableName, ColumnName, dataType, IsNullable());
        }

        /// <summary>
        /// Change data type of column.
        /// </summary>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        public void ChangeDataType(string dataType, bool nullable)
        {
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            database.ChangeColumn(TableName, ColumnName, dataType, nullable);
        }

        /// <summary>
        /// Change data type of column.
        /// </summary>
        /// <typeparam name="TType">CLR data typed to translate to SQL data type.</typeparam>
        public void ChangeDataType<TType>()
        {
            var dataType = database.TypeMappings.GetDataType(typeof(TType));
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            var nullable = Nullable.GetUnderlyingType(typeof(TType)) != null;

            ChangeDataType(dataType, nullable);
        }

        /// <summary>
        /// Change data type of column.
        /// </summary>
        /// <param name="length">Length of data type.</param>
        /// <param name="nullable">True if columns is allowed to be null.</param>
        /// <typeparam name="TType">CLR data typed to translate to SQL data type.</typeparam>
        public void ChangeDataType<TType>(int length, bool nullable = false)
        {
            var dataType = database.TypeMappings.GetDataType(typeof(TType), length);
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            ChangeDataType(dataType, nullable);
        }

        /// <summary>
        /// Change data type of column.
        /// </summary>
        /// <param name="scale">Scale of data type.</param>
        /// <param name="precision">Precision of data type.</param>
        /// <typeparam name="TType">CLR data typed to translate to SQL data type.</typeparam>
        public void ChangeDataType<TType>(int scale, int precision)
        {
            var dataType = database.TypeMappings.GetDataType(typeof(TType), scale, precision);
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            var nullable = Nullable.GetUnderlyingType(typeof(TType)) != null;

            ChangeDataType(dataType, nullable);
        }
    }
}
