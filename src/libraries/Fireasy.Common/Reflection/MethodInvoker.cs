// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Emit;
using System.Reflection;
using System.Reflection.Emit;

namespace Fireasy.Common.Reflection
{
    /// <summary>
    /// 包装 <see cref="MethodInfo"/> 对象，创建一个委托来提升方法的执行。
    /// </summary>
    public class MethodInvoker : IMethodInvoker
    {
        private readonly Func<object?, object[], object?> _invoker;
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public MethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, object[], object?> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(object),
                new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType, true);

            _parameters = methodInfo.GetParameters();
            var returnType = methodInfo.ReturnType;

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .For(0, _parameters.Length, (e, i) => e.ldarg_1.ldc_i4(i).ldelem_ref.If(_parameters[i].ParameterType.IsValueType, f => f.unbox_any(_parameters[i].ParameterType), f => f.castclass(_parameters[i].ParameterType)))
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .If(returnType != typeof(void), e => e.box(returnType), e => e.ldnull.end())
            .ret();

            return (Func<object?, object[], object?>)dm.CreateDelegate(typeof(Func<object, object[], object>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="parameters">方法的参数。</param>
        /// <returns></returns>
        public object? Invoke(object? instance, params object[] parameters)
        {
            if (parameters.Length != _parameters.Length)
            {
                throw new Exception($"参数个数不匹配，应为 {_parameters.Length} 个。");
            }

            return _invoker(instance, parameters);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class MethodInvoker<TValue> : IMethodInvoker<TValue>, IMethodInvoker
    {
        private readonly Func<object?, object[], TValue?> _invoker;
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public MethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, object[], TValue?> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(void) && methodInfo.ReturnType != typeof(TValue))
            {
                throw new ArgumentException($"方法 {methodInfo.Name} 的返回类型必须与 TValue 一致。");
            }

            _parameters = methodInfo.GetParameters();

            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(TValue),
                new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .For(0, _parameters.Length, (e, i) => e.ldarg_1.ldc_i4(i).ldelem_ref.If(_parameters[i].ParameterType.IsValueType, f => f.unbox_any(_parameters[i].ParameterType), f => f.castclass(_parameters[i].ParameterType)))
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .If(methodInfo.ReturnType == typeof(void), e => e.Default(typeof(TValue)))
            .ret();

            return (Func<object?, object[], TValue?>)dm.CreateDelegate(typeof(Func<object, object[], TValue>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="parameters">方法的参数。</param>
        /// <returns></returns>
        public TValue? Invoke(object? instance, params object[] parameters)
        {
            if (parameters.Length != _parameters.Length)
            {
                throw new Exception($"参数个数不匹配，应为 {_parameters.Length} 个。");
            }

            return _invoker(instance, parameters);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return Invoke(instance, parameters);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    public class MethodInvoker<TArg1, TValue> : IMethodInvoker<TArg1, TValue>, IMethodInvoker
    {
        private readonly Func<object?, TArg1?, TValue?> _invoker;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public MethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, TArg1?, TValue?> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(void) && methodInfo.ReturnType != typeof(TValue))
            {
                throw new ArgumentException($"方法 {methodInfo.Name} 的返回类型必须与 TValue 一致。");
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 1)
            {
                throw new Exception($"参数个数不匹配，应为 1 个。");
            }

            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(TValue),
                new Type[] { typeof(object), typeof(TArg1) }, methodInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .ldarg_1
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .If(methodInfo.ReturnType == typeof(void), e => e.Default(typeof(TValue)))
            .ret();

            return (Func<object?, TArg1?, TValue?>)dm.CreateDelegate(typeof(Func<object, TArg1, TValue>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">方法的参数。</param>
        /// <returns></returns>
        public TValue? Invoke(object? instance, TArg1? arg1)
        {
            return _invoker(instance, arg1);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return Invoke(instance, (TArg1)parameters[0]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    public class MethodInvoker<TArg1, TArg2, TValue> : IMethodInvoker<TArg1, TArg2, TValue>, IMethodInvoker
    {
        private readonly Func<object?, TArg1?, TArg2?, TValue?> _invoker;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public MethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, TArg1?, TArg2?, TValue?> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(void) && methodInfo.ReturnType != typeof(TValue))
            {
                throw new ArgumentException($"方法 {methodInfo.Name} 的返回类型必须与 TValue 一致。");
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 2)
            {
                throw new Exception($"参数个数不匹配，应为 2 个。");
            }

            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(TValue),
                new Type[] { typeof(object), typeof(TArg1), typeof(TArg2) }, methodInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .ldarg_1.ldarg_2
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .If(methodInfo.ReturnType == typeof(void), e => e.Default(typeof(TValue)))
            .ret();

            return (Func<object?, TArg1?, TArg2?, TValue?>)dm.CreateDelegate(typeof(Func<object, TArg1, TArg2, TValue>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">方法的参数。</param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public TValue? Invoke(object? instance, TArg1? arg1, TArg2? arg2)
        {
            return _invoker(instance, arg1, arg2);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return Invoke(instance, (TArg1)parameters[0], (TArg2)parameters[1]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    public class MethodInvoker<TArg1, TArg2, TArg3, TValue> : IMethodInvoker<TArg1, TArg2, TArg3, TValue>, IMethodInvoker
    {
        private readonly Func<object?, TArg1?, TArg2?, TArg3?, TValue?> _invoker;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public MethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, TArg1?, TArg2?, TArg3?, TValue?> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(void) && methodInfo.ReturnType != typeof(TValue))
            {
                throw new ArgumentException($"方法 {methodInfo.Name} 的返回类型必须与 TValue 一致。");
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 3)
            {
                throw new Exception($"参数个数不匹配，应为 3 个。");
            }

            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(TValue),
                new Type[] { typeof(object), typeof(TArg1), typeof(TArg2), typeof(TArg3) }, methodInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .ldarg_1.ldarg_2.ldarg_3
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .If(methodInfo.ReturnType == typeof(void), e => e.Default(typeof(TValue)))
            .ret();

            return (Func<object?, TArg1?, TArg2?, TArg3?, TValue?>)dm.CreateDelegate(typeof(Func<object, TArg1, TArg2, TArg3?, TValue>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">方法的参数。</param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public TValue? Invoke(object? instance, TArg1? arg1, TArg2? arg2, TArg3? arg3)
        {
            return _invoker(instance, arg1, arg2, arg3);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return Invoke(instance, (TArg1)parameters[0], (TArg2)parameters[1], (TArg3)parameters[2]);
        }
    }

    /// <summary>
    /// 包装 <see cref="MethodInfo"/> 对象，创建一个委托来提升方法的执行。
    /// </summary>
    public class AsyncMethodInvoker : IAsyncMethodInvoker, IMethodInvoker
    {
        private readonly Func<object?, object[], Task<object?>> _invoker;
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public AsyncMethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, object[], Task<object?>> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(Task<object>),
                new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType, true);

            _parameters = methodInfo.GetParameters();
            var returnType = methodInfo.ReturnType;

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .For(0, _parameters.Length, (e, i) => e.ldarg_1.ldc_i4(i).ldelem_ref.If(_parameters[i].ParameterType.IsValueType, f => f.unbox_any(_parameters[i].ParameterType), f => f.castclass(_parameters[i].ParameterType)))
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .If(returnType != typeof(void), e => e.box(returnType), e => e.ldnull.end())
            .ret();

            return (Func<object?, object[], Task<object?>>)dm.CreateDelegate(typeof(Func<object, object[], Task<object>>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="parameters">方法的参数。</param>
        /// <returns></returns>
        public Task<object?> InvokeAsync(object? instance, params object[] parameters)
        {
            if (parameters.Length != _parameters.Length)
            {
                throw new Exception($"参数个数不匹配，应为 {_parameters.Length} 个。");
            }

            return _invoker(instance, parameters);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return InvokeAsync(instance, parameters).Result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class AsyncMethodInvoker<TValue> : IAsyncMethodInvoker<TValue>, IMethodInvoker
    {
        private readonly Func<object?, object[], Task<TValue?>> _invoker;
        private ParameterInfo[] _parameters;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public AsyncMethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, object[], Task<TValue?>> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(Task<TValue>))
            {
                throw new ArgumentException($"方法 {methodInfo.Name} 的返回类型必须与 TValue 一致。");
            }

            _parameters = methodInfo.GetParameters();

            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(Task<TValue>),
                new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .For(0, _parameters.Length, (e, i) => e.ldarg_1.ldc_i4(i).ldelem_ref.If(_parameters[i].ParameterType.IsValueType, f => f.unbox_any(_parameters[i].ParameterType), f => f.castclass(_parameters[i].ParameterType)))
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .ret();

            return (Func<object?, object[], Task<TValue?>>)dm.CreateDelegate(typeof(Func<object, object[], Task<TValue>>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="parameters">方法的参数。</param>
        /// <returns></returns>
        public Task<TValue?> InvokeAsync(object? instance, params object[] parameters)
        {
            if (parameters?.Length != _parameters.Length)
            {
                throw new Exception($"参数个数不匹配，应为 {_parameters.Length} 个。");
            }

            return _invoker(instance, parameters);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return InvokeAsync(instance, parameters).Result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    public class AsyncMethodInvoker<TArg1, TValue> : IAsyncMethodInvoker<TArg1, TValue>, IMethodInvoker
    {
        private readonly Func<object?, TArg1?, Task<TValue?>> _invoker;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public AsyncMethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, TArg1?, Task<TValue?>> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(Task) && methodInfo.ReturnType != typeof(ValueTask) && methodInfo.ReturnType.GetGenericArguments()[0] != typeof(TValue))
            {
                throw new ArgumentException($"方法 {methodInfo.Name} 的返回类型必须与 TValue 一致。");
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 1)
            {
                throw new Exception($"参数个数不匹配，应为 1 个。");
            }

            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(Task<TValue>),
                new Type[] { typeof(object), typeof(TArg1) }, methodInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .ldarg_1
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .ret();

            return (Func<object?, TArg1?, Task<TValue?>>)dm.CreateDelegate(typeof(Func<object, TArg1, Task<TValue>>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">方法的参数。</param>
        /// <returns></returns>
        public Task<TValue?> InvokeAsync(object? instance, TArg1? arg1)
        {
            return _invoker(instance, arg1);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return InvokeAsync(instance, (TArg1)parameters[0]).Result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    public class AsyncMethodInvoker<TArg1, TArg2, TValue> : IAsyncMethodInvoker<TArg1, TArg2, TValue>, IMethodInvoker
    {
        private readonly Func<object?, TArg1?, TArg2?, Task<TValue?>> _invoker;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public AsyncMethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, TArg1?, TArg2?, Task<TValue?>> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(Task<TValue>))
            {
                throw new ArgumentException($"方法 {methodInfo.Name} 的返回类型必须与 TValue 一致。");
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 2)
            {
                throw new Exception($"参数个数不匹配，应为 2 个。");
            }

            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(Task<TValue>),
                new Type[] { typeof(object), typeof(TArg1), typeof(TArg2) }, methodInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .ldarg_1.ldarg_2
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .ret();

            return (Func<object?, TArg1?, TArg2?, Task<TValue?>>)dm.CreateDelegate(typeof(Func<object, TArg1, TArg2, Task<TValue>>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">方法的参数。</param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public Task<TValue?> InvokeAsync(object? instance, TArg1? arg1, TArg2? arg2)
        {
            return _invoker(instance, arg1, arg2);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return InvokeAsync(instance, (TArg1)parameters[0], (TArg2)parameters[1]).Result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    /// <typeparam name="TArg3"></typeparam>
    public class AsyncMethodInvoker<TArg1, TArg2, TArg3, TValue> : IAsyncMethodInvoker<TArg1, TArg2, TArg3, TValue>, IMethodInvoker
    {
        private readonly Func<object?, TArg1?, TArg2?, TArg3?, Task<TValue?>> _invoker;

        /// <summary>
        /// 初始化 <see cref="MethodInvoker"/> 类的新实例。
        /// </summary>
        /// <param name="methodInfo">要包装的 <see cref="MethodInfo"/> 对象。</param>
        public AsyncMethodInvoker(MethodInfo methodInfo)
        {
            Guard.ArgumentNull(methodInfo, nameof(methodInfo));

            _invoker = CreateInvokerDelegate(methodInfo);
        }

        private Func<object?, TArg1?, TArg2?, TArg3?, Task<TValue?>> CreateInvokerDelegate(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(Task<TValue>))
            {
                throw new ArgumentException($"方法 {methodInfo.Name} 的返回类型必须与 TValue 一致。");
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 3)
            {
                throw new Exception($"参数个数不匹配，应为 3 个。");
            }

            var dm = new DynamicMethod($"{methodInfo.Name}_Invoker", typeof(Task<TValue>),
                new Type[] { typeof(object), typeof(TArg1), typeof(TArg2), typeof(TArg3) }, methodInfo.DeclaringType, true);

            var emiter = new EmitHelper(dm.GetILGenerator());
            emiter.If(!methodInfo.IsStatic, e => e.ldarg_0.end())
            .ldarg_1.ldarg_2.ldarg_3
            .If(methodInfo.IsStatic || methodInfo.DeclaringType.IsValueType, e => e.call(methodInfo), e => e.callvirt(methodInfo))
            .ret();

            return (Func<object?, TArg1?, TArg2?, TArg3?, Task<TValue?>>)dm.CreateDelegate(typeof(Func<object, TArg1, TArg2, TArg3?, Task<TValue>>));
        }

        /// <summary>
        /// 使用指定的参数调用当前实例的方法。
        /// </summary>
        /// <param name="instance">实例对象。</param>
        /// <param name="arg1">方法的参数。</param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public Task<TValue?> InvokeAsync(object? instance, TArg1? arg1, TArg2? arg2, TArg3? arg3)
        {
            return _invoker(instance, arg1, arg2, arg3);
        }

        object? IMethodInvoker.Invoke(object? instance, params object[] parameters)
        {
            return InvokeAsync(instance, (TArg1)parameters[0], (TArg2)parameters[1], (TArg3)parameters[2]).Result;
        }
    }
}
