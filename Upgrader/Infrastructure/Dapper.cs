using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Dapper;

namespace Upgrader.Infrastructure
{
    internal class Dapper
    {
        private readonly IDbConnection connection;

        public Dapper(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void Execute(string sql, object parameters = null)
        {
            try
            {
                connection.Execute(sql, parameters);
            }
            catch (DbException exception)
            {
                throw UpgraderException.CannotExecuteStatement(sql, parameters, exception);
            }
        }

        public T ExecuteScalar<T>(string sql, object parameters = null)
        {
            try
            {
                return connection.ExecuteScalar<T>(sql, parameters);
            }
            catch (DbException exception)
            {
                throw UpgraderException.CannotExecuteStatement(sql, parameters, exception);
            }
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            try
            {
                return connection.Query<T>(sql, parameters);
            }
            catch (DbException exception)
            {
                throw UpgraderException.CannotExecuteStatement(sql, parameters, exception);
            }
        }
    }
}
