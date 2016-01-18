# SunDapper
对Dapper的轻量拓展，不改变Dapper的操作方式
所有功能都支持异步操作，Sql使用模板缓存，首次以外基本达到手写性能

Example usage:

`@查询分页数据`
```csharp
using (var conn = GetConnection())
{
    long currentPage = 1;
    long pageSize = 10;
    var pageList = conn.QueryPage<sys_module>(currentPage, pageSize,
                    "SELECT * FROM sys_module where moduleid<@maxId", new { maxId = 10 });
    var pageList = await conn.QueryPageAsync<sys_module>(currentPage, pageSize,
                    "SELECT * FROM sys_module where moduleid<@maxId", new { maxId = 10 });
    
}
```

`@根据主键查询`
```csharp
using (var conn = GetConnection())
{
    var data = conn.Single<sys_module>(14);
    var data = await conn.SingleAsync<sys_module>(14);
}
```

`@更新数据`
```csharp
using (var conn = GetConnection())
{

    var data = conn.Single<sys_module>(14);
    data.Name = "Test";
    int count= conn.Update(data);
    int count = await conn.UpdateAsync(data);
    //指定更新那些字段
    int count = conn.Update(data,columns: new List<string>() { "name","key"});
    //指定不更新那些字段
    int count = conn.Update(data,noColumns:new List<string>() { "name", "key" });
}
```

`@插入数据`
```csharp
using (var conn = GetConnection())
{

    var data = conn.Single<sys_module>(14);
    data.Name = "Test";
    int id= conn.Insert(data);
    int id = await conn.InsertAsync(data);
}
```
`@删除数据`
```csharp
using (var conn = GetConnection())
{

    conn.Delete<sys_module>(14);
    await conn.DeleteAsync<sys_module>(14);
}
```
