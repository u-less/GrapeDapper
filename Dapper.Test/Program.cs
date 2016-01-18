using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dapper;
using System.Diagnostics;
using SunDapper;

namespace DapperT.Test
{
    class Program
    {
        static void Main(string[] args)
        {
        //    var builder = new SqlBuilder();

        //    var query = builder.AddTemplate("SELECT Id, This, That, TheOther" +
        //                                    "FROM MyTable /**where**/ {{ORDERBY}}" +
        //                                    "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
        //        new { skip = 10, take = 25 });
            DbProviderFactory factory = GetFactory("Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7");

            //builder.Where("id > @id", new { id = 5 });
            //builder.OrWhere("That = @That", new { that = 10 });

            //var count = builder.AddTemplate("SELECT Count(*) FROM MyTable {{WHERE}}");
            var connection = factory.CreateConnection();
            connection.ConnectionString = "Server=127.0.0.1;Port=5432;Database=fang_base;User Id=postgres;Password=luohuazhiyu";
            connection.Open();
            DapperConnection conn = new DapperConnection();
            conn.Connection = connection;
            conn.SqlDbType = SunDapper.Sql.SqlType.Npgsql;
            conn.SqlProvider = SunDapper.Sql.Provider.ProviderFactory.GetProvider(conn.SqlDbType);
           // const string sql = "SELECT * from sys_module";
            //const string sqlOne = @"SELECT * from sys_module LIMIT 10 OFFSET 0 
            //                       SELECT COUNT(*) from sys_module";
            //using (var multi = connection.QueryMultiple(sqlOne))
            //{
            //    var list= multi.Read<sys_module>().ToList();
            //    var TotalItems = multi.ReadSingle<long>();
            //}
                while (true)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1000; i++)
                {
                    var data = conn.QueryPage<sys_module>(1, 10, "SELECT * FROM sys_module where moduleid<@maxId", new { maxId = 10 });
                    // var data = conn.Connection.QuerySingle<sys_module>("SELECT * FROM sys_module where moduleid=@moduleid", new { moduleid = 14 });
                }
                //var data = conn.Single<sys_module>(14);
                //data.ModuleKey = "wode1";
                //var id = conn.Delete<sys_module>(89);
                watch.Stop();
                Console.WriteLine("10000次列表查询,每次查询13条数据，耗时：{0} 秒{1}毫秒", watch.Elapsed.Seconds, watch.Elapsed.Milliseconds);
                Console.ReadLine();
            }
        }
        protected static DbProviderFactory GetFactory(string assemblyQualifiedName)
        {
            var ft = Type.GetType(assemblyQualifiedName);

            return (DbProviderFactory)ft.GetField("Instance").GetValue(null);
        }
    }
}
