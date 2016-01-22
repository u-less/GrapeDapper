using System;
using System.Data;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using System.Text;
using SunDapper.Core;
using SunDapper.Sql;

namespace SunDapper.Sql.Provider
{
    public abstract class DatabaseProvider : IProvider
    {

        public virtual IPagingHelper PagingUtility
        {
            get { return PagingHelper.Instance; }
        }
        private ConcurrentDictionary<string, string> insertSqlDict = new ConcurrentDictionary<string, string>();
        public string GetInsertSqlFromCache(string tableName, Func<string> getMethod)
        {
            string sql;
            if (!insertSqlDict.TryGetValue(tableName, out sql))
            {
                sql = getMethod();
                insertSqlDict.TryAdd(tableName, sql);
            }
            return sql;
        }
        public virtual string EscapeTableName(string tableName)
        {
            return tableName.IndexOf('.') >= 0 ? tableName :GetColumnName(tableName);
        }

        public virtual string GetColumnName(string columnName)
        {
            return string.Format("[{0}]", columnName);
        }
        public virtual void AppendColumnName(StringBuilder sb, string columnName)
        {
            sb.AppendFormat("[{0}]", columnName);
        }
        public virtual void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
        {
            sb.AppendFormat("[{0}] = @{1}", columnName, columnName);
        }
        public virtual string GetColumnNameEqualsValue(string columnName)
        {
            return string.Format("[{0}] = @{1}", columnName, columnName);
        }
        public virtual string BuildPageQuery(long skip, long take, SQLParts parts,ref object param)
        {
            var sql = string.Format("{0}\nLIMIT @take OFFSET @skip", parts.Sql);
            DynamicParameters newParam = new DynamicParameters(param);
            newParam.Add("skip", skip);
            newParam.Add("take",take);
            param = newParam;
            return sql;
        }
        protected Tuple<string,string> GetInsertSqlParts(TableInfo tableInfo)
        {
            var names = new List<string>();
            var values = new List<string>();
            for (int i = 0; i < tableInfo.Columns.Count; i++)
            {
                var column = tableInfo.Columns[i];
                if (column.Name.Equals(tableInfo.PrimaryColumn.Name))
                {
                    if (tableInfo.AutoIncrement) continue;
                }
                if (column.IsResult) continue;
                names.Add(GetColumnName(column.Name));
                values.Add(string.Format("@{0}",column.Name));
            }
            return Tuple.Create(string.Join(",", names.ToArray()), string.Join(",", values.ToArray()));
        }
        public virtual object Insert<T>(IDbConnection connection, TableInfo tableInfo, T data,Type tType, IDbTransaction transaction = null)
        {
            return null;
        }
        public virtual async Task<object> InsertAsync<T>(IDbConnection connection, TableInfo tableInfo, T data, Type tType, IDbTransaction transaction = null)
        {
            return await Task.FromResult<object>(null);
        }
        public virtual string GetExistsSql(string table,string whereSql)
        {
            return string.Format("select 1 FROM {0} WHERE {1} LIMIT 1",table,whereSql);
        }
    }
}