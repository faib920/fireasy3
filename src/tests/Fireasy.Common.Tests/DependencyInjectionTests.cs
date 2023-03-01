using Fireasy.Common.DependencyInjection;
using Fireasy.Common.DependencyInjection.Filters;
using Fireasy.Common.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fireasy.Common.Tests
{
    /// <summary>
    /// 依赖注入测试
    /// </summary>
    [TestClass]
    public class DependencyInjectionTests
    {
        /// <summary>
        /// 添加 fireasy 支持
        /// </summary>
        [TestMethod]
        public void TestAddFireasy()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = services.BuildServiceProvider();

            Assert.IsNotNull(serviceProvider);
        }

        /// <summary>
        /// 列出被发现的程序集列表
        /// </summary>
        [TestMethod]
        public void TestDisplayAssemblies()
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

        /// <summary>
        /// 使用自定义的 <see cref="IServiceDiscoverer"/>
        /// </summary>
        [TestMethod]
        public void TestUseMyDiscoverer()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy(opt => opt.DiscoverOptions.DiscovererFactory = (services, options) => new MyServiceDiscoverer(services, options));

            var serviceProvider = services.BuildServiceProvider();

            var discoverer = serviceProvider.GetRequiredService<IServiceDiscoverer>();

            Assert.AreEqual(discoverer.GetType(), typeof(MyServiceDiscoverer));

            foreach (var assembly in discoverer.Assemblies)
            {
                Console.WriteLine(assembly.FullName);
            }
        }

        /// <summary>
        /// 自定义服务发现类
        /// </summary>
        private class MyServiceDiscoverer : DefaultServiceDiscoverer
        {
            public MyServiceDiscoverer(IServiceCollection services, DiscoverOptions options)
                : base(services, options)
            {
            }

            /// <summary>
            /// 只返回当前程序集
            /// </summary>
            /// <returns></returns>
            protected override IEnumerable<Assembly> GetAssemblies()
            {
                yield return this.GetType().Assembly;
            }
        }

        /// <summary>
        /// 使用过滤器，列出被发现的程序集列表
        /// </summary>
        [TestMethod]
        public void TestUseAssemblyFilter()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy(opt => opt.DiscoverOptions.AssemblyFilters.Add(new MyAssemblyFilter()));

            var serviceProvider = services.BuildServiceProvider();

            var discoverer = serviceProvider.GetRequiredService<IServiceDiscoverer>();

            foreach (var assembly in discoverer.Assemblies)
            {
                Console.WriteLine(assembly.FullName);
            }
        }

        private class MyAssemblyFilter : IAssemblyFilter
        {
            public bool IsFilter(Assembly assembly)
            {
                return !assembly.FullName!.StartsWith("Fireasy.Common.Tests");
            }
        }

        /// <summary>
        /// 使用过滤器，列出被发现的程序集列表
        /// </summary>
        [TestMethod]
        public void TestUseAssemblyFilterPredicate()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy(opt => opt.DiscoverOptions.AssemblyFilterPredicates.Add(s => !s.FullName!.StartsWith("Fireasy.Common.Tests")));

            var serviceProvider = services.BuildServiceProvider();

            var discoverer = serviceProvider.GetRequiredService<IServiceDiscoverer>();

            foreach (var assembly in discoverer.Assemblies)
            {
                Console.WriteLine(assembly.FullName);
            }
        }

        /// <summary>
        /// 列出被发现的服务列表
        /// </summary>
        [TestMethod]
        public void TestDisplayDescriptors()
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

        /// <summary>
        /// 测试单例服务
        /// </summary>
        [TestMethod]
        public void TestSingletonService()
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
        }

        /// <summary>
        /// 测试瞬时服务
        /// </summary>
        [TestMethod]
        public void TestTransientService()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = services.BuildServiceProvider();

            var service1 = serviceProvider.GetService<ITestTransientService>();
            var service2 = serviceProvider.GetService<ITestTransientService>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);

            //两对象的id应不相等
            Assert.AreNotEqual(service1.Id, service2.Id);
        }

        /// <summary>
        /// 测试作用域服务
        /// </summary>
        [TestMethod]
        public void TestScopedService()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = services.BuildServiceProvider();

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

        /// <summary>
        /// 测试通过 <see cref="ServiceRegisterAttribute"/> 注册的服务
        /// </summary>
        [TestMethod]
        public void TestWithRegisterAttributeService()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = services.BuildServiceProvider();

            var service1 = serviceProvider.GetService<ITestWithRegisterAttr>();
            var service2 = serviceProvider.GetService<ITestWithRegisterAttr>();
            var service3 = serviceProvider.GetService<TestWithRegisterAttrNonIntefaceImpl>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.IsNotNull(service3);

            //两对象的id应不相等
            Assert.AreNotEqual(service1.Id, service2.Id);
        }

        /// <summary>
        /// 测试使用 <see cref="DisableServerDiscoverAttribute"/> 忽略发现
        /// </summary>
        [TestMethod]
        public void TestDisableDiscover()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<ITestDisableDiscover>();

            Assert.IsNull(service);
        }

        /// <summary>
        /// 测试对象访问器
        /// </summary>
        [TestMethod]
        public void TestObjectAccessor()
        {
            var services = new ServiceCollection();

            services.AddObjectAccessor<ITestSingletonService>(new TestSingletonServiceImpl());
            //services.AddObjectAccessor<ITestSingletonService>(new TestSingletonServiceImpl()); //测试抛错

            var serviceProvider = services.BuildServiceProvider();
            var obj = serviceProvider.GetService<IObjectAccessor<ITestSingletonService>>();
            Assert.IsNotNull(obj);

            using (var scope = serviceProvider.CreateScope())
            {
                var obj1 = scope.ServiceProvider.GetService<IObjectAccessor<ITestSingletonService>>();

                Assert.IsNotNull(obj1);
                Assert.AreEqual(obj, obj1);
            }
        }

        /// <summary>
        /// 测试从 <see cref="IServiceCollection"/> 里获取实例
        /// </summary>
        [TestMethod]
        public void TestGetInstance()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ITestSingletonService>(new TestSingletonServiceImpl());

            var obj1 = services.GetSingletonInstance<ITestSingletonService>();
            var obj2 = services.GetSingletonInstance<ITestSingletonService>();

            Assert.IsNotNull(obj1);
            Assert.AreEqual(obj1, obj2);
        }

        /// <summary>
        /// 测试从 <see cref="IServiceCollection"/> 里获取实例(工厂模式)
        /// </summary>
        [TestMethod]
        public void TestGetInstanceFactory()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ITestSingletonService>(sp => new TestSingletonServiceImpl());

            var obj1 = services.GetSingletonInstance<ITestSingletonService>();
            var obj2 = services.GetSingletonInstance<ITestSingletonService>();

            Assert.IsNotNull(obj1);
            Assert.AreNotEqual(obj1, obj2);
        }

        /// <summary>
        /// 测试从 <see cref="IServiceCollection"/> 里获取实例(工厂模式，但IServiceProvider为空)
        /// </summary>
        [TestMethod]
        public void TestGetInstanceFactoryButServiceProviderIsNull()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ITestSingletonService>(sp => new TestSingletonServiceImplWithArgs(sp));

            Assert.ThrowsException<ArgumentNullException>(() => services.GetSingletonInstance<ITestSingletonService>());
        }

        /// <summary>
        /// 测试动态代理类
        /// </summary>
        [TestMethod]
        public void TestDynamicProxy()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = builder.Services.BuildServiceProvider();
            var obj = serviceProvider.GetService<TestDynamicProxyClass>();

            Assert.AreNotEqual(typeof(TestDynamicProxyClass), obj.GetType());

            var value = obj.GetString();

            Assert.AreEqual("hello world", value);
        }

        /// <summary>
        /// 测试泛型类
        /// </summary>
        [TestMethod]
        public void TestGenericType()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = builder.Services.BuildServiceProvider();
            var obj = serviceProvider.GetService<IGenericService<string, int>>();

            var value = obj.GetValue();

            Assert.AreEqual(null, value);
        }

        #region 测试接口和类
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

        public class TestSingletonServiceImplWithArgs : ITestSingletonService
        {
            public TestSingletonServiceImplWithArgs(IServiceProvider serviceProvider)
            {
                Guard.ArgumentNull(serviceProvider, nameof(serviceProvider));
                Id = Guid.NewGuid();
            }

            public Guid Id { get; }

            public void Test() => Console.WriteLine("Hello TestSingletonService!");
        }

        public interface ITestTransientService
        {
            Guid Id { get; }

            void Test();
        }

        public class TestTransientServiceImpl : ITestTransientService, ITransientService
        {
            public TestTransientServiceImpl()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; }

            public void Test() => Console.WriteLine("Hello TestTransientService!");
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

        public interface ITestWithRegisterAttr
        {
            Guid Id { get; }

            void Test();
        }

        [ServiceRegister(ServiceLifetime.Transient)]
        public class TestWithRegisterAttrImpl : ITestWithRegisterAttr
        {
            public TestWithRegisterAttrImpl()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; }

            public void Test() => Console.WriteLine("Hello TestWithRegisterAttribute!");
        }

        [ServiceRegister(ServiceLifetime.Transient)]
        public class TestWithRegisterAttrNonIntefaceImpl
        {
            public void Test() => Console.WriteLine("Hello TestWithRegisterAttribute!");
        }

        public interface ITestWithRegisterOneselfAttr
        {
            Guid Id { get; }

            void Test();
        }

        public interface ITestDisableDiscover
        {
            void Test();
        }

        [DisableServerDiscover]
        public class TestDisableDiscoverImpl : ITestDisableDiscover
        {
            public void Test() => Console.WriteLine("Hello TestWithRegisterAttribute!");
        }

        public interface IGenericService<T1, T2>
        {
            T1 GetValue();
        }

        public class GenericService<T1, T2> : IGenericService<T1, T2>, ITransientService
        {
            public T1 GetValue()
            {
                return default;
            }
        }

        public class TestDynamicProxyClass : ITransientService
        {
            [Intercept(typeof(AnyInterceptor))]
            public virtual string GetString()
            {
                return string.Empty;
            }
        }

        public class AnyInterceptor : IInterceptor
        {
            public void Initialize(InterceptContext context)
            {
            }

            public void Intercept(InterceptCallInfo info)
            {
                info.ReturnValue = "hello world";

                Console.WriteLine(info.InterceptType);
            }
        }

        #endregion
    }
}