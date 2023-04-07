# fireasy 3.0
----

　　fireasy 3.0 在 2.0 的基础上进行了重构，抛弃了 `.Net Framework` 时代的一些思想和模式，使之完全与 `.Net Core` 相匹配。

　　目前 3.0 还处于开发阶段，到正式发布还需要一段时间，感谢你的关注与支持。

　　<img src="http://fireasy.cn/content/upload/donate_weixin.jpg" style="height:240px" /> <img src="http://fireasy.cn/content/upload/qqgroup.png" style="height:240px" />

## 一、目标框架
　　目前目标框架为 `netstandard2.0`、`netstandard2.1` 和 `net6.0`。

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
        Application.ThreadException += Application_ThreadException;
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
public void Test()
{
    var services = new ServiceCollection();

    //只遍列 Fireasy.Common.Tests
    services.AddFireasy(opt => opt.DiscoverOptions.AssemblyFilterPredicates.Add(s => !s.FullName!.StartsWith("Fireasy.Common.Tests")));
}
```

* 使用 `IAssemblyFilter` 对程序集进行过滤

```csharp
public void Test()
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
public void Test()
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
public void Test()
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
public void Test()
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
    /// <summary>
    /// 异步方法
    /// </summary>
    /// <returns></returns>
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