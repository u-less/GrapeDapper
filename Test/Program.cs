using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Diagnostics;
using GrapeDapper;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {            //    var builder = new SqlBuilder();

            //    var query = builder.AddTemplate("SELECT Id, This, That, TheOther" +
            //                                    "FROM MyTable /**where**/ {{ORDERBY}}" +
            //                                    "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
            //        new { skip = 10, take = 25 });


            //builder.Where("id > @id", new { id = 5 });
            //builder.OrWhere("That = @That", new { that = 10 });

            //var count = builder.AddTemplate("SELECT Count(*) FROM MyTable {{WHERE}}");


            // const string sql = "SELECT * from sys_module";
            //const string sqlOne = @"SELECT * from sys_module LIMIT 10 OFFSET 0 
            //                       SELECT COUNT(*) from sys_module";
            var connectionString = "Server=127.0.0.1;Port=5432;Database=fang_base;User Id=postgres;Password=luohuazhiyu";
            DbFactory factory = new DbFactory(GrapeDapper.SqlAdapter.SqlType.Npgsql, connectionString);
            while (true)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                //var data = conn.Single<sys_module>(14);
                //data.ModuleKey = "wode1";
                //var id = conn.Delete<sys_module>(89);
                watch.Stop();

                using (var conn = factory.GetConnection())
                {
                    long currentPage = 1;
                    long pageSize = 10;
                    var data = conn.QueryPage<sys_module>(currentPage, pageSize, "SELECT * FROM sys_module where moduleid<@maxId", new { maxId = 10 });
                    var exts = conn.Exists<sys_module>("moduleid>@maxId", new { maxId = 10000 });
                }
                Console.WriteLine("10000次列表查询,每次查询13条数据，耗时：{0} 秒{1}毫秒", watch.Elapsed.Seconds, watch.Elapsed.Milliseconds);
                Console.ReadLine();
            }
        }
    }
}
