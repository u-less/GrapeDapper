# GrapeDapper
对Dapper的轻量拓展，不改变Dapper的操作方式
所有功能都支持异步操作，Sql使用模板缓存，首次以外基本达到手写性能

Example usage:

`@实体定义(可使用标记也可以不使用标记，使用标记是为了提供更多的灵活性)`
```csharp
    [Table(AutoIncrement = true)]//表标记，括号里为主键是否是自增
    public class sys_module
    { 

        [Key]//主键标记
        public int Id
        {
            get;
            set;
        }
        [Ignore]//完全忽略，不参与数据库操作，逻辑字段
        public string Name
        {
            get;
            set;
        }
        [Result]//标记为结果字段，只对查询结果有用，对新增修改无用
        public int ParentId
        {
            get;
            set;
        }
        [Column]//标记为列，可有可无
        public string key
        {
            get;
            set;
        }
    }
```
`@查询所有数据`
```csharp
using (var conn = GetConnection())
{
    conn.GetAll<sys_module>();
    await conn.GetAllAsync<sys_module>();
    conn.GetAllPage<sys_module>(1,10);
    await conn.GetAllPageAsync<sys_module>(1,10);
}
```

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
    var allCount=pageList.TotalItems;//获取总共多少条数据
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
`@判断数据是否存在`
```csharp
using (var conn = GetConnection())
{
    conn.Exists<sys_module>(14);
    await conn.ExistsAsync<sys_module>(14);
    conn.Exists<sys_module>("moduleid=@moduleid",new{moduleid=14});
    await conn.ExistsAsync<sys_module>("moduleid=@moduleid",new{moduleid=14});
}
```
