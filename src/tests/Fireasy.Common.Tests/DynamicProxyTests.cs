using Fireasy.Common.DynamicProxy;
using Fireasy.Tests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Fireasy.Common.Tests
{
    [TestClass]
    public class DynamicProxyTests : ServiceProviderBaseTests
    {
        /// <summary>
        /// 测试获取代理类
        /// </summary>
        [TestMethod]
        public void TestGetProxyType()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyType = factory!.GetProxyType(typeof(TestProxy));

            Assert.AreNotEqual(typeof(TestProxy), proxyType);
        }

        /// <summary>
        /// 测试拦截 GetString
        /// </summary>
        [TestMethod]
        public void TestGetString()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyObj = factory!.BuildProxy<TestProxy>();

            var value = proxyObj!.GetString();
            Assert.AreEqual("hello world", value);
        }

        /// <summary>
        /// 测试拦截 GetStringCancel
        /// </summary>
        [TestMethod]
        public void TestGetStringCancel()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyObj = factory!.BuildProxy<TestProxy>();

            //被拦截取消了，不会返回该返回的 fireasy
            var value = proxyObj!.GetStringCancel();
            Assert.AreNotEqual("fireasy", value);
        }

        /// <summary>
        /// 测试拦截 TestArguments
        /// </summary>
        [TestMethod]
        public void TestArguments()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyObj = factory!.BuildProxy<TestProxy>();

            var value = proxyObj!.TestArguments(100, "fireasy", DateTime.Now);
            Assert.AreEqual("fireasy", value);
        }

        /// <summary>
        /// 测试拦截 GetString
        /// </summary>
        [TestMethod]
        public async Task TestGetStringAsync()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyObj = factory!.BuildProxy<TestProxy>();

            var value = await proxyObj!.GetStringAsync();
            Assert.AreEqual("hello world", value);
        }

        /// <summary>
        /// 测试拦截 TestIgnoreThrowException
        /// </summary>
        [TestMethod]
        public void TestIgnoreThrowException()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyObj = factory!.BuildProxy<TestProxy>();

            //忽略方法抛出的异常
            var value = proxyObj!.IgnoreThrowException();
            Assert.AreEqual(null, value);
        }

        /// <summary>
        /// 测试拦截 TestGeneric
        /// </summary>
        [TestMethod]
        public void TestGeneric()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyObj = factory!.BuildProxy<TestProxy>();

            //忽略方法抛出的异常
            var value = proxyObj!.TestGeneric<int, string>(100);
            Assert.AreNotEqual(null, value);
        }

        /// <summary>
        /// 测试拦截 TestRef
        /// </summary>
        [TestMethod]
        public void TestRef()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyObj = factory!.BuildProxy<TestProxy>();

            //忽略方法抛出的异常
            var a = 0;
            var b = string.Empty;
            var value = proxyObj!.TestRef(ref a, ref b);
            Assert.AreNotEqual(null, value);
        }

        /// <summary>
        /// 测试拦截 TestRef
        /// </summary>
        [TestMethod]
        public void TestOut()
        {
            var factory = ServiceProvider.GetService<IDynamicProxyFactory>();
            var proxyObj = factory!.BuildProxy<TestProxy>();

            //忽略方法抛出的异常
            var value = proxyObj!.TestOut(out int a, out string b);
            Assert.AreNotEqual(null, value);
        }

        [Intercept(typeof(ClassAllInterceptor))]
        public class TestProxy
        {
            public TestProxy(IServiceProvider serviceProvider)
            {
            }

            /// <summary>
            /// 字符串
            /// </summary>
            /// <returns></returns>
            [Intercept(typeof(GetStringInterceptor))]
            public virtual string GetString()
            {
                return string.Empty;
            }

            /// <summary>
            /// 字符串，取消
            /// </summary>
            /// <returns></returns>
            [Intercept(typeof(CancelInterceptor))]
            public virtual string GetStringCancel()
            {
                return "fireasy";
            }

            /// <summary>
            /// 整型
            /// </summary>
            /// <returns></returns>
            public virtual int GetInteger()
            {
                return 0;
            }

            /// <summary>
            /// 有参数
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="c"></param>
            /// <returns></returns>
            public virtual string TestArguments(int a, string b, DateTime c)
            {
                return b;
            }

            /// <summary>
            /// ref
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public virtual string TestRef(ref int a, ref string b)
            {
                return b;
            }

            /// <summary>
            /// ref
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public string TestOut(out int a, out string b)
            {
                a = 1;
                b = string.Empty;
                return string.Empty;
            }

            /// <summary>
            /// 返回泛型类型
            /// </summary>
            /// <returns></returns>
            public virtual Dictionary<string, int> TestReturnGeneric(List<string> list)
            {
                return new Dictionary<string, int>();
            }

            /// <summary>
            /// 返回泛型方法
            /// </summary>
            /// <returns></returns>
            public virtual List<TValue> TestGeneric<T, TValue>(T value)
            {
                return new List<TValue>();
            }

            /// <summary>
            /// 异步方法
            /// </summary>
            /// <returns></returns>
            [Intercept(typeof(GetStringAsyncInterceptor))]
            public virtual Task<string> GetStringAsync()
            {
                return Task.FromResult(string.Empty);
            }

            /// <summary>
            /// 忽略抛出异常
            /// </summary>
            /// <returns></returns>
            [IgnoreThrowException]
            public virtual string IgnoreThrowException()
            {
                throw new Exception("抛错了");
            }

            /// <summary>
            /// 属性
            /// </summary>
            public virtual string Address { get; set; }
        }

        public class ClassAllInterceptor : IInterceptor
        {
            public void Initialize(InterceptContext context)
            {
            }

            public void Intercept(InterceptCallInfo info)
            {
                Console.WriteLine(info.InterceptType);
            }
        }

        public class GetStringInterceptor : IInterceptor
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

        public class CancelInterceptor : IInterceptor
        {
            public void Initialize(InterceptContext context)
            {
            }

            public void Intercept(InterceptCallInfo info)
            {
                if (info.InterceptType == InterceptType.BeforeGetValue || 
                    info.InterceptType == InterceptType.BeforeSetValue || 
                    info.InterceptType == InterceptType.BeforeMethodCall)
                {
                    info.Cancel = true;
                }
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

                Console.WriteLine(info.InterceptType);

                return ValueTask.CompletedTask;
            }
        }

    }
}
