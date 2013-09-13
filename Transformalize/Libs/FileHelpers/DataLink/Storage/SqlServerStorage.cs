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

#if ! MINI
using System;
using System.Data;
using System.Data.SqlClient;
using Transformalize.Libs.FileHelpers.ErrorHandling;
using Transformalize.Libs.FileHelpers.Helpers;

namespace Transformalize.Libs.FileHelpers.DataLink.Storage
{
    /// <summary>
    ///     This is a base class that implements the <see cref="DataStorage" /> for Microsoft SqlServer.
    /// </summary>
    public sealed class SqlServerStorage : DatabaseStorage
    {
        #region "  Constructors  "

        /// <summary>Create a new instance of the SqlServerStorage based on the record type provided.</summary>
        /// <param name="recordType">The type of the record class.</param>
        public SqlServerStorage(Type recordType)
            : this(recordType, string.Empty)
        {
        }

        /// <summary>Create a new instance of the SqlServerStorage based on the record type provided.</summary>
        /// <param name="recordType">The type of the record class.</param>
        /// <param name="connectionStr">The full conection string used to connect to the sql server.</param>
        public SqlServerStorage(Type recordType, string connectionStr)
            : base(recordType)
        {
            ConnectionString = connectionStr;
        }

        /// <summary>Create a new instance of the SqlServerStorage based on the record type provided (uses windows auth)</summary>
        /// <param name="recordType">The type of the record class.</param>
        /// <param name="server">The server name or IP of the sqlserver</param>
        /// <param name="database">The database name into the server.</param>
        public SqlServerStorage(Type recordType, string server, string database)
            : this(recordType, server, database, string.Empty, string.Empty)
        {
        }

        /// <summary>Create a new instance of the SqlServerStorage based on the record type provided (uses SqlServer auth)</summary>
        /// <param name="recordType">The type of the record class.</param>
        /// <param name="server">The server name or IP of the sqlserver</param>
        /// <param name="database">The database name into the server.</param>
        /// <param name="user">The sql username to login into the server.</param>
        /// <param name="pass">The pass of the sql username to login into the server.</param>
        public SqlServerStorage(Type recordType, string server, string database, string user, string pass)
            : this(recordType, DataBaseHelper.SqlConnectionString(server, database, user, pass))
        {
            mServerName = server;
            mDatabaseName = database;
            mUserName = user;
            mUserPass = pass;
        }

        #endregion

        #region "  Create Connection and Command  "

        /// <summary>Must create an abstract connection object.</summary>
        /// <returns>An Abstract Connection Object.</returns>
        protected override sealed IDbConnection CreateConnection()
        {
            string conString;
            if (ConnectionString == string.Empty)
            {
                if (mServerName == null || mServerName == string.Empty)
                    throw new BadUsageException("The ServerName can�t be null or empty.");

                if (mDatabaseName == null || mDatabaseName == string.Empty)
                    throw new BadUsageException("The DatabaseName can�t be null or empty.");

                conString = DataBaseHelper.SqlConnectionString(ServerName, DatabaseName, UserName, UserPass);
            }
            else
            {
                conString = ConnectionString;
            }

            return new SqlConnection(conString);
        }

        #endregion

        #region "  ServerName  "

        private string mServerName = string.Empty;

        /// <summary> The server name or IP of the SqlServer </summary>
        public string ServerName
        {
            get { return mServerName; }
            set
            {
                mServerName = value;
                ConnectionString = DataBaseHelper.SqlConnectionString(ServerName, DatabaseName, UserName, UserPass);
            }
        }

        #endregion

        #region "  DatabaseName  "

        private string mDatabaseName = string.Empty;

        /// <summary> The name of the database. </summary>
        public string DatabaseName
        {
            get { return mDatabaseName; }
            set
            {
                mDatabaseName = value;
                ConnectionString = DataBaseHelper.SqlConnectionString(ServerName, DatabaseName, UserName, UserPass);
            }
        }

        #endregion

        #region "  UserName  "

        private string mUserName = string.Empty;

        /// <summary> The user name used to logon into the SqlServer. (leave empty for WindowsAuth)</summary>
        public string UserName
        {
            get { return mUserName; }
            set
            {
                mUserName = value;
                ConnectionString = DataBaseHelper.SqlConnectionString(ServerName, DatabaseName, UserName, UserPass);
            }
        }

        #endregion

        #region "  UserPass  "

        private string mUserPass = string.Empty;

        /// <summary> The user pass used to logon into the SqlServer. (leave empty for WindowsAuth)</summary>
        public string UserPass
        {
            get { return mUserPass; }
            set
            {
                mUserPass = value;
                ConnectionString = DataBaseHelper.SqlConnectionString(ServerName, DatabaseName, UserName, UserPass);
            }
        }

        #endregion

        #region "  ExecuteInBatch  "

        /// <summary></summary>
        protected override bool ExecuteInBatch
        {
            get { return true; }
        }

        #endregion
    }
}

#endif