using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using GrapeDapper.SqlAdapter;

namespace GrapeDapper
{
    public class DbFactory
    {
        public DbFactory(SqlType dbType,string connectionString)
        {
            switch (dbType)
            {
                case SqlType.Npgsql:Provider = PostgreSqlFactory(); break;
                case SqlType.MySql:Provider = MySqlFactory(); break;
                case SqlType.SQLite:Provider = SQLiteFactory(); break;
                default:Provider = SqlServerFactory(); break;
            }
            _connectionString = connectionString;
        }
        private string _connectionString;
        public DbProviderFactory Provider { get; private set; }

        public DapperConnection GetConnection()
        {
            var connection = Provider.CreateConnection();
            connection.ConnectionString = _connectionString;
            DapperConnection conn = new DapperConnection();
            conn.Base = connection;
            conn.SqlDbType = SqlType.Npgsql;
            conn.SqlProvider = SqlAdapter.Provider.ProviderFactory.GetProvider(conn.SqlDbType);
            return conn;
        }
        protected static DbProviderFactory PostgreSqlFactory()
        {
            return GetFactory("Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7"); 
        }
        protected static DbProviderFactory MySqlFactory()
        {
            return GetFactory("MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Culture=neutral, PublicKeyToken=c5687fc88969c44d");
        }
        protected static DbProviderFactory SqlServerFactory()
        {
            return GetFactory("System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        }
        public static DbProviderFactory SQLiteFactory()
        {
            return GetFactory("System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Culture=neutral, PublicKeyToken=db937bc2d44ff139");
        }
        protected static DbProviderFactory GetFactory(string assemblyQualifiedName)
        {
            var ft = Type.GetType(assemblyQualifiedName);
            return (DbProviderFactory)ft.GetField("Instance").GetValue(null);
        }
    }
}
