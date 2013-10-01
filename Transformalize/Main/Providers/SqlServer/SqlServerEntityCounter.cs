#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Data.SqlClient;

namespace Transformalize.Main.Providers.SqlServer
{
    public class SqlServerEntityCounter : IEntityCounter
    {
        private readonly IConnectionChecker _connectionChecker;
        private readonly SqlServerEntityExists _entityExists;

        public SqlServerEntityCounter(IConnectionChecker connectionChecker = null)
        {
            _connectionChecker = connectionChecker ?? new DefaultConnectionChecker();
            _entityExists = new SqlServerEntityExists();
        }

        public int Count(AbstractConnection connection, string schema, string name)
        {
            if (_connectionChecker == null || _connectionChecker.Check(connection)) {
                if (_entityExists.Exists(connection, schema, name)) {
                    using (var cn = connection.GetConnection()) {
                        cn.Open();
                        var sql = string.Format("SELECT COUNT(*) FROM [{0}].[{1}] WITH (NOLOCK);", schema, name);
                        var cmd = cn.CreateCommand();
                        cmd.CommandText = sql;
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            return 0;
        }
    }
}