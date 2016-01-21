using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using SunDapper.Core;
using SunDapper.Sql;

namespace SunDapper
{
    public class DapperConnection:IDisposable
    {
        public DbConnection Base { get; set; }
        public SqlType SqlDbType { get; set; }
        public IProvider SqlProvider { get; set; }

        public static implicit operator DbConnection(DapperConnection connection)
        {
            return connection.Base;
        }
        public void Dispose()
        {
            Base.Dispose();
        }
    }
}
