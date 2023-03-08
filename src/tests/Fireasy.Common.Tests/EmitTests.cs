using Fireasy.Common.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Tests
{
    [TestClass]
    public class EmitTests
    {
        [TestMethod]
        public void TestBuildAssembly()
        {
            var assemblyBuilder = new DynamicAssemblyBuilder("MyAssembly");

            var interfaceBuilder = assemblyBuilder.DefineInterface("MyInterface");
            var typeBuilder = assemblyBuilder.DefineType("MyClass");
            var enumBuilder = assemblyBuilder.DefineEnum("MyEnum");

            Assert.AreEqual("MyClass", typeBuilder.TypeBuilder.Name);
        }

        [TestMethod]
        public void TestBuildAssemblyWithModifier()
        {
            var assemblyBuilder = new DynamicAssemblyBuilder("MyAssembly");
            var typeBuilder1 = assemblyBuilder.DefineType("MyPrivateClass", Accessibility.Private);
            var typeBuilder2 = assemblyBuilder.DefineType("MyAbstractClass", modifier: Modifier.Abstract);

            Assert.IsFalse(typeBuilder1.TypeBuilder.IsPublic);
            Assert.IsTrue(typeBuilder2.TypeBuilder.IsAbstract);
        }

        [TestMethod]
        public void TestTypeBuilder()
        {
            var assemblyBuilder = new DynamicAssemblyBuilder("MyAssembly");
            var typeBuilder = assemblyBuilder.DefineType("MyClass");

            typeBuilder.BaseType = typeof(MyBaseClass);

            typeBuilder.ImplementInterface(typeof(IMyInterface));

            var methodBuilder = typeBuilder.DefineMethod("HelloWorld");
            var propertyBuilder = typeBuilder.DefineProperty("Title", typeof(string)).DefineGetSetMethods();

            methodBuilder = typeBuilder.DefineMethod("WriteName", typeof(string), new[] { typeof(string) });

            var type = typeBuilder.CreateType();

            Assert.IsTrue(typeof(IMyInterface).IsAssignableFrom(type));
        }

        [TestMethod]
        public void TestDefineGenericType()
        {
            var gt = new GtpType("T").SetBaseTypeConstraint(typeof(MyBaseClass));

            var assemblyBuilder = new DynamicAssemblyBuilder("MyAssembly");
            var typeBuilder = assemblyBuilder.DefineType("MyClass");
            typeBuilder.DefineGenericParameters(gt);

            typeBuilder.DefineConstructor(new Type[] { gt });

            var methodBuilder = typeBuilder.DefineMethod("Hello", gt, new Type[] { gt, new GtpType("TV") }, ilCoding: c =>
            {
                c.Emitter
                .ldarg_2.call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }))
                .ldarg_1.ret();
            });

            var type = typeBuilder.CreateType().MakeGenericType(typeof(MyBaseClass));
            var obj = Activator.CreateInstance(type, new MyBaseClass());

            var method = type.GetMethod("Hello").MakeGenericMethod(typeof(string));
            var value = method.Invoke(obj, new object[] { new MyBaseClass(), "world" });

            Assert.IsInstanceOfType(value, typeof(MyBaseClass));
        }

        [TestMethod()]
        public void TestDefineInterface()
        {
            var assemblyBuilder = new DynamicAssemblyBuilder("dynamicAssembly1");

            assemblyBuilder.DefineInterface("interface1");
        }

        [TestMethod()]
        public void TestDefineEnum()
        {
            var assemblyBuilder = new DynamicAssemblyBuilder("dynamicAssembly1");

            assemblyBuilder.DefineEnum("enum1");
        }

        private DynamicTypeBuilder CreateBuilder()
        {
            var assemblyBuilder = new DynamicAssemblyBuilder("assemblyTests");
            return assemblyBuilder.DefineType("testClass");
        }

        /// <summary>
        /// 测试CreateType方法。
        /// </summary>
        [TestMethod()]
        public void TestCreateType()
        {
            var typeBuilder = CreateBuilder();
            var type = typeBuilder.CreateType();

            Assert.IsNotNull(type);
        }

        /// <summary>
        /// 测试BaseType属性。
        /// </summary>
        [TestMethod()]
        public void TestBaseType()
        {
            var typeBuilder = CreateBuilder();
            typeBuilder.BaseType = typeof(DynamicBuilderBase);
            var type = typeBuilder.CreateType();

            Assert.AreEqual(typeof(DynamicBuilderBase), type.BaseType);
        }

        /// <summary>
        /// 测试ImplementInterface方法。
        /// </summary>
        [TestMethod()]
        public void ImplementInterface()
        {
            var typeBuilder = CreateBuilder();
            typeBuilder.ImplementInterface(typeof(IDynamicInterface));
            var type = typeBuilder.CreateType();

            Assert.IsTrue(typeof(IDynamicInterface).IsAssignableFrom(type));
        }

        /// <summary>
        /// 使用接口成员测试ImplementInterface方法。
        /// </summary>
        [TestMethod()]
        public void ImplementInterfaceWithMember()
        {
            var typeBuilder = CreateBuilder();
            typeBuilder.ImplementInterface(typeof(IDynamicPropertyInterface));
            typeBuilder.DefineProperty("Name", typeof(string)).DefineGetSetMethods();

            var type = typeBuilder.CreateType();

            Assert.IsTrue(typeof(IDynamicPropertyInterface).IsAssignableFrom(type));
            Assert.IsNotNull(type.GetProperty("Name"));
        }

        /// <summary>
        /// 使用接口成员显式实现测试ImplementInterface方法。
        /// </summary>
        [TestMethod()]
        public void ImplementInterfaceWithExplicitMember()
        {
            var typeBuilder = CreateBuilder();

            typeBuilder.ImplementInterface(typeof(IDynamicMethodInterface));
            var methodBuilder = typeBuilder.DefineMethod("Test",
                parameterTypes: new[] { typeof(int) },
                modifier: Modifier.ExplicitImpl,
                ilCoding: (e) => e.Emitter.ldstr("fireasy").call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) })).ret());

            methodBuilder.DefineParameter("s");

            var type = typeBuilder.CreateType();

            var obj = Activator.CreateInstance(type) as IDynamicMethodInterface;
            obj.Test(111);

            Assert.IsTrue(typeof(IDynamicMethodInterface).IsAssignableFrom(type));
        }

        /// <summary>
        /// 测试DefineProperty方法。
        /// </summary>
        [TestMethod()]
        public void TestDefineProperty()
        {
            var typeBuilder = CreateBuilder();

            typeBuilder.DefineProperty("Name", typeof(string)).DefineGetSetMethods();
            var type = typeBuilder.CreateType();

            Assert.IsNotNull(type.GetProperty("Name"));
        }

        /// <summary>
        /// 测试DefineMethod方法。
        /// </summary>
        [TestMethod()]
        public void TestDefineMethod()
        {
            var typeBuilder = CreateBuilder();

            typeBuilder.DefineMethod("HelloWorld");
            var type = typeBuilder.CreateType();

            Assert.IsNotNull(type.GetMethod("HelloWorld"));
        }

        /// <summary>
        /// 使用泛型参数测试DefineMethod方法。
        /// </summary>
        [TestMethod()]
        public void TestDefineGenericMethod()
        {
            var typeBuilder = CreateBuilder();

            // void Helo<T1, T2>(string name, T1 any1, T2 any2)
            var methodBuilder = typeBuilder.DefineMethod("Hello", parameterTypes: new Type[] { typeof(string), new GtpType("T1"), new GtpType("T2") });
            methodBuilder.DefineParameter("name");
            methodBuilder.DefineParameter("any1");
            methodBuilder.DefineParameter("any2");

            var paraCount = methodBuilder.ParameterTypes.Length;

            methodBuilder.OverwriteCode(e =>
            {
                e.ldc_i4(paraCount)
                .newarr(typeof(object))
                .dup.ldc_i4_0.ldarg_1.stelem_ref
                .For(1, paraCount, (e1, i) =>
                {
                    e1.dup.ldc_i4(i).ldarg(i + 1).box(methodBuilder.ParameterTypes[i]).stelem_ref.end();
                })
                .call(typeof(string).GetMethod("Concat", new[] { typeof(object[]) }))
                .call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }))
                .ret();
            });

            var type = typeBuilder.CreateType();

            var method = type.GetMethod("Hello");
            Assert.IsNotNull(method);
            Assert.IsTrue(method.IsGenericMethod);

            var obj = Activator.CreateInstance(type);

            method = method.MakeGenericMethod(typeof(int), typeof(decimal));

            method.Invoke(obj, new object[] { "fireasy", 22, 45m });
        }

        /// <summary>
        /// 使用泛型参数测试DefineMethod方法，有返回值。
        /// </summary>
        [TestMethod()]
        public void TestDefineGenericMethodWithReturnValue()
        {
            var typeBuilder = CreateBuilder();

            // T2 Helo<T1, T2>(string name, T1 any1, T2 any2)
            var methodBuilder = typeBuilder.DefineMethod("Hello", new GtpType("T2"), new Type[] { typeof(string), new GtpType("T1"), new GtpType("T2") });
            methodBuilder.DefineParameter("name");
            methodBuilder.DefineParameter("any1");
            methodBuilder.DefineParameter("any2");

            var paraCount = methodBuilder.ParameterTypes.Length;

            methodBuilder.OverwriteCode(e =>
            {
                e.ldc_i4(paraCount)
                .newarr(typeof(object))
                .dup.ldc_i4_0.ldarg_1.stelem_ref
                .For(1, paraCount, (e1, i) =>
                {
                    e1.dup.ldc_i4(i).ldarg(i + 1).box(methodBuilder.ParameterTypes[i]).stelem_ref.end();
                })
                .call(typeof(string).GetMethod("Concat", new[] { typeof(object[]) }))
                .call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }))
                .ldarg_3
                .ret();
            });

            var type = typeBuilder.CreateType();

            var method = type.GetMethod("Hello");
            Assert.IsNotNull(method);
            Assert.IsTrue(method.IsGenericMethod);

            var obj = Activator.CreateInstance(type);

            method = method.MakeGenericMethod(typeof(int), typeof(decimal));

            var ret = method.Invoke(obj, new object[] { "fireasy", 22, 45m });
            Assert.AreEqual(45m, ret);
        }

        /// <summary>
        /// 使用泛型参数测试DefineMethod方法，有返回值。
        /// </summary>
        [TestMethod()]
        public void TestDefineGenericMethodWithBaseType()
        {
            var assemblyBuilder = new DynamicAssemblyBuilder("assemblyTests");
            var typeBuilder = assemblyBuilder.DefineType("testClass", baseType: typeof(GenericMethodClass));

            // T2 Helo<T1, T2>(string name, T1 any1, T2 any2)
            var methodBuilder = typeBuilder.DefineMethod("Hello", new GtpType("T2"), new Type[] { typeof(string), new GtpType("T1").SetBaseTypeConstraint(typeof(GenericMethodClass)), new GtpType("T2") }, ilCoding: (context) =>
            {
                context.Emitter
                    .ldarg_0
                    .ldarg_1
                    .ldarg_2
                    .ldarg_3
                    .call(typeBuilder.BaseType.GetMethod("Hello"))
                    .ret();
            });

            methodBuilder.DefineParameter("name");
            methodBuilder.DefineParameter("any1");
            methodBuilder.DefineParameter("any2");

            var paraCount = methodBuilder.ParameterTypes.Length;

            var type = typeBuilder.CreateType();

            var method = type.GetMethod("Hello");
            Assert.IsNotNull(method);
            Assert.IsTrue(method.IsGenericMethod);

            var obj = Activator.CreateInstance(type);

            method = method.MakeGenericMethod(typeof(GenericMethodClass), typeof(decimal));

            var ret = method.Invoke(obj, new object[] { "fireasy", new GenericMethodClass(), 45m });
            Assert.AreEqual(45m, ret);
        }

        /// <summary>
        /// 使用重写抽象类方法测试DefineMethod方法。
        /// </summary>
        [TestMethod()]
        public void TestDefineOverrideMethod()
        {
            var typeBuilder = CreateBuilder();
            typeBuilder.BaseType = typeof(DynamicBuilderBase);

            typeBuilder.DefineMethod("Hello", parameterTypes: new Type[] { typeof(string) }, ilCoding: (context) =>
            {
                context.Emitter
                    .ldarg_0
                    .ldarg_1
                    .call(typeBuilder.BaseType.GetMethod("Hello"))
                    .ret();
            });
            var type = typeBuilder.CreateType();

            var method = type.GetMethod("Hello");
            Assert.IsNotNull(method);
            var obj = Activator.CreateInstance(type);
            method.Invoke(obj, new object[] { "fireasy" });
        }

        /// <summary>
        /// 测试DefineConstructor方法。
        /// </summary>
        [TestMethod()]
        public void TestDefineConstructor()
        {
            var typeBuilder = CreateBuilder();

            var c = typeBuilder.DefineConstructor(new Type[] { typeof(string), typeof(string) });

            c.DefineParameter("name").DefineParameter("tt", "bbb");

            c.OverwriteCode(e =>
                e.ldarg_1.ldarg_2
                    .call(typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }))
                    .call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }))
                    .ret()
            );

            var type = typeBuilder.CreateType();


            type.GetConstructors().FirstOrDefault().Invoke(new[] { "fireasy", null });
        }

        /// <summary>
        /// 测试DefineField方法。
        /// </summary>
        [TestMethod()]
        public void TestDefineField()
        {
            var typeBuilder = CreateBuilder();

            typeBuilder.DefineField("Name", typeof(string));
            var type = typeBuilder.CreateType();

            Assert.IsNotNull(type.GetField("Name", BindingFlags.NonPublic | BindingFlags.Instance));
        }

        /// <summary>
        /// 使用公有特性测试DefineField方法。
        /// </summary>
        [TestMethod()]
        public void TestDefineFieldWithPublic()
        {
            var typeBuilder = CreateBuilder();

            typeBuilder.DefineField("Name", typeof(string), "fireasy", Accessibility.Public);
            var type = typeBuilder.CreateType();

            Assert.IsNotNull(type.GetField("Name"));
        }

        /// <summary>
        /// 测试DefineNestedType方法。
        /// </summary>
        [TestMethod()]
        public void TestDefineNestedType()
        {
            var typeBuilder = CreateBuilder();

            var nestedType = typeBuilder.DefineNestedType("nestedClass");
            var type = typeBuilder.CreateType();

            Assert.IsNotNull(type.GetNestedType("nestedClass", BindingFlags.NonPublic));
        }

        [TestMethod()]
        public void TestDefineParameter()
        {
            // Hello(string name);
            var typeBuilder = CreateBuilder();
            var methodBuilder = typeBuilder.DefineMethod("Hello", parameterTypes: new Type[] { typeof(string) });

            methodBuilder.DefineParameter("name");
            typeBuilder.CreateType();

            Assert.AreEqual("name", methodBuilder.MethodBuilder.GetParameters()[0].Name);
        }

        [TestMethod()]
        public void TestDefineParameterRef()
        {
            // Hello(ref string name);
            var typeBuilder = CreateBuilder();
            var methodBuilder = typeBuilder.DefineMethod("Hello", parameterTypes: new Type[] { typeof(string) });

            methodBuilder.DefineParameter("name", true);
            typeBuilder.CreateType();

            Assert.IsTrue(methodBuilder.MethodBuilder.GetParameters()[0].IsOut);
        }

        [TestMethod()]
        public void TestDefineParameterOptional()
        {
            // Hello(string name = "fireasy");
            var typeBuilder = CreateBuilder();
            var methodBuilder = typeBuilder.DefineMethod("Hello", parameterTypes: new Type[] { typeof(string) });

            methodBuilder.DefineParameter("name", hasDefaultValue: true, defaultValue: "fireasy");
            typeBuilder.CreateType();

            Assert.AreEqual("fireasy", methodBuilder.MethodBuilder.GetParameters()[0].DefaultValue);
        }

        [TestMethod()]
        public void TestAppendCode()
        {
            var typeBuilder = CreateBuilder();
            var writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            var methodBuilder = typeBuilder.DefineMethod("Hello", modifier: Modifier.Static, ilCoding: context => { });

            methodBuilder.AppendCode(e => e.ldstr("Hello fireasy!").call(writeLineMethod));
            methodBuilder.AppendCode(e => e.ldstr("Hello world!").call(writeLineMethod).ret());

            var type = typeBuilder.CreateType();

            type.GetMethod("Hello").Invoke(null, null);
        }

        [TestMethod()]
        public void TestOverwriteCode()
        {
            var typeBuilder = CreateBuilder();
            var writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            var methodBuilder = typeBuilder.DefineMethod("Hello", modifier: Modifier.Static, ilCoding: context =>
                context.Emitter.ldstr("Hello fireasy").call(writeLineMethod).ret());

            methodBuilder.OverwriteCode(e => e.ldstr("Hello world!").call(writeLineMethod).ret());

            var type = typeBuilder.CreateType();

            type.GetMethod("Hello").Invoke(null, null);
        }

        [TestMethod()]
        public void TestDefineGetSetMethods()
        {
            var typeBuilder = CreateBuilder();
            var propertyBuilder = typeBuilder.DefineProperty("Name", typeof(string));
            propertyBuilder.DefineGetSetMethods();

            Assert.IsTrue(propertyBuilder.PropertyBuilder.CanRead);
            Assert.IsTrue(propertyBuilder.PropertyBuilder.CanWrite);
        }

        [TestMethod()]
        public void TestDefineGetMethod()
        {
            var typeBuilder = CreateBuilder();
            var propertyBuilder = typeBuilder.DefineProperty("Name", typeof(string));
            propertyBuilder.DefineGetMethod();

            Assert.IsTrue(propertyBuilder.PropertyBuilder.CanRead);
            Assert.IsFalse(propertyBuilder.PropertyBuilder.CanWrite);
        }

        [TestMethod()]
        public void TestDefineGetMethodByField()
        {
            var typeBuilder = CreateBuilder();
            var propertyBuilder = typeBuilder.DefineProperty("Name", typeof(string));

            propertyBuilder.DefineGetMethodByField();

            Assert.IsTrue(propertyBuilder.PropertyBuilder.CanRead);
            Assert.IsFalse(propertyBuilder.PropertyBuilder.CanWrite);
        }

        [TestMethod()]
        public void TestDefineSetMethod()
        {
            var typeBuilder = CreateBuilder();
            var propertyBuilder = typeBuilder.DefineProperty("Name", typeof(string));
            propertyBuilder.DefineSetMethod();

            Assert.IsFalse(propertyBuilder.PropertyBuilder.CanRead);
            Assert.IsTrue(propertyBuilder.PropertyBuilder.CanWrite);
        }

        [TestMethod()]
        public void TestDefineSetMethodByField()
        {
            var typeBuilder = CreateBuilder();
            var propertyBuilder = typeBuilder.DefineProperty("Name", typeof(string));

            propertyBuilder.DefineSetMethodByField();

            Assert.IsFalse(propertyBuilder.PropertyBuilder.CanRead);
            Assert.IsTrue(propertyBuilder.PropertyBuilder.CanWrite);
        }
    }

    public interface IMyInterface
    {
        void HelloWorld();

        string Title { get; set; }
    }

    public class MyBaseClass
    {
        public virtual string GetName(string name)
        {
            return name;
        }
    }

    public interface IDynamicInterface
    {
    }

    public interface IDynamicPropertyInterface
    {
        string Name { get; set; }
    }

    public interface IDynamicMethodInterface
    {
        void Test(int s);
    }

    public class DynamicBuilderBase
    {
        public virtual void Hello(string name)
        {
            Console.WriteLine("Hello " + name);
        }
    }

    public class GenericMethodClass
    {
        public virtual T2 Hello<T1, T2>(string name, T1 any1, T2 any2) where T1 : GenericMethodClass
        {
            return any2;
        }
    }
}
