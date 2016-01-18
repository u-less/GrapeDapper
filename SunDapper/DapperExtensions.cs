﻿using System;
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
        #region Update
        public static int Update<T>(this DapperConnection connection, T data, List<string> columns = null, List<string> noColumns = null, IDbTransaction transaction = null)
        {
            return connection.Connection.Execute(BuildUpdateSql(connection, data, columns, noColumns), data, transaction);
        }
        public static async Task<int> UpdateAsync<T>(this DapperConnection connection, T data, List<string> columns = null, List<string> noColumns = null, IDbTransaction transaction = null)
        {
            return await connection.Connection.ExecuteAsync(BuildUpdateSql(connection, data, columns, noColumns), data,transaction);
        }
        private static string BuildUpdateSql<T>(DapperConnection connection, T data, List<string> columns = null, List<string> noColumns = null)
        {
            var tb = TableInfo.FromType(typeof(T));
            IProvider _provider = connection.SqlProvider;
            object primaryValue;
            var sb = new StringBuilder("UPDATE ");
            sb.Append(_provider.EscapeTableName(tb.TableName)).Append(" SET ");
            if (columns == null)
            {
                for (int i = 0; i < tb.Columns.Count; i++)
                {
                    var column = tb.Columns[i];
                    if (noColumns.Contains(column.Name)) continue;
                    if (column.Name.Equals(tb.PrimaryColumn.Name))
                    {
                        primaryValue = column.GetValue(data);
                        if (tb.AutoIncrement) continue;
                    }
                    if (column.IsResult) continue;
                    if (i > 0)
                        sb.Append(", ");
                    _provider.AppendColumnNameEqualsValue(sb,column.Name);
                }
            }
            else
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    if (i > 0)
                        sb.Append(", ");
                    _provider.AppendColumnNameEqualsValue(sb, column);
                }
            }
            sb.Append(" WHERE ");
            _provider.AppendColumnNameEqualsValue(sb, tb.PrimaryColumn.Name);
            return sb.ToString();
        }
        #endregion
        #region QuerySingle
        public static T Single<T>(this DapperConnection connection, object primaryKey)
        {
            var sqlPara = BuildSingleSql<T>(connection, primaryKey);
            return connection.Connection.QuerySingle<T>(sqlPara.Item1, sqlPara.Item2);
        }
        public static async Task<T> SingleAsync<T>(this DapperConnection connection, object primaryKey)
        {
            var sqlPara = BuildSingleSql<T>(connection, primaryKey);
            return await connection.Connection.QuerySingleAsync<T>(sqlPara.Item1, sqlPara.Item2);
        }
        public static T SingleOrDefault<T>(this DapperConnection connection, object primaryKey)
        {
            var sqlPara = BuildSingleSql<T>(connection, primaryKey);
            return connection.Connection.QuerySingleOrDefault<T>(sqlPara.Item1, sqlPara.Item2);
        }
        public static async Task<T> SingleOrDefaultAsync<T>(this DapperConnection connection, object primaryKey)
        {
            var sqlPara = BuildSingleSql<T>(connection, primaryKey);
            return await connection.Connection.QuerySingleOrDefaultAsync<T>(sqlPara.Item1, sqlPara.Item2);
        }
        private static Tuple<string, DynamicParameters> BuildSingleSql<T>(DapperConnection connection, object primaryKey)
        {
            var tb = TableInfo.FromType(typeof(T));
            IProvider provider = connection.SqlProvider;
            var sql = string.Format("SELECT * FROM {0} WHERE {1}", provider.EscapeTableName(tb.TableName), provider.GetColumnNameEqualsValue(tb.PrimaryColumn.Name));
            DynamicParameters paras = new DynamicParameters();
            paras.Add(tb.PrimaryColumn.Name, primaryKey);
            return Tuple.Create(sql, paras);
        }
        #endregion
        #region QueryPage
        public static Page<T> QueryPage<T>(this DapperConnection connection, long page, long pageSize, string sql, object param=null, CommandType? commandType = null)
        {
            var tb = TableInfo.FromType(typeof(T));
            SQLParts parts;
            if (!connection.SqlProvider.PagingUtility.SplitSQL(sql, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");
            string pageSql = connection.SqlProvider.BuildPageQuery((page - 1) * pageSize, pageSize, parts, ref param);
            string sqlCount = parts.SqlCount;
            var result = new Page<T>();
            var list= connection.Connection.Query<T>(pageSql,param);
            using (var multi = connection.Connection.QueryMultiple(pageSql + ";" + sqlCount, param))
            {
                result.Items = multi.Read<T>().ToList();
                result.TotalItems = multi.ReadSingle<long>();
                result.CurrentPage = page;
                result.PageSize = pageSize;
            }
            return result;
        }
        public static async Task<Page<T>> QueryPageAsync<T>(this DapperConnection connection, long page, long pageSize, string sql, object param=null, CommandType? commandType = null)
        {
            var tb = TableInfo.FromType(typeof(T));
            SQLParts parts;
            if (!connection.SqlProvider.PagingUtility.SplitSQL(sql, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");
            string pageSql = connection.SqlProvider.BuildPageQuery((page - 1) * pageSize, pageSize, parts, ref param);
            string sqlCount = parts.SqlCount;
            var result = new Page<T>();
            using (var multi =await connection.Connection.QueryMultipleAsync(pageSql + " " + sqlCount, param))
            {
                var items = await multi.ReadAsync<T>();
                result.Items =items.ToList();
                result.TotalItems =await multi.ReadSingleAsync<long>();
                result.CurrentPage = page;
            }
            return result;
        }
        #endregion
        #region Delete
        public static bool Delete<T>(this DapperConnection connection, object primaryKey, IDbTransaction transaction = null)
        {
            var sqlPara = BuildDeleteSql<T>(connection, primaryKey);
            return connection.Connection.Execute(sqlPara.Item1, sqlPara.Item2,transaction) > 0;
        }
        public static async Task<bool> DeleteAsync<T>(this DapperConnection connection, object primaryKey, IDbTransaction transaction = null)
        {
            var sqlPara = BuildDeleteSql<T>(connection, primaryKey);
            var count = await connection.Connection.ExecuteAsync(sqlPara.Item1, sqlPara.Item2,transaction);
            return count > 0;
        }
        private static Tuple<string, DynamicParameters> BuildDeleteSql<T>(DapperConnection connection, object primaryKey)
        {
            var tb = TableInfo.FromType(typeof(T));
            IProvider provider = connection.SqlProvider;
            var sql = string.Format("DELETE FROM {0} WHERE {1}", provider.EscapeTableName(tb.TableName), provider.GetColumnNameEqualsValue(tb.PrimaryColumn.Name));
            DynamicParameters paras = new DynamicParameters();
            paras.Add(tb.PrimaryColumn.Name, primaryKey);
            return Tuple.Create(sql, paras);
        }
        #endregion
        #region Insert
        public static object Insert<T>(this DapperConnection connection,T data, IDbTransaction transaction = null)
        {
            var tb = TableInfo.FromType(typeof(T));
            return connection.SqlProvider.Insert<T>(connection.Connection, tb, data, transaction);
        }
        public static async Task<object> InsertAsync<T>(this DapperConnection connection, T data, IDbTransaction transaction = null)
        {
            var tb = TableInfo.FromType(typeof(T));
            return await connection.SqlProvider.InsertAsync<T>(connection.Connection, tb, data, transaction);
        }
        #endregion Insert
    }
}