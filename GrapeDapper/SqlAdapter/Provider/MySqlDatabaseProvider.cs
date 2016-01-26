using System;
using System.Data.Common;
using GrapeDapper.SqlAdapter;
using System.Threading.Tasks;
using System.Text;
using GrapeDapper.Core;
using System.Data;
using Dapper;

namespace GrapeDapper.SqlAdapter.Provider
{
    public class MySqlDatabaseProvider : DatabaseProvider
    {
        private string GetInsertSql(TableInfo tableInfo,string name)
        {
            return GetInsertSqlFromCache(name, () =>
            {
                var part = GetInsertSqlParts(tableInfo);
                return string.Format("insert into {0} ({1}) values ({2})", tableInfo.TableName, part.Item1, part.Item2, tableInfo.PrimaryColumn.Name);
            });
        }
        public override object Insert<T>(IDbConnection connection, TableInfo tableInfo, T data, Type tType, IDbTransaction transaction = null)
        {
            connection.Execute(GetInsertSql(tableInfo,tType.FullName), data, transaction);
            return connection.ExecuteScalar("Select LAST_INSERT_ID()", transaction: transaction);
        }
        public override async Task<object> InsertAsync<T>(IDbConnection connection, TableInfo tableInfo, T data, Type tType, IDbTransaction transaction = null)
        {
            await connection.ExecuteAsync(GetInsertSql(tableInfo, tType.FullName), data, transaction);
            return await connection.ExecuteScalarAsync("Select LAST_INSERT_ID()", transaction: transaction);
        }
        public override string GetColumnName(string columnName)
        {
            return string.Format("`{0}`", columnName);
        }
        public override void AppendColumnName(StringBuilder sb, string columnName)
        {
            sb.AppendFormat("`{0}`", columnName);
        }

        public override void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
        {
            sb.AppendFormat("`{0}` = @{1}", columnName, columnName);
        }
        public override string GetColumnNameEqualsValue(string columnName)
        {
            return string.Format("`{0}` = @{1}", columnName, columnName);
        }
        public override string GetExistsSql(string table, string whereSql)
        {
            return string.Format("SELECT EXISTS (SELECT 1 FROM `{0}` WHERE {1})",table,whereSql);
        }
    }
}