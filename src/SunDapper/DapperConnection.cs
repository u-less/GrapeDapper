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
    public abstract class DapperConnection:DbConnection
    {
        public SqlType SqlDbType { get; set; }
        public IProvider SqlProvider { get; set; }
    }
}
