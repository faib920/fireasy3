# fireasy 3.0
----

　　fireasy 3.0 在 2.0 的基础上进行了重构，抛弃了 `.Net Framework` 时代的一些思想和模式，使之完全与 `.Net Core` 相匹配。

　　目前 3.0 还处于开发阶段，到正式发布还需要一段时间，感谢你的关注与支持。

　　<img src="http://fireasy.cn/content/upload/donate_weixin.jpg" style="height:240px" /> <img src="http://fireasy.cn/content/upload/qqgroup.png" style="height:240px" />

## 一、目标框架
　　目前目标框架为 `net462`、`netstandard2.0`、`netstandard2.1` 和 `net6.0`。

## 二、部分程序集

| 程序集 | 说明 |
| :--- | :--- |
|Fireasy.Common|公共类库，提供依赖注入、动态代理、代码编译、静态编译、反射缓存等等，以及各类基础接口的定义|
|Fireasy.Composition|提供 MEF 的集成|
|Fireasy.Common.Analyzers|Fireasy.Common 的语法分析类库，提供动态代理(AOP)生成、依赖服务发现与注入|
|Fireasy.CodeCompiler.VisualBasic|提供代码编译对 VB.NET 语言支持|
|Fireasy.Configuration|配置相关类库，提供配置的解析和配置|
|Fireasy.Data.Abstractions|数据库抽象类库，定义数据库层面的各类接口、适配器接口(语法、架构、批量插入、记录包装等等)|
|Fireasy.Data|数据库实现类库，对抽象类库的实现，提供SqlServer、MySql、SQLite、Firebird、PostgreSql、Oracle、达梦、人大金仓和神通数据库的适配|
|Fireasy.Data.Configuration|数据库配置相关类库，提供数据库适配器及连接字符串的配置解析|
|Fireasy.Data.OleDb|OleDb 适配|
|Fireasy.Data.Analyzers|Fireasy.Data 的语法分析类库，提供 BulkCopy 适配器的生成|
|Fireasy.Data.Entity.Abstractions|实体框架的抽象类库，定义 EntityContext、实体仓储等等|
|Fireasy.Data.Entity|实体框架的具体实现，提供SqlServer、MySql、SQLite、Firebird、PostgreSql、Oracle、达梦、人大金仓和神通数据库的 linq 查询解析|
|Fireasy.ObjectMapping.Abstractions|对象映射抽象类库，定义对象映射的接口|
|Fireasy.AutoMapper|对象映射之 AutoMapper 适配|
|Fireasy.Mapster|对象映射之 Mapster 适配|

## 三、开始使用

　　要使用 Fireasy 相关的组件，必须在 `IServiceCollection` 对象上使用扩展方法 `AddFireasy()`。

### 1、Winform

```csharp
static class Program
{
    public static IServiceProvider ServiceProvider { get; set; }

    [STAThread]
    static void Main(params string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
            
        var services = new ServiceCollection();

        //必不可少
        services.AddFireasy();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        ServiceProvider = services.BuildServiceProvider();

        Application.Run(new frmMain());
    }
}
```

### 2、Console

```csharp
static class Program
{
    public static IServiceProvider ServiceProvider { get; set; }

    static void Main(params string[] args)
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        //必不可少
        services.AddFireasy();

        services.AddSingleton<IConfiguration>(configuration);
        ServiceProvider = services.BuildServiceProvider();

        Console.ReadKey();
    }
}
```

### 3、ASP.NET Core

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        //必不可少
        services.AddFireasy();
    }
}
```

## 四、公共类库

### 1、服务发现

* 使用 `Predicate` 对程序集进行过滤

```csharp
private void Test()
{
    var services = new ServiceCollection();

    //只遍列 Fireasy.Common.Tests
    services.AddFireasy(opt => opt.DiscoverOptions.AssemblyFilterPredicates.Add(s => !s.FullName!.StartsWith("Fireasy.Common.Tests")));
}
```

* 使用 `IAssemblyFilter` 对程序集进行过滤

```csharp
private void Test()
{
    var services = new ServiceCollection();
    services.AddFireasy(opt => opt.DiscoverOptions.AssemblyFilters.Add(new MyAssemblyFilter()));

    private class MyAssemblyFilter : IAssemblyFilter
    {
        public bool IsFilter(Assembly assembly)
        {
            //只遍列 Fireasy.Common.Tests
            return !assembly.FullName!.StartsWith("Fireasy.Common.Tests");
        }
    }
}
```

* 列出遍列过的程序集

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var discoverer = serviceProvider.GetRequiredService<IServiceDiscoverer>();

    foreach (var assembly in discoverer.Assemblies)
    {
        Console.WriteLine(assembly.FullName);
    }
}
```

* 列出注册的所有服务描述

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var discoverer = serviceProvider.GetRequiredService<IServiceDiscoverer>();

    foreach (var desc in discoverer.Descriptors)
    {
        Console.WriteLine($"{desc.ServiceType} -> {desc.ImplementationType}");
    }
}
```

* 发现 `ISingletonService`、`ITransientService` 或 `IScopedService` 三种生命周期的服务

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var service1 = serviceProvider.GetService<ITestSingletonService>();
    var service2 = serviceProvider.GetService<ITestSingletonService>();

    Assert.IsNotNull(service1);
    Assert.IsNotNull(service2);

    //两对象的id应相等
    Assert.AreEqual(service1.Id, service2.Id);

    Guid id1, id2;

    //作用域1
    using (var scope1 = serviceProvider.CreateScope())
    {
        var service1 = scope1.ServiceProvider.GetService<ITestScopedService>();
        var service2 = scope1.ServiceProvider.GetService<ITestScopedService>();

        Assert.IsNotNull(service1);
        Assert.IsNotNull(service2);

        //两对象的id应相等
        Assert.AreEqual(service1.Id, service2.Id);

        id1 = service1.Id;
    }

    //作用域2
    using (var scope2 = serviceProvider.CreateScope())
    {
        var service1 = scope2.ServiceProvider.GetService<ITestScopedService>();
        var service2 = scope2.ServiceProvider.GetService<ITestScopedService>();

        Assert.IsNotNull(service1);
        Assert.IsNotNull(service2);

        //两对象的id应相等
        Assert.AreEqual(service1.Id, service2.Id);

        id2 = service1.Id;
    }

    //两次scoped的id应不相等
    Assert.AreNotEqual(id1, id2);
}

public interface ITestSingletonService
{
    Guid Id { get; }

    void Test();
}

public class TestSingletonServiceImpl : ITestSingletonService, ISingletonService
{
    public TestSingletonServiceImpl()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }

    public void Test() => Console.WriteLine("Hello TestSingletonService!");
}

public interface ITestScopedService
{
    Guid Id { get; }

    void Test();
}

public class TestScopedServiceImpl : ITestScopedService, IScopedService
{
    public TestScopedServiceImpl()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }

    public void Test() => Console.WriteLine("Hello TestScopedService!");
}
```

* 使用服务部署器，自动注册服务

```csharp
[assembly: ServicesDeploy(typeof(DataServicesDeployer))]

public class DataServicesDeployer : IServicesDeployer
{
    void IServicesDeployer.Configure(IServiceCollection services)
    {
        services.AddSingleton<IProviderManager, DefaultProviderManager>();
        services.AddSingleton<IDatabaseFactory, DefaultDatabaseFactory>();
        services.AddSingleton<IRowMapperFactory, DefaultRowMapperFactory>();
        services.AddSingleton<IValueConvertManager, DefaultValueConvertManager>();
        services.AddScoped<IDatabase>(sp => sp.GetRequiredService<IDatabaseFactory>().CreateDatabase());
    }
}
```

### 2、动态代理

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var proxyFactory = serviceProvider.GetService<IDynamicProxyFactory>();
    var proxyObj = proxyFactory!.BuildProxy<TestProxy>();

    var value = await proxyObj!.GetStringAsync();
    Assert.AreEqual("hello world", value);
}

public class TestProxy
{
    [Intercept(typeof(GetStringAsyncInterceptor))]
    public virtual Task<string> GetStringAsync()
    {
        return Task.FromResult(string.Empty);
    }
}

public class GetStringAsyncInterceptor : IAsyncInterceptor
{
    public ValueTask InitializeAsync(InterceptContext context)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask InterceptAsync(InterceptCallInfo info)
    {
        info.ReturnValue = "hello world";

        return ValueTask.CompletedTask;
    }
}
```

### 3、反射缓存

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var reflectionFactory = serviceProvider.GetService<IReflectionFactory>();

    var obj = new OneObject();
    var property = typeof(OneObject).GetTypeInfo().GetDeclaredProperty(nameof(OneObject.Value));
    var accessor = reflectionFactory!.GetAccessor<int>(property!);

    accessor.SetValue(obj, 100);
    var value = accessor.GetValue(obj);

    Assert.AreEqual(100, value);
}

private class OneObject
{
    public int Value { get; set; }
}
```

### 4、动态编译

```csharp
private void Test()
{
    /*
    public class MyClass<T, TS> where T : MyBaseClass
    {
        public MyClass(TS ts)
        {
        }
        public T Hello<TV>(T t, TV tv)
        {
            Console.WriteLine(tv);
            return t;
        }
    }
    */

    var gt = new GtpType("T").SetBaseTypeConstraint(typeof(MyBaseClass));

    var assemblyBuilder = new DynamicAssemblyBuilder("MyAssembly");
    var typeBuilder = assemblyBuilder.DefineType("MyClass");

    //定义泛型类型参数
    typeBuilder.DefineGenericParameters(gt, new GtpType("TS"));

    //定义构造函数
    typeBuilder.DefineConstructor(new Type[] { new GtpType("TS") });

    //定义一个泛型方法，TV不在类中定义，所以属于方法的泛型类型参数
    var methodBuilder = typeBuilder.DefineMethod("Hello", gt, new Type[] { gt, new GtpType("TV") }, ilCoding: c =>
    {
        c.Emitter
        .ldarg_2.call(typeof(Console).GetMethod("WriteLine", new[] { typeof(object) }))
        .ldarg_1.ret();
    });

    var type = typeBuilder.CreateType().MakeGenericType(typeof(MyBaseClass), typeof(int));
    var obj = Activator.CreateInstance(type, 100);

    var method = type.GetMethod("Hello").MakeGenericMethod(typeof(string));
    var value = method.Invoke(obj, new object[] { new MyBaseClass(), "world" });

    Assert.IsInstanceOfType(value, typeof(MyBaseClass));
}
```

### 5、代码编译

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var source = @"
public class TestClass
{
    public string Hello(string str)
    {
        return str;
    }
}";

    var codeCompilerManager = serviceProvider.GetService<ICodeCompilerManager>();
    var codeCompiler = codeCompilerManager!.CreateCompiler("csharp");

    var opt = new ConfigureOptions();
    opt.Assemblies.Add("System.Core.dll");

    var assembly = codeCompiler!.CompileAssembly(source, opt);

    var type = assembly!.GetType("TestClass");

    Assert.IsNotNull(type);
}
```

　　要使用 `VB.NET` 语言只需要在项目中引用 `Fireasy.CodeCompiler.VisualBasic` 即可。

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var source = @"
Public Class A
    Public Function Hello(ByVal str As String) As String
        Return str
    End Function
End Class";

    var codeCompilerManager = serviceProvider.GetService<ICodeCompilerManager>();
    var codeCompiler = codeCompilerManager!.CreateCompiler("vb");

    var assembly = codeCompiler!.CompileAssembly(source);

    var type = assembly!.GetType("A");

    Assert.IsNotNull(type);
}
```

### 6、对象序列化

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var obj = new TestObject { Name = "fireasy", Address = "kunming", Age = 30 };

    var serializer = serviceProvider.GetRequiredService<IJsonSerializer>();

    //在明确提供方的情况下，可以指定 JsonSerializerOptions，但是更换提供方后，相应的也要切换
    var json = serializer.Serialize(obj, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });

    Console.WriteLine(json);
}
```

　　除了 `Json` 序列化，还提供了二进制序列化 `IBinarySerializer`，但目前还没有适配。

### 7、MEF 服务导出

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    var serviceProvider = services.BuildServiceProvider();

    //这是一个扩展方法 GetExportedServices
    var exportedServices = serviceProvider.GetExportedServices<IExportService>();
    Assert.AreEqual(2, exportedServices.Count());
}

public interface IExportService
{
}

[Export(typeof(IExportService))]
public class ExportService1 : IExportService
{
}

[Export(typeof(IExportService))]
public class ExportService2 : IExportService
{
}
```

　　使用配置进行匹配，`assembly` 匹配程序集名称，`pattern` 匹配文件名称。

```json
{
  "fireasy": {
    "imports": {
      "settings": {
        "usePattern": {
          "pattern": "Fireasy.*.dll"
        },
        "useAssembly": {
          "assembly": "Fireasy.Composition.Tests"
        }
      }
    }
  }
}
```

### 8、动态扩展对象

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    //如果不获取 DynamicDescriptionSupporter 则该测试将失败
    var supporter = serviceProvider.GetService<DynamicDescriptionSupporter>();

    var obj = (IDictionary<string, object>)new DynamicExpandoObject();
    obj.Add("Name", "fireasy");

    var property = TypeDescriptor.GetProperties(obj).Find("Name", false);

    Assert.IsNotNull(property);

    var value = property.GetValue(obj);

    Assert.AreEqual("fireasy", value);
}
```

## 五、数据库类库

　　`IProvider` 定义了一套标准，不同的数据库类型有不同的实现。另外，每种数据库还应实现以下插件服务接口：

* `ISyntaxProvider` 语法插件服务

* `ISchemaProvider` 架构插件服务

* `IBatcherProvider` 批量插入插件服务

* `IRecordWrapper` 记录包装器插件服务

* `IGeneratorProvider` 标识生成器插件服务

　　目前已经提供了 SqlServer、MySql、SQLite、Firebird、PostgreSql、Oracle、Dameng(达梦)、Kingbase(人大金仓)和ShenTong(神通)数据库，以及 OleDb 驱动。

　　Fireasy.Data 中未引用相关的 Nuget 包，只要在项目中直接安装 Nuget 包即可。

| 数据库类型 | Nuget 包 |
| :-- | :-- |
| SqlServer | Microsoft.Data.SqlClient、System.Data.SqlClient |
| MySql | MySql.Data、MySqlConnector |
| SQLite | System.Data.SQLite、Microsoft.Data.Sqlite |
| Firebird | FirebirdSql.Data.FirebirdClient |
| PostgreSql | Npgsql |
| Oracle | Oracle.ManagedDataAccess |
| Dameng | DmProvider |
| Kingbase | Kdbndp |
| ShenTong | Oscar.Data.SqlClient |
| OleDb | System.Data.OleDb |

### 1、列举数据库提供者(比如SqlServer、MySql等等)

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();
    var serviceProvider = services.BuildServiceProvider();

    var manager = serviceProvider.GetRequiredService<IProviderManager>();

    var descriptors = manager.GetSupportedProviders();

    foreach (var item in descriptors)
    {
        Console.WriteLine($"{item.Alais} {item.Description}");
    }
}
```

### 2、更换提供者或插件服务

```csharp
private void Test()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();

    //用于替换 SqlServerProvider 中的 DbProviderFactory
    builder.ConfigureData(s => s.AddProivderFactory<SqlServerProvider>(System.Data.SqlClient.SqlClientFactory.Instance));

    //用于替换数据库提供者，SqlServer 不再使用 SqlServerProvider，而是使用自定义的 TestProvider
    //sqlserver使用2012以下版本的语法
    builder.ConfigureData(s => s.AddProvider<TestProvider>("SqlServer").AddProivderService<SqlServerProvider, SqlServerSyntaxLessThan2012>());

    var serviceProvider = services.BuildServiceProvider();
}
```

### 3、使用配置文件配置连接字符串

```json
{
  "fireasy": {
    "dataInstances": {
      "default": "sqlserver",
      "settings": {
        "sqlite": {
          "providerType": "SQLite",
          "connectionString": "Data source=|appdir|..\\..\\..\\..\\..\\..\\db\\Northwind.db3;Pooling=True"
        },
        "sqlserver": {
          "providerType": "SqlServer",
          "connectionString": "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|appdir|..\\..\\..\\..\\..\\..\\db\\Northwind.mdf;Integrated Security=True;Connect Timeout=30"
        },
        "oracle": {
          "providerType": "Oracle",
          "connectionString": "Data Source=localhost/orcl;User ID=c##test;Password=Faib1234"
        },
        "mysql": {
          "providerType": "MySql",
          "connectionString": "Data Source=localhost;database=northwind;User Id=root;password=faib;pooling=true;charset=utf8"
        },
        "oledb_access": {
          "providerType": "OleDb",
          "connectionString": "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|appdir|..\\..\\..\\..\\..\\..\\db\\Northwind.accdb;Persist Security Info=False;"
        },
        "testdb": {
          "providerType": "myprovider",
          "connectionString": "Data Source=localhost;database=northwind;User Id=root;password=faib;pooling=true;charset=utf8"
        }
      }
    },
    "dataProviders": {
      "settings": {
        "myprovider": {
          "type": "Fireasy.Data.Tests.TestProvider, Fireasy.Data.Tests"
        }
      }
    }
  }
}
```

### 4、创建数据库实例

```csharp
private async Task TestAsync()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    var serviceProvider = services.BuildServiceProvider();

    var factory = serviceProvider.GetRequiredService<IDatabaseFactory>();

    //指定提供者及连接串
    await using var database = factory.CreateDatabase<MySqlProvider>("Data Source=localhost;database=northwind;User Id=root;password=faib;pooling=true;charset=utf8");
    Assert.IsNotNull(database);

    //指定数据库配置名称
    await using var database = factory.CreateDatabase("oledb_access");
    Assert.IsNotNull(database);
}
```

### 5、数据库增删改查

```csharp
private async Task TestAsync()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    var serviceProvider = services.BuildServiceProvider();

    var factory = serviceProvider.GetRequiredService<IDatabaseFactory>();

    await using var database = factory.CreateDatabase("mysql");

    //查询列表，带分页
    var pager = new DataPager(10, 1);
    var list = await database.ExecuteEnumerableAsync<Customers>($"select * from customers order by CustomerID", pager);

    //单一字段
    var value = await database.ExecuteScalarAsync<byte[]>($"select Picture from categories where CategoryID=1");

    //更新字段，参数化
    var parameters = new ParameterCollection();
    parameters.Add("customerId", "ALFKI");
    parameters.Add("country", "Germany");
    var ret = await database.ExecuteNonQueryAsync($"update customers set Country=@country where CustomerID=@customerId", parameters: parameters);
}
```

### 6、使用事务

```csharp
private async Task TestAsync()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    var serviceProvider = services.BuildServiceProvider();

    var factory = serviceProvider.GetRequiredService<IDatabaseFactory>();

    await using var database = factory.CreateDatabase("mysql");
    await database.BeginTransactionAsync();

    try
    {
        await database.ExecuteNonQueryAsync($"delete from customers");
        //表不存在，将抛错
        await database.ExecuteNonQueryAsync($"delete from testtable");

        await database.CommitTransactionAsync();
    }
    catch (Exception)
    {
        await database.RollbackTransactionAsync();
    }
}
```

### 7、获取数据库架构

```csharp
private async Task TestAsync()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    var serviceProvider = services.BuildServiceProvider();

    var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

    await using var database = factory.CreateDatabase<T>(ConnectionString);
    var schema = database.GetService<ISchemaProvider>();
    var syntax = database.GetService<ISyntaxProvider>();

    //获取表
    var tables = await schema!.GetSchemasAsync<Data.Schema.Table>(database).ToListAsync();
    
    //获取字段
    var columns = await schema!.GetSchemasAsync<Data.Schema.Column>(database, s => s.TableName == syntax!.ToggleCase("products") && s.Name == "ProductID").ToListAsync();

    //获取外键
    var foreignKeys = await schema!.GetSchemasAsync<Data.Schema.ForeignKey>(database, s => s.TableName.Equals(syntax!.ToggleCase("orders"))).ToListAsync();
}
```

### 8、批量插入

```csharp
private async Task TestAsync()
{
    var services = new ServiceCollection();
    var builder = services.AddFireasy();

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    var serviceProvider = services.BuildServiceProvider();

    var factory = ServiceProvider.GetRequiredService<IDatabaseFactory>();

    await using var database = factory.CreateDatabase<T>(ConnectionString);
    var batcher = database.GetService<IBatcherProvider>();

    var list = new List<BatcherData>();

    for (var i = 0; i < 100000; i++)
    {
        list.Add(new BatcherData(i + 1, "Name" + i, "Address" + i));
    }

    await batcher.InsertAsync(database, list, "batchers");
}

private class BatcherData
{
    public BatcherData(int id, string name, string address)
    {
        Id = id;
        Name = name;
        Address = address;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}
```

## 技术揭秘系列文章
- [Fireasy3 揭秘 -- 依赖注入与服务发现](https://www.cnblogs.com/fireasy/p/17170417.html)
- [Fireasy3 揭秘 -- 自动服务部署](https://www.cnblogs.com/fireasy/p/17173997.html)
- [Fireasy3 揭秘 -- 使用 SourceGeneraor 改进服务发现](https://www.cnblogs.com/fireasy/p/17174121.html)
- [Fireasy3 揭秘 -- 使用 SourceGeneraor 实现动态代理(AOP)](https://www.cnblogs.com/fireasy/p/17179651.html)
- [Fireasy3 揭秘 -- 使用 Emit 构建程序集](https://www.cnblogs.com/fireasy/p/17201880.html)
- [Fireasy3 揭秘 -- 代码编译器及适配器](https://www.cnblogs.com/fireasy/p/17213296.html)
- Fireasy3 揭秘 -- 使用缓存提高反射性能
- Fireasy3 揭秘 -- 动态类型及扩展支持
- Fireasy3 揭秘 -- 线程数据共享的实现
- Fireasy3 揭秘 -- 配置管理及解析处理
- Fireasy3 揭秘 -- 数据库适配器
- Fireasy3 揭秘 -- 解决数据库之间的语法差异
- Fireasy3 揭秘 -- 获取数据库的架构信息
- Fireasy3 揭秘 -- 数据批量插入的实现
- Fireasy3 揭秘 -- 使用包装器对数据读取进行兼容
- Fireasy3 揭秘 -- 数据行映射器
- Fireasy3 揭秘 -- 数据转换器的实现
- Fireasy3 揭秘 -- 通用序列生成器和雪花生成器的实现
- Fireasy3 揭秘 -- 命令拦截器的实现
- Fireasy3 揭秘 -- 数据库主从同步的实现
- Fireasy3 揭秘 -- 大数据分页的策略
- Fireasy3 揭秘 -- 数据按需更新及生成实体代理类
- Fireasy3 揭秘 -- 用对象池技术管理上下文
- Fireasy3 揭秘 -- Lambda 表达式解析的原理
- Fireasy3 揭秘 -- 扩展选择的实现
- Fireasy3 揭秘 -- 按需加载与惰性加载的区别与实现
- Fireasy3 揭秘 -- 自定义函数的解析与绑定
- Fireasy3 揭秘 -- 与 MongoDB 进行适配
- Fireasy3 揭秘 -- 模块化的实现原理