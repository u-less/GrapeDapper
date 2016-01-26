using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrapeDapper.SqlAdapter;
using GrapeDapper.Core;
using GrapeDapper.SqlAdapter.Provider;

namespace GrapeDapper.SqlAdapter.Provider
{
    public class ProviderFactory
    {
        public static IProvider GetProvider(SqlType type)
        {
            switch (type)
            {
                case SqlType.SqlServer:return SingleInstance<SqlServerDatabaseProvider>.Instance;
                case SqlType.MySql:return SingleInstance<MySqlDatabaseProvider>.Instance;
                case SqlType.Npgsql:return SingleInstance<PostgreSQLDatabaseProvider>.Instance;
                default:return SingleInstance<SQLiteDatabaseProvider>.Instance;
            }
        }
    }
}
