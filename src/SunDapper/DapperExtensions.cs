using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Text;
using Dapper;
using SunDapper.Sql;
using SunDapper.Core;

namespace SunDapper
{
    public static class DapperExtensions
    {
        #region update
        public static int Update<T>(this DapperConnection connection, T data, List<string> columns = null, List<string> noColumns = null, IDbTransaction transaction = null)
        {
            return connection.Execute(GetUpdateSql(connection, data, columns, noColumns), data, transaction);
        }
        public static async Task<int> UpdateAsync<T>(this DapperConnection connection, T data, List<string> columns = null, List<string> noColumns = null, IDbTransaction transaction = null)
        {
            return await connection.ExecuteAsync(GetUpdateSql(connection, data, columns, noColumns), data,transaction);
        }
        private static string GetUpdateSql<T>(DapperConnection connection, T data, List<string> columns = null, List<string> noColumns = null)
        {
            var tb = TableInfo.FromType(typeof(T));
            IProvider _provider = connection.SqlProvider;
            string prefix = _provider.GetParameterPrefix(connection.ConnectionString);
            object primaryValue;
            var sb = new StringBuilder("UPDATE ");
            sb.Append(_provider.EscapeTableName(tb.TableName)).Append(" SET ");
            if (columns == null)
            {
                for (int i = 0; i < tb.Columns.Count; i++)
                {
                    var column = tb.Columns[i];
                    if (noColumns.Contains(column.Name)) continue;
                    if (column.Name.Equals(tb.PrimaryColumn))
                    {
                        primaryValue = column.GetValue(data);
                        if (tb.AutoIncrement) continue;
                    }
                    if (column.IsResult) continue;
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(_provider.EscapeSqlIdentifier(column.Name)).Append(" = ").Append(prefix).Append(column.Name);
                }
            }
            else
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(_provider.EscapeSqlIdentifier(column)).Append(" = ").Append(prefix).Append(column);
                }
            }
            sb.Append(" WHERE ").Append(_provider.EscapeSqlIdentifier(tb.PrimaryColumn.Name)).Append(" = ").Append(prefix).Append(tb.PrimaryColumn.Name);
            return sb.ToString();
        }
        #endregion
        #region QuerySingle
        public static T Single<T>(this DapperConnection connection, object primaryKey)
        {
            var sqlPara = GetSingleSql<T>(connection, primaryKey);
            return connection.QuerySingle<T>(sqlPara.Item1, sqlPara.Item2);
        }
        public static async Task<T> SingleAsync<T>(this DapperConnection connection, object primaryKey)
        {
            var sqlPara = GetSingleSql<T>(connection, primaryKey);
            return await connection.QuerySingleAsync<T>(sqlPara.Item1, sqlPara.Item2);
        }
        public static T SingleOrDefault<T>(this DapperConnection connection, object primaryKey)
        {
            var sqlPara = GetSingleSql<T>(connection, primaryKey);
            return connection.QuerySingleOrDefault<T>(sqlPara.Item1, sqlPara.Item2);
        }
        public static async Task<T> SingleOrDefaultAsync<T>(this DapperConnection connection, object primaryKey)
        {
            var sqlPara = GetSingleSql<T>(connection, primaryKey);
            return await connection.QuerySingleOrDefaultAsync<T>(sqlPara.Item1, sqlPara.Item2);
        }
        private static Tuple<string, DynamicParameters> GetSingleSql<T>(DapperConnection connection, object primaryKey)
        {
            var tb = TableInfo.FromType(typeof(T));
            IProvider provider = connection.SqlProvider;
            string prefix = provider.GetParameterPrefix(connection.ConnectionString);
            var sql = string.Format("SELECT TOP 1 * FROM {0} WHERE {1} = {2}{3}", provider.EscapeTableName(tb.TableName), provider.EscapeSqlIdentifier(tb.PrimaryColumn.Name), provider.GetParameterPrefix(connection.ConnectionString), tb.PrimaryColumn.Name);
            DynamicParameters paras = new DynamicParameters();
            paras.Add(tb.PrimaryColumn.Name, primaryKey);
            return Tuple.Create(sql, paras);
        }
        #endregion
        #region queryPage
        public static Page<T> QueryPage<T>(this DapperConnection connection, long page, long pageSize, string sql, object param)
        {
            var tb = TableInfo.FromType(typeof(T));
            SQLParts parts;
            if (!connection.SqlProvider.PagingUtility.SplitSQL(sql, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");
            string pageSql = connection.SqlProvider.BuildPageQuery((page - 1) * pageSize, pageSize, parts, ref param);
            string sqlCount = parts.SqlCount;
            var result = new Page<T>();
            using (var multi = connection.QueryMultiple(pageSql + " " + sqlCount, param))
            {
                result.Items = multi.Read<T>().ToList();
                result.TotalItems = multi.ReadSingle<long>();
                result.TotalPages = result.TotalItems / pageSize;
                result.CurrentPage = page;
            }
            return result;
        }
        public static async Task<Page<T>> QueryPageAsync<T>(this DapperConnection connection, long page, long pageSize, string sql, object param)
        {
            var tb = TableInfo.FromType(typeof(T));
            SQLParts parts;
            if (!connection.SqlProvider.PagingUtility.SplitSQL(sql, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");
            string pageSql = connection.SqlProvider.BuildPageQuery((page - 1) * pageSize, pageSize, parts, ref param);
            string sqlCount = parts.SqlCount;
            var result = new Page<T>();
            using (var multi =await connection.QueryMultipleAsync(pageSql + " " + sqlCount, param))
            {
                var items = await multi.ReadAsync<T>();
                result.Items =items.ToList();
                result.TotalItems =await multi.ReadSingleAsync<long>();
                result.TotalPages = result.TotalItems / pageSize;
                result.CurrentPage = page;
            }
            return result;
        }
        #endregion
        #region Delete
        public static bool Delete<T>(this DapperConnection connection, bool primaryKey, IDbTransaction transaction = null)
        {
            var sqlPara = GetDeleteSql<T>(connection, primaryKey);
            return connection.Execute(sqlPara.Item1, sqlPara.Item2,transaction) > 0;
        }
        public static async Task<bool> DeleteAsync<T>(this DapperConnection connection, bool primaryKey, IDbTransaction transaction = null)
        {
            var sqlPara = GetDeleteSql<T>(connection, primaryKey);
            var count = await connection.ExecuteAsync(sqlPara.Item1, sqlPara.Item2,transaction);
            return count > 0;
        }
        private static Tuple<string, DynamicParameters> GetDeleteSql<T>(DapperConnection connection, object primaryKey)
        {
            var tb = TableInfo.FromType(typeof(T));
            IProvider provider = connection.SqlProvider;
            string prefix = provider.GetParameterPrefix(connection.ConnectionString);
            var sql = string.Format("DELETE {0} WHERE {1} = {2}{3}", provider.EscapeTableName(tb.TableName), provider.EscapeSqlIdentifier(tb.PrimaryColumn.Name), provider.GetParameterPrefix(connection.ConnectionString), tb.PrimaryColumn.Name);
            DynamicParameters paras = new DynamicParameters();
            paras.Add(tb.PrimaryColumn.Name, primaryKey);
            return Tuple.Create(sql, paras);
        }
        #endregion
    }
}
