using Fireasy.Common.DependencyInjection;
using Fireasy.Common.DependencyInjection.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fireasy.Common.Tests
{
    /// <summary>
    /// ����ע�����
    /// </summary>
    [TestClass]
    public class DependencyInjectionTests
    {
        /// <summary>
        /// ��� fireasy ֧��
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
        /// �г������ֵĳ����б�
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
        /// ʹ���Զ���� <see cref="IServiceDiscoverer"/>
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
        /// �Զ����������
        /// </summary>
        private class MyServiceDiscoverer : DefaultServiceDiscoverer
        {
            public MyServiceDiscoverer(IServiceCollection services, DiscoverOptions options)
                : base(services, options)
            {
            }

            /// <summary>
            /// ֻ���ص�ǰ����
            /// </summary>
            /// <returns></returns>
            protected override IEnumerable<Assembly> GetAssemblies()
            {
                yield return this.GetType().Assembly;
            }
        }

        /// <summary>
        /// ʹ�ù��������г������ֵĳ����б�
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
        /// ʹ�ù��������г������ֵĳ����б�
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
        /// �г������ֵķ����б�
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
        /// ���Ե�������
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

            //�������idӦ���
            Assert.AreEqual(service1.Id, service2.Id);
        }

        /// <summary>
        /// ����˲ʱ����
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

            //�������idӦ�����
            Assert.AreNotEqual(service1.Id, service2.Id);
        }

        /// <summary>
        /// �������������
        /// </summary>
        [TestMethod]
        public void TestScopedService()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = services.BuildServiceProvider();

            Guid id1, id2;

            //������1
            using (var scope1 = serviceProvider.CreateScope())
            {
                var service1 = scope1.ServiceProvider.GetService<ITestScopedService>();
                var service2 = scope1.ServiceProvider.GetService<ITestScopedService>();

                Assert.IsNotNull(service1);
                Assert.IsNotNull(service2);

                //�������idӦ���
                Assert.AreEqual(service1.Id, service2.Id);

                id1 = service1.Id;
            }

            //������2
            using (var scope2 = serviceProvider.CreateScope())
            {
                var service1 = scope2.ServiceProvider.GetService<ITestScopedService>();
                var service2 = scope2.ServiceProvider.GetService<ITestScopedService>();

                Assert.IsNotNull(service1);
                Assert.IsNotNull(service2);

                //�������idӦ���
                Assert.AreEqual(service1.Id, service2.Id);

                id2 = service1.Id;
            }

            //����scoped��idӦ�����
            Assert.AreNotEqual(id1, id2);
        }

        /// <summary>
        /// ����ͨ�� <see cref="ServiceRegisterAttribute"/> ע��ķ���
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

            //�������idӦ�����
            Assert.AreNotEqual(service1.Id, service2.Id);
        }

        /// <summary>
        /// ����ͨ�� <see cref="RegisterOneselfAttribute"/> ע������ķ���
        /// </summary>
        [TestMethod]
        public void TestWithRegisterOneselfService()
        {
            var services = new ServiceCollection();

            var builder = services.AddFireasy();

            var serviceProvider = services.BuildServiceProvider();

            var service1 = serviceProvider.GetService<TestWithRegisterOneselfAttrImpl>();
            var service2 = serviceProvider.GetService<ITestWithRegisterOneselfAttr>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
        }

        /// <summary>
        /// ����ʹ�� <see cref="DisableServerDiscoverAttribute"/> ���Է���
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
        /// ���Զ��������
        /// </summary>
        [TestMethod]
        public void TestObjectAccessor()
        {
            var services = new ServiceCollection();

            services.AddObjectAccessor<ITestSingletonService>(new TestSingletonServiceImpl());
            //services.AddObjectAccessor<ITestSingletonService>(new TestSingletonServiceImpl()); //�����״�

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
        /// ���Դ� <see cref="IServiceCollection"/> ���ȡʵ��
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
        /// ���Դ� <see cref="IServiceCollection"/> ���ȡʵ��(����ģʽ)
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
        /// ���Դ� <see cref="IServiceCollection"/> ���ȡʵ��(����ģʽ����IServiceProviderΪ��)
        /// </summary>
        [TestMethod]
        public void TestGetInstanceFactoryButServiceProviderIsNull()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ITestSingletonService>(sp => new TestSingletonServiceImplWithArgs(sp));

            Assert.ThrowsException<ArgumentNullException>(() => services.GetSingletonInstance<ITestSingletonService>());
        }

        #region ���Խӿں���
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

        [RegisterOneself]
        [ServiceRegister(ServiceLifetime.Singleton)]
        public class TestWithRegisterOneselfAttrImpl : ITestWithRegisterOneselfAttr
        {
            public TestWithRegisterOneselfAttrImpl()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; }

            public void Test() => Console.WriteLine("Hello TestWithRegisterAttribute!");
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
        #endregion
    }
}