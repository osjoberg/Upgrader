using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrader.Infrastructure;

namespace Upgrader.Schema
{
    /// <inheritdoc />
    /// <summary>
    /// Collection of all columns in the specified table.
    /// </summary>
    public class ColumnCollection : IEnumerable<ColumnInfo>
    {
        private readonly Database database;
        private readonly string tableName;

        internal ColumnCollection(Database database, string tableName)
        {
            this.database = database;
            this.tableName = tableName;
        }

        /// <summary>
        /// Gets column information for the specified column. Returns null if the specified column name does not exist.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Column information.</returns>
        public ColumnInfo this[string columnName]
        {
            get
            {
                Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
                Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);

                return new ColumnInfo(database, tableName, columnName);
            }
        }

        public IEnumerator<ColumnInfo> GetEnumerator()
        {
            return database
                .GetColumnNames(tableName)
                .Select(columnName => new ColumnInfo(database, tableName, columnName))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Checks if a column exists in the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>True, if the column exists or False if it does not exist.</returns>
        public bool Exists(string columnName)
        {
            return database.GetColumnDataType(tableName, columnName) != null;
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="nullable">True to allow null values.</param>
        /// <param name="initialValue">Initial value to set to all existing rows.</param>
        public void Add(string columnName, string dataType, bool nullable, object initialValue)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));

            // Add column.
            if (nullable == false && initialValue != null)
            {
                database.AddColumn(tableName, columnName, dataType, true);
            }
            else
            {
                database.AddColumn(tableName, columnName, dataType, nullable);
            }

            // Set value.
            if (initialValue != null)
            {
                database.SetColumnValue(tableName, columnName, initialValue);
            }

            // Change nullable.
            if (nullable == false && initialValue != null)
            {
                database.ChangeColumn(tableName, columnName, dataType, false);
            }
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="nullable">True to allow null values.</param>
        public void Add(string columnName, string dataType, bool nullable)
        {
            Add(columnName, dataType, nullable, null);
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="dataType">SQL data type.</param>
        public void Add(string columnName, string dataType)
        {
            Add(columnName, dataType, false, null);
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void Add<TType>(string columnName)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));

            var dataType = database.TypeMappings.GetDataType(typeof(TType));
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            var nullable = Nullable.GetUnderlyingType(typeof(TType)) != null;
            var initialValue = typeof(TType) == typeof(string) ? "" : (object)default(TType);

            Add(columnName, dataType, nullable, initialValue);
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="initialValue">Initial value to set to all existing rows.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void Add<TType>(string columnName, TType initialValue)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));

            var dataType = database.TypeMappings.GetDataType(typeof(TType));
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            var nullable = Nullable.GetUnderlyingType(typeof(TType)) != null;

            Add(columnName, dataType, nullable, initialValue);
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="length">Length of string data type.</param>
        /// <param name="nullable">True to allow null values.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void Add<TType>(string columnName, int length, bool nullable)
        {
            Validate.IsNotNullable(typeof(TType), nameof(TType));
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsEqualOrGreaterThan(length, 1, nameof(length));

            var dataType = database.TypeMappings.GetDataType(typeof(TType), length);
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            object initialValue;
            if (typeof(TType) == typeof(string) && nullable)
            {
                initialValue = null;
            }
            else if (typeof(TType) == typeof(string) && nullable == false)
            {
                initialValue = "";
            }
            else
            {
                initialValue = default(TType);
            }
                
            Add(columnName, dataType, nullable, initialValue);
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="length">Length of string data type.</param>
        /// <param name="nullable">True to allow null values.</param>
        /// <param name="initialValue">Initial value to set to all existing rows.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void Add<TType>(string columnName, int length, bool nullable, TType initialValue)
        {
            Validate.IsNotNullable(typeof(TType), nameof(TType));
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsEqualOrGreaterThan(length, 1, nameof(length));

            var dataType = database.TypeMappings.GetDataType(typeof(TType), length);
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            Add(columnName, dataType, nullable, initialValue);
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="scale">Scale of numeric data type.</param>
        /// <param name="precision">Precision of numeric data type.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void Add<TType>(string columnName, int scale, int precision)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsEqualOrGreaterThan(scale, 1, nameof(scale));
            Validate.IsEqualOrGreaterThan(precision, 0, nameof(precision));

            var dataType = database.TypeMappings.GetDataType(typeof(TType), scale, precision);
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            var nullable = Nullable.GetUnderlyingType(typeof(TType)) != null;
            var initialValue = typeof(TType) == typeof(string) ? "" : (object)default(TType);

            Add(columnName, dataType, nullable, initialValue);
        }

        /// <summary>
        /// Adds a column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="scale">Scale of numeric data type.</param>
        /// <param name="precision">Precision of numeric data type.</param>
        /// <param name="initialValue">Initial value to set to all existing rows.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void Add<TType>(string columnName, int scale, int precision, TType initialValue)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsEqualOrGreaterThan(scale, 1, nameof(scale));
            Validate.IsEqualOrGreaterThan(precision, 0, nameof(precision));

            var dataType = database.TypeMappings.GetDataType(typeof(TType), scale, precision);
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            var nullable = Nullable.GetUnderlyingType(typeof(TType)) != null;

            Add(columnName, dataType, nullable, initialValue);
        }

        /// <summary>
        /// Adds a computed column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="dataType">SQL data type.</param>
        /// <param name="nullable">True to allow null values.</param>
        /// <param name="expression">SQL expression used to compute computed value.</param>
        /// <param name="persisted">True to store the computed value in the table.</param>
        public void AddComputed(string columnName, string dataType, bool nullable, string expression, bool persisted = false)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(dataType, nameof(dataType));
            Validate.IsNotNullAndNotEmpty(expression, nameof(expression));

            database.AddComputedColumn(tableName, columnName, dataType, nullable, expression, persisted);
        }

        /// <summary>
        /// Adds a computed column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="expression">SQL expression used to compute computed value.</param>
        /// <param name="persisted">True to store the computed value in the table.</param>
        public void AddComputed(string columnName, string expression, bool persisted = false)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(expression, nameof(expression));

            database.AddComputedColumn(tableName, columnName, null, false, expression, persisted);
        }

        /// <summary>
        /// Adds a computed column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="expression">SQL expression used to compute computed value.</param>
        /// <param name="persisted">True to store the computed value in the table.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void AddComputed<TType>(string columnName, string expression, bool persisted = false)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsNotNullAndNotEmpty(expression, nameof(expression));

            var dataType = database.TypeMappings.GetDataType(typeof(TType));
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            var nullable = Nullable.GetUnderlyingType(typeof(TType)) != null;

            AddComputed(columnName, dataType, nullable, expression, persisted);
        }

        /// <summary>
        /// Adds a computed column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="length">Length of string data type.</param>
        /// <param name="nullable">True to allow null values.</param>
        /// <param name="expression">SQL expression used to compute computed value.</param>
        /// <param name="persisted">True to store the computed value in the table.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void AddComputed<TType>(string columnName, int length, bool nullable, string expression, bool persisted = false)
        {
            Validate.IsNotNullable(typeof(TType), nameof(TType));
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsEqualOrGreaterThan(length, 1, nameof(length));
            Validate.IsNotNullAndNotEmpty(expression, nameof(expression));

            var dataType = database.TypeMappings.GetDataType(typeof(TType), length);
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            AddComputed(columnName, dataType, nullable, expression, persisted);
        }

        /// <summary>
        /// Adds a computed column to the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="scale">Scale of numeric data type.</param>
        /// <param name="precision">Precision of numeric data type.</param>
        /// <param name="expression">SQL expression used to compute computed value.</param>
        /// <param name="persisted">True to store the computed value in the table.</param>
        /// <typeparam name="TType">CLR data typed to resolve SQL data type from.</typeparam>
        public void AddComputed<TType>(string columnName, int scale, int precision, string expression, bool persisted = false)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.IsEqualOrGreaterThan(scale, 1, nameof(scale));
            Validate.IsEqualOrGreaterThan(precision, 0, nameof(precision));
            Validate.IsNotNullAndNotEmpty(expression, nameof(expression));

            var dataType = database.TypeMappings.GetDataType(typeof(TType), scale, precision);
            if (dataType == null)
            {
                var type = Nullable.GetUnderlyingType(typeof(TType)) ?? typeof(TType);
                throw new ArgumentException($"No type mapping could be found for type \"{type.FullName}\".", nameof(TType));
            }

            var nullable = Nullable.GetUnderlyingType(typeof(TType)) != null;

            AddComputed(columnName, dataType, nullable, expression, persisted);
        }

        /// <summary>
        /// Rename a column in the table.
        /// </summary>
        /// <param name="currentColumnName">Current name of column.</param>
        /// <param name="newColumnName">New name of column.</param>
        public void Rename(string currentColumnName, string newColumnName)
        {
            Validate.IsNotNullAndNotEmpty(currentColumnName, nameof(currentColumnName));
            Validate.MaxLength(currentColumnName, nameof(currentColumnName), database.MaxIdentifierLength);
            Validate.IsNotNullAndNotEmpty(newColumnName, nameof(newColumnName));
            Validate.MaxLength(newColumnName, nameof(newColumnName), database.MaxIdentifierLength);

            database.RenameColumn(tableName, currentColumnName, newColumnName);
        }

        /// <summary>
        /// Removes a column from the table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        public void Remove(string columnName)
        {
            Validate.IsNotNullAndNotEmpty(columnName, nameof(columnName));
            Validate.MaxLength(columnName, nameof(columnName), database.MaxIdentifierLength);

            database.RemoveColumn(tableName, columnName);
        }
    }
}
