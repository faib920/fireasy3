using Fireasy.Common.DependencyInjection;
using Fireasy.Common.DynamicProxy;
using Fireasy.Common.ObjectActivator;
using Fireasy.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fireasy.Common.Tests
{
    /// <summary>
    /// 对象创建测试
    /// </summary>
    [TestClass]
    public class ObjectActivatorTests : ServiceProviderBaseTests
    {
        /// <summary>
        /// 测试一般的对象创建
        /// </summary>
        [TestMethod]
        public void TestObjectActivate()
        {
            var objActivator = ServiceProvider.GetRequiredService<IObjectActivator>();
            var obj = objActivator.CreateInstance<TestServiceProxy1>(new TestService());
            Assert.IsNotNull(obj);
            var value = obj.Hello();
            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 测试一般的对象创建，通过 <see cref="IServiceProvider"/> 来获取
        /// </summary>
        [TestMethod]
        public void TestObjectActivateByServiceProvider()
        {
            var objActivator = ServiceProvider.GetRequiredService<IObjectActivator>();
            var obj = objActivator.CreateInstance<TestServiceProxy1>();
            Assert.IsNotNull(obj);
            var value = obj.Hello();
            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 测试动态代理对象创建
        /// </summary>
        [TestMethod]
        public void TestProxyObjectActivate()
        {
            var objActivator = ServiceProvider.GetRequiredService<IObjectActivator>();
            var obj = objActivator.CreateInstance<TestServiceProxy2>();
            Assert.IsNotNull(obj);
            var value = obj.Hello();
            Assert.AreEqual(1, value);
            Assert.AreNotEqual(typeof(TestServiceProxy2), obj.GetType());
        }

        public interface ITestService
        {
            int Hello();
        }

        public class TestService : ITestService, ITransientService
        {
            public int Hello()
            {
                return 1;
            }
        }

        /// <summary>
        /// 一般的代理服务，注入 <see cref="ITestService"/> 对象
        /// </summary>
        public class TestServiceProxy1
        {
            private readonly ITestService _service;

            public TestServiceProxy1(ITestService service)
            {
                _service = service;
            }

            public int Hello()
            {
                return _service.Hello();
            }
        }

        /// <summary>
        /// 使用动态代理的服务类，使用AOP拦截器
        /// </summary>
        public class TestServiceProxy2
        {
            private readonly ITestService _service;
            private readonly ILogger<TestServiceProxy2> _logger;

            public TestServiceProxy2(IServiceProvider serviceProvider, ITestService service, ILogger<TestServiceProxy2> logger, IObjectActivator objectActivator)
            {
                _service = service;
                _logger = logger;
            }

            [Intercept(typeof(TestInterceptor))]
            public virtual int Hello()
            {
                _logger.LogWarning("hello world");
                return _service.Hello();
            }

            public class TestInterceptor : IInterceptor
            {
                public void Initialize(InterceptContext context)
                {
                }

                public void Intercept(InterceptCallInfo info)
                {
                    Console.WriteLine(info.InterceptType);
                }
            }
        }
    }
}
