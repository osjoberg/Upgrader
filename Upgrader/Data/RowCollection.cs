using System.Collections.Generic;
using System.Linq;

namespace Upgrader.Data
{
    public class RowCollection
    {
        private readonly Database database;
        private readonly string tableName;

        internal RowCollection(Database database, string tableName)
        {
            this.database = database;
            this.tableName = tableName;
        }

        /// <summary>
        /// Insert row to the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="row">Row to insert into the table.</param>
        public void Add<T>(T row)
        {
            database.InsertRows(tableName, Enumerable.Repeat(row, 1));
        }

        /// <summary>
        /// Insert rows to the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="rows">Rows to insert into the table.</param>
        public void AddRange<T>(IEnumerable<T> rows)
        {
            database.InsertRows(tableName, rows);
        }

        /// <summary>
        /// Insert rows to the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="rows">Rows to insert into the table.</param>
        public void AddRange<T>(params T[] rows)
        {
            database.InsertRows(tableName, rows);
        }

        /// <summary>
        /// Update one row in the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="row">Row to update in the table.</param>
        public void Update<T>(T row)
        {
            database.UpdateRows(tableName, Enumerable.Repeat(row, 1));
        }

        /// <summary>
        /// Update rows in the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="rows">Rows to update in the table.</param>
        public void UpdateRange<T>(IEnumerable<T> rows)
        {
            database.UpdateRows(tableName, rows);
        }

        /// <summary>
        /// Update rows in the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="rows">Rows to update in the table.</param>
        public void UpdateRange<T>(params T[] rows)
        {
            database.UpdateRows(tableName, rows);
        }

        /// <summary>
        /// Delete one row in the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="row">Row to be deleted in the table.</param>
        public void Remove<T>(T row)
        {
            database.DeleteRows(tableName, Enumerable.Repeat(row, 1));
        }

        /// <summary>
        /// Delete rows in the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="rows">Rows to be deleted in the table.</param>
        public void RemoveRange<T>(IEnumerable<T> rows) 
        {
            database.DeleteRows(tableName, rows);
        }

        /// <summary>
        /// Delete rows in the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="rows">Rows to be deleted in the table.</param>
        public void RemoveRange<T>(params T[] rows)
        {
            database.DeleteRows(tableName, rows);
        }

        /// <summary>
        /// Delete rows in the table.
        /// </summary>
        /// <param name="where">Optional where criteria to filter specific rows.</param>
        public void Remove(string where = null)
        {
            database.DeleteRows(tableName, where);
        }

        /// <summary>
        /// Get rows from the table.
        /// </summary>
        /// <typeparam name="T">Type with properties matching the table column names.</typeparam>
        /// <param name="where">Optional where criteria to filter specific rows.</param>
        /// <returns>Rows fetched from the table.</returns>
        public IEnumerable<T> Query<T>(string where = null)
        {
            return database.Select<T>(tableName, where);
        }

        /// <summary>
        /// Get rows from the table.
        /// </summary>
        /// <param name="where">Optional where criteria to filter specific rows.</param>
        /// <returns>Rows fetched from the table.</returns>
        public IEnumerable<dynamic> Query(string where = null)
        {
            return database.Select(tableName, where);
        }
    }
}
