using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Text;
using SunDapper.Core;

namespace SunDapper.Sql
{
    public interface IProvider
    {
        IPagingHelper PagingUtility { get; }
        string EscapeTableName(string tableName);
        string GetColumnName(string columnName);
        void AppendColumnName(StringBuilder sb, string columnName);
        void AppendColumnNameEqualsValue(StringBuilder sb, string columnName);
        string GetColumnNameEqualsValue(string columnName);
        string BuildPageQuery(long skip, long take, SQLParts parts, ref object param);
        object Insert<T>(IDbConnection connection, TableInfo tableInfo, T data, Type tType, IDbTransaction transaction = null);
        Task<object> InsertAsync<T>(IDbConnection connection, TableInfo tableInfo, T data, Type tType, IDbTransaction transaction = null);
        string GetExistsSql(string table, string whereSql);
    }
}
