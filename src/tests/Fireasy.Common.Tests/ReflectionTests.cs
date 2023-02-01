using Fireasy.Common.Reflection;
using Fireasy.Tests.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fireasy.Common.Tests
{
    /// <summary>
    /// 反射测试
    /// </summary>
    [TestClass]
    public class ReflectionTests : ServiceProviderBaseTests
    {
        const int LOOP_TIME = 10000000;

        /// <summary>
        /// 测试属性访问器
        /// </summary>
        [TestMethod]
        public void TestPropertyAccessor()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var property = typeof(OneObject).GetTypeInfo().GetDeclaredProperty(nameof(OneObject.Value));
            var accessor = reflectionFactory!.GetAccessor<int>(property!);

            accessor.SetValue(obj, 100);
            var value = accessor.GetValue(obj);

            Assert.AreEqual(100, value);
        }

        /// <summary>
        /// 测试属性访问器(静态属性)
        /// </summary>
        [TestMethod]
        public void TestPropertyAccessorForStatic()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var property = typeof(OneObject).GetTypeInfo().GetDeclaredProperty(nameof(OneObject.StaticValue));
            var accessor = reflectionFactory!.GetAccessor<int>(property!);

            accessor.SetValue(null, 100);
            var value = accessor.GetValue(null);

            Assert.AreEqual(100, value);
        }

        /// <summary>
        /// 测试属性访问器(静态属性)
        /// </summary>
        [TestMethod]
        public void TestPropertyAccessorForNullable()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var property = typeof(OneObject).GetTypeInfo().GetDeclaredProperty(nameof(OneObject.NullableValue));
            var accessor = reflectionFactory!.GetAccessor<int?>(property!);

            accessor.SetValue(obj, 100);
            var value = accessor.GetValue(obj);

            Assert.AreEqual(100, value);
        }

        /// <summary>
        /// 测试属性访问器(引用类型)
        /// </summary>
        [TestMethod]
        public void TestPropertyAccessorForRef()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var property = typeof(OneObject).GetTypeInfo().GetDeclaredProperty(nameof(OneObject.Two));
            var accessor = reflectionFactory!.GetAccessor<TwoObject>(property!);

            var three = new ThreeObject();
            accessor.SetValue(obj, three);
            var value = accessor.GetValue(obj);

            Assert.AreEqual(three, value);
        }

        /// <summary>
        /// 测试字段访问器
        /// </summary>
        [TestMethod]
        public void TestFieldAccessor()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var field = typeof(OneObject).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessor = reflectionFactory!.GetAccessor<int>(field!);

            accessor.SetValue(obj, 100);
            var value = accessor.GetValue(obj);

            Assert.AreEqual(100, value);
        }

        /// <summary>
        /// 测试字段访问器(静态字段)
        /// </summary>
        [TestMethod]
        public void TestFieldAccessorForStatic()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var field = typeof(OneObject).GetField("_staticValue", BindingFlags.NonPublic | BindingFlags.Static);
            var accessor = reflectionFactory!.GetAccessor<int>(field!);

            accessor.SetValue(null, 100);
            var value = accessor.GetValue(null);

            Assert.AreEqual(100, value);
        }

        /// <summary>
        /// 循环设置属性值，使用访问器
        /// </summary>
        [TestMethod]
        public void TestLoopSetPropertyValueByAccessor()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();
            var property = typeof(OneObject).GetTypeInfo().GetDeclaredProperty(nameof(OneObject.Value));
            var accessor = reflectionFactory!.GetAccessor<int>(property!);

            for (var i = 0; i < LOOP_TIME; i++)
            {
                var obj = new OneObject();
                accessor.SetValue(obj, 100);
            }
        }

        /// <summary>
        /// 循环设置属性值，使用反射
        /// </summary>
        [TestMethod]
        public void TestLoopSetPropertyValueByReflection()
        {
            var property = typeof(OneObject).GetTypeInfo().GetDeclaredProperty(nameof(OneObject.Value));

            for (var i = 0; i < LOOP_TIME; i++)
            {
                var obj = new OneObject();
                property!.SetValue(obj, 100);
            }
        }

        /// <summary>
        /// 循环设置字段值，使用访问器
        /// </summary>
        [TestMethod]
        public void TestLoopSetFieldValueByAccessor()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();
            var field = typeof(OneObject).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessor = reflectionFactory!.GetAccessor<int>(field!);

            for (var i = 0; i < LOOP_TIME; i++)
            {
                var obj = new OneObject();
                accessor.SetValue(obj, 100);
            }
        }

        /// <summary>
        /// 循环设置字段值，使用反射
        /// </summary>
        [TestMethod]
        public void TestLoopSetFieldValueByReflection()
        {
            var field = typeof(OneObject).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);

            for (var i = 0; i < LOOP_TIME; i++)
            {
                var obj = new OneObject();
                field!.SetValue(obj, 100);
            }
        }

        /// <summary>
        /// 测试调用方法
        /// </summary>
        [TestMethod]
        public void TestInvokeMethod()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoSomething1));
            var invoker = reflectionFactory!.GetInvoker(method!);

            var value = invoker.Invoke(obj, 1);

            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 测试调用方法(泛型执行器)
        /// </summary>
        [TestMethod]
        public void TestInvokeMethodForGeneric()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoSomething1));
            var invoker = reflectionFactory!.GetInvoker<int>(method!);

            var value = invoker.Invoke(obj, 1);

            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 测试调用方法(参数1)
        /// </summary>
        [TestMethod]
        public void TestInvokeMethodForArg1()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoSomething1));
            var invoker = reflectionFactory!.GetInvoker<int, int>(method!);

            var value = invoker.Invoke(obj, 1);

            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 测试调用方法(参数2)
        /// </summary>
        [TestMethod]
        public void TestInvokeMethodForArg2()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoSomething2));
            var invoker = reflectionFactory!.GetInvoker<int, string, int>(method!);

            var value = invoker.Invoke(obj, 1, "fireasy");

            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 测试调用方法(无返回值)
        /// </summary>
        [TestMethod]
        public void TestInvokeMethodWithoutReturn()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.Hello));
            var invoker = reflectionFactory!.GetInvoker(method!);

            var value = invoker.Invoke(obj);

            Assert.IsNull(value);
        }

        /// <summary>
        /// 测试调用方法(无返回值)
        /// </summary>
        [TestMethod]
        public void TestInvokeMethodWithoutReturnHasArgs()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.HelloArg1));
            var invoker = reflectionFactory!.GetInvoker<int, object>(method!);

            var value = invoker.Invoke(obj, 1);

            Assert.IsNull(value);
        }

        /// <summary>
        /// 测试调用方法(静态方法)
        /// </summary>
        [TestMethod]
        public void TestInvokeMethodForStatic()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetMethod(nameof(OneObject.StaticHello), BindingFlags.Static | BindingFlags.Public);
            var invoker = reflectionFactory!.GetInvoker(method!);

            var value = invoker.Invoke(null);

            Assert.IsNull(value);
        }

        /// <summary>
        /// 测试调用方法(out)
        /// </summary>
        [TestMethod]
        public void TestInvokeMethodForOut()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoOut));
            var invoker = reflectionFactory!.GetInvoker<string, bool>(method!);

            var str = string.Empty;
            var value = invoker.Invoke(obj, str);

            Assert.IsTrue(value);
            Assert.AreEqual("fireasy", str);
        }

        /// <summary>
        /// 测试调用方法(ref)
        /// </summary>
        [TestMethod]
        public void TestInvokeMethodForRef()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoRef));
            var invoker = reflectionFactory!.GetInvoker<string, bool>(method!);

            var str = string.Empty;
            var value = invoker.Invoke(obj, str);

            Assert.IsTrue(value);
            Assert.AreEqual("fireasy", str);
        }

        /// <summary>
        /// 测试调用方法
        /// </summary>
        [TestMethod]
        public async Task TestInvokeAsyncMethod()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoSomething1Async));
            var invoker = reflectionFactory!.GetAsyncInvoker<int, int>(method!);

            var value = await invoker.InvokeAsync(obj, 1);

            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 测试调用方法
        /// </summary>
        [TestMethod]
        public async Task TestInvokeStaticAsyncMethod()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoStaticSomething1Async));
            var invoker = reflectionFactory!.GetAsyncInvoker<int, int>(method!);

            var value = await invoker.InvokeAsync(null, 1);

            Assert.AreEqual(1, value);
        }

        /// <summary>
        /// 测试调用异步方法(无返回值)
        /// </summary>
        [TestMethod]
        public async Task TestInvokeAsyncMethodWithoutReturn()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.HelloAsync));
            var invoker = reflectionFactory!.GetAsyncInvoker(method!);

            var value = await invoker.InvokeAsync(obj);

            Assert.IsNull(value);
        }

        /// <summary>
        /// 测试调用方法(无返回值)
        /// </summary>
        [TestMethod]
        public async Task TestInvokeAsyncMethodWithoutReturnHasArgs()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var obj = new OneObject();
            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.HelloArg1Async));
            var invoker = reflectionFactory!.GetAsyncInvoker<int, object>(method!);

            var value = await invoker.InvokeAsync(obj, 1);

            Assert.IsNull(value);
        }

        /// <summary>
        /// 循环调用方法
        /// </summary>
        [TestMethod]
        public void TestLoopInvokeMethodByInvoker()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoSomething2));
            var invoker = reflectionFactory!.GetInvoker<int, string, int>(method!);

            for (var i = 0; i < LOOP_TIME; i++)
            {
                var obj = new OneObject();
                invoker.Invoke(obj, 1, "fireasy");
            }
        }

        /// <summary>
        /// 循环调用方法
        /// </summary>
        [TestMethod]
        public void TestLoopInvokeMethodByRefection()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var method = typeof(OneObject).GetTypeInfo().GetDeclaredMethod(nameof(OneObject.DoSomething2));

            for (var i = 0; i < LOOP_TIME; i++)
            {
                var obj = new OneObject();
                method.Invoke(obj, new object[] { 1, "fireasy" });
            }
        }

        /// <summary>
        /// 测试调用构造函数
        /// </summary>
        [TestMethod]
        public void TestInvokeConstructor()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var constructor = typeof(OneObject).GetConstructors().FirstOrDefault(s => s.GetParameters().Length == 3);
            var invoker = reflectionFactory!.GetInvoker(constructor!);

            var obj = (OneObject)invoker.Invoke(100, 1, new TwoObject());

            Assert.AreEqual(100, obj.Value);
        }

        /// <summary>
        /// 测试调用构造函数(参数3)
        /// </summary>
        [TestMethod]
        public void TestInvokeConstructorForArg3()
        {
            var reflectionFactory = ServiceProvider.GetService<IReflectionFactory>();

            var constructor = typeof(OneObject).GetConstructors().FirstOrDefault(s => s.GetParameters().Length == 3);
            var invoker = reflectionFactory!.GetInvoker<int, int?, TwoObject>(constructor!);

            var obj = (OneObject)invoker.Invoke(100, 1, new TwoObject());

            Assert.AreEqual(100, obj.Value);
        }

        private class OneObject
        {
            private int _value;

            private static int _staticValue;

            public OneObject()
            {
            }

            public OneObject(int value, int? nullableValue, TwoObject two)
            {
                Value = value;
                NullableValue = nullableValue;
                Two = two;
            }

            public object New(params object[] objj)
            {
                return new OneObject((int)objj[0], (int?)objj[1], (TwoObject)objj[2]);
            }

            public int Value { get; set; }

            public int? NullableValue { get; set; }

            public static int StaticValue { get; set; }

            public TwoObject Two { get; set; }

            public int DoSomething1(int value)
            {
                return value;
            }

            public Task<int> DoSomething1Async(int value)
            {
                return Task.FromResult(value);
            }

            public static Task<int> DoStaticSomething1Async(int value)
            {
                return Task.FromResult(value);
            }

            public int DoSomething2(int value, string str)
            {
                return value;
            }

            public Task<int> DoSomething2Async(int value, string str)
            {
                return Task.FromResult(value);
            }

            public int DoSomething3(int value, string str, TwoObject two)
            {
                return value;
            }

            public Task<int> DoSomething3Async(int value, string str, TwoObject two)
            {
                return Task.FromResult(value);
            }

            public bool DoRef(ref string str)
            {
                str = "fireasy";
                return true;
            }

            public bool DoOut(out string str)
            {
                str = "fireasy";
                return true;
            }

            public void Hello()
            {
                Console.WriteLine("hello world");
            }

            public Task HelloAsync()
            {
                return Task.CompletedTask;
            }

            public void HelloArg1(int value)
            {
                Console.WriteLine("hello world");
            }

            public Task HelloArg1Async(int value)
            {
                return Task.CompletedTask;
            }

            public static void StaticHello()
            {
                Console.WriteLine("hello world");
            }
        }

        public class TwoObject
        {
        }

        public class ThreeObject : TwoObject
        {
        }
    }
}
